using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lxs.Core.Domain.Users;

namespace Lxs.Services.Users
{
    public class UserService : IUserService
    {
        public User GetCustomerByUsername(string usernameOrEmail)
        {
            throw new NotImplementedException();
        }

        public User GetCustomerByEmail(string usernameOrEmail)
        {
            throw new NotImplementedException();
        }

        public User GetCustomerByGuid(Guid customerGuid)
        {
            throw new NotImplementedException();
        }

        public User InsertGuestCustomer()
        {
            throw new NotImplementedException();
        }
    }
}
