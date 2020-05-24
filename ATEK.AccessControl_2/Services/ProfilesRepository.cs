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

        public async Task<Profile> AddProfileAsync(Profile profile)
        {
            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        public async Task DeleteProfileAsync(int id)
        {
            var profile = _context.Profiles.FirstOrDefault(c => c.Id == id);
            if (profile != null)
            {
                _context.Profiles.Remove(profile);
            }
            await _context.SaveChangesAsync();
        }

        public Task<Profile> GetProfileAsync(int id)
        {
            return _context.Profiles.FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task<List<Profile>> GetProfilesAsync()
        {
            return _context.Profiles.ToListAsync();
        }

        public async Task<Profile> UpdateProfileAsync(Profile profile)
        {
            if (!_context.Profiles.Local.Any(p => p.Id == profile.Id))
            {
                _context.Profiles.Attach(profile);
            }
            _context.Entry(profile).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return profile;
        }
    }
}