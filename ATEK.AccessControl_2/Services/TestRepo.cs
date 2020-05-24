using ATEK.Data.Contexts;
using ATEK.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2.Services
{
    public class TestRepo
    {
        private AccessControlContext _context = new AccessControlContext();

        public List<Profile> GetProfiles()
        {
            return new List<Profile>();
        }
    }
}