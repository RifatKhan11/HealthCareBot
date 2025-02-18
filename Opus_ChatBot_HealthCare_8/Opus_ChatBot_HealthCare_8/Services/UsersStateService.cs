using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Services.IServices;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class UsersStateService : IUsersStateService
    {
        private readonly ApplicationDbContext _contex;

        public UsersStateService(ApplicationDbContext contex)
        {
            _contex = contex;
        }

        public string GetUsersCurrentState()
        {
            throw new NotImplementedException();
        }
    }
}
