using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lxs.Core.Data;
using Lxs.Core.Domain.Users;

namespace Lxs.Services.Installation
{
    public class CodeFirstInstallationService : IInstallationService
    {
        private readonly IRepository<User> _useRepository;


        public CodeFirstInstallationService(IRepository<User> useRepository)
        {
            _useRepository = useRepository;
        }

        public void InstallUser()
        {
            var users = new List<User>
            {
                new User() {Active = true, Deleted = false, UserGuid = Guid.NewGuid(), Username = "admin"},
                new User() {Active = true, Deleted = false, UserGuid = Guid.NewGuid(), Username = "lxs"}
            };
            users.ForEach(x=>_useRepository.Insert(x));
        }

        public void InstallData()
        {
            InstallUser();
        }
    }
}
