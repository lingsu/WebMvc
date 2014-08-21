using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lxs.Core.Domain.Users;

namespace Lxs.Services.Users
{
    public interface IUserService
    {
        User GetCustomerByUsername(string usernameOrEmail);

        User GetCustomerByEmail(string usernameOrEmail);

        User GetCustomerByGuid(Guid customerGuid);

        User InsertGuestCustomer();
    }
}
