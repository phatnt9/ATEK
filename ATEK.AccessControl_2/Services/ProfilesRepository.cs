using ATEK.Data.Contexts;
using ATEK.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2.Services
{
    public class ProfilesRepository : IProfilesRepository
    {
        private AccessControlContext _context;

        public ProfilesRepository()
        {
            _context = new AccessControlContext();
        }

        public Task<List<Profile>> GetProfilesAsync()
        {
            return _context.Profiles.ToListAsync();
        }

        public List<Profile> GetProfiles()
        {
            return _context.Profiles.ToList();
        }
    }
}