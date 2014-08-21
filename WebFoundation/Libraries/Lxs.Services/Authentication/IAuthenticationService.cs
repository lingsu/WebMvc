using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lxs.Core.Domain.Users;

namespace Lxs.Services.Authentication
{
    public partial interface IAuthenticationService
    {
        void SignIn(User user, bool createPersistentCookie);
        void SignOut();
        User GetAuthenticatedUser();
    }
}
