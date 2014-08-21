using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Lxs.Core;
using Lxs.Core.Domain.Users;
using Lxs.Services.Authentication;
using Lxs.Services.Common;
using Lxs.Services.Users;

namespace Lxs.Web.Framework
{
    public class WebWorkContext:IWorkContext
    {
        #region Const

        private const string UserCookieName = "Lxs.user";

        #endregion


        #region Fields

        private readonly HttpContextBase _httpContext;
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IGenericAttributeService _genericAttributeService;

        private User _cachedUser;

        #endregion

        #region Ctor

        public WebWorkContext(HttpContextBase httpContext,
            IUserService userService,
            IAuthenticationService authenticationService,
            IGenericAttributeService genericAttributeService)
        {
            this._httpContext = httpContext;
            this._userService = userService;
            this._authenticationService = authenticationService;
            this._genericAttributeService = genericAttributeService;
            Debug.WriteLine("Create a IWorkContext");
        }

        #endregion

        protected virtual HttpCookie GetUserCookie()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            return _httpContext.Request.Cookies[UserCookieName];
        }

        protected virtual void SetUserCookie(Guid userGuid)
        {
            if (_httpContext != null && _httpContext.Response != null)
            {
                var cookie = new HttpCookie(UserCookieName);
                cookie.HttpOnly = true;
                cookie.Value = userGuid.ToString();
                if (userGuid == Guid.Empty)
                {
                    cookie.Expires = DateTime.Now.AddMonths(-1);
                }
                else
                {
                    int cookieExpires = 24 * 365; //TODO make configurable
                    cookie.Expires = DateTime.Now.AddHours(cookieExpires);
                }

                _httpContext.Response.Cookies.Remove(UserCookieName);
                _httpContext.Response.Cookies.Add(cookie);
            }
        }


        public User CurrentUser
        {
            get
            {
                if (_cachedUser != null)
                {
                    return _cachedUser;
                }

                User user = _authenticationService.GetAuthenticatedUser(); ;


                if (user==null || !user.Active || user.Deleted)
                {
                    var userCookie = GetUserCookie();

                    if (userCookie != null && !String.IsNullOrEmpty(userCookie.Value))
                    {
                        Guid customerGuid;
                        if (Guid.TryParse(userCookie.Value, out customerGuid))
                        {
                            var customerByCookie = _userService.GetCustomerByGuid(customerGuid);
                            if (customerByCookie != null &&
                                //this user (from cookie) should not be registered
                                !customerByCookie.IsRegistered())
                                user = customerByCookie;
                        }
                    }
                }

                if (user == null || !user.Active || user.Deleted)
                {
                    user = _userService.InsertGuestCustomer();
                }

                if (!user.Deleted && user.Active)
                {
                    SetUserCookie(user.UserGuid);
                    _cachedUser = user;
                }


                return _cachedUser;
            }
            set
            {
                SetUserCookie(value.UserGuid);
                _cachedUser = value;
            }
        }
        public bool IsAdmin { get; set; }
    }
}
