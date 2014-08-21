using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Lxs.Core.Domain.Users;
using Lxs.Services.Users;

namespace Lxs.Services.Authentication
{
    public class FormsAuthenticationService:IAuthenticationService
    {
        private readonly HttpContextBase _httpContext;
        private readonly IUserService _userService;
        private readonly TimeSpan _expirationTimeSpan;

        private User _cachedUser;

        public FormsAuthenticationService(HttpContextBase httpContext,
            IUserService userService)
        {
            _httpContext = httpContext;
            _userService = userService;
            _expirationTimeSpan = FormsAuthentication.Timeout;
        }

        public void SignIn(User user, bool createPersistentCookie)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var ticket = new FormsAuthenticationTicket(
                1 /*version*/,
                user.Username,
                now,
                now.Add(_expirationTimeSpan),
                createPersistentCookie,
                user.Username,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.HttpOnly = true;
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            _httpContext.Response.Cookies.Add(cookie);
            _cachedUser = user;
        }

        public void SignOut()
        {
            _cachedUser = null;
            FormsAuthentication.SignOut();
        }

        public User GetAuthenticatedUser()
        {
            if (_cachedUser != null)
                return _cachedUser;

            if (_httpContext == null ||
                _httpContext.Request == null ||
                !_httpContext.Request.IsAuthenticated ||
                !(_httpContext.User.Identity is FormsIdentity))
            {
                return null;
            }

            var formsIdentity = (FormsIdentity)_httpContext.User.Identity;
            var user = GetAuthenticatedCustomerFromTicket(formsIdentity.Ticket);
            if (user != null && user.Active && !user.Deleted && user.IsRegistered())
                _cachedUser = user;
            return _cachedUser;
        }
        public virtual User GetAuthenticatedCustomerFromTicket(FormsAuthenticationTicket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            var usernameOrEmail = ticket.UserData;

            if (String.IsNullOrWhiteSpace(usernameOrEmail))
                return null;

            var user = _userService.GetCustomerByUsername(usernameOrEmail);
              
            return user;
        }
    }
}
