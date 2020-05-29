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
    public class AccessControlRepository : IAccessControlRepository
    {
        private AccessControlContext _context;

        public AccessControlRepository()
        {
            _context = new AccessControlContext();
        }

        #region Profiles

        public List<Profile> GetProfiles()
        {
            return _context.Profiles.ToList();
        }

        public Profile AddProfile(Profile profile)
        {
            _context.Profiles.Add(profile);
            _context.SaveChanges();
            return profile;
        }

        public void AddProfiles(IEnumerable<Profile> profiles)
        {
            _context.Profiles.AddRange(profiles);
            _context.SaveChanges();
        }

        public Profile UpdateProfile(Profile profile)
        {
            if (!_context.Profiles.Local.Any(c => c.Id == profile.Id))
            {
                _context.Profiles.Attach(profile);
            }
            _context.Entry(profile).State = EntityState.Modified;
            _context.SaveChanges();
            return profile;
        }

        public void RemoveProfiles(IEnumerable<Profile> profiles)
        {
            _context.RemoveRange(profiles);
            _context.SaveChanges();
        }

        #endregion Profiles

        #region Classes

        public List<Class> GetClasses()
        {
            return _context.Classes.ToList();
        }

        public Class AddClass(Class @class)
        {
            _context.Classes.Add(@class);
            _context.SaveChanges();
            return @class;
        }

        public Class UpdateClass(Class @class)
        {
            if (!_context.Classes.Local.Any(c => c.Id == @class.Id))
            {
                _context.Classes.Attach(@class);
            }
            _context.Entry(@class).State = EntityState.Modified;
            _context.SaveChanges();
            return @class;
        }

        public void RemoveClasses(IEnumerable<Class> classes)
        {
            _context.RemoveRange(classes);
            _context.SaveChanges();
        }

        public int CheckClassNameValid(string className)
        {
            var @class = _context.Classes.FirstOrDefault((c) => c.Name == className);
            if (@class != null)
            {
                return @class.Id;
            }
            else
            {
                return 0;
            }
        }

        public Class AddProfilesToClass(int classId, List<Profile> profiles)
        {
            var @class = _context.Classes.Find(classId);
            @class.Profiles.AddRange(profiles);
            _context.SaveChanges();
            return @class;
        }

        public Class AddProfileToClass(int classId, Profile profile)
        {
            var @class = _context.Classes.Find(classId);
            @class.Profiles.Add(profile);
            _context.SaveChanges();
            return @class;
        }

        public Class GetClassIncludeProfiles(int classId)
        {
            var @class = _context.Classes.Where(c => c.Id == classId).Include(c => c.Profiles).FirstOrDefault();
            return @class;
        }

        public Class LoadClassProfiles(int classId)
        {
            Class @class = _context.Classes.Include(c => c.Profiles).SingleOrDefault(c => c.Id == classId);
            return @class;
        }

        #endregion Classes

        #region Groups

        public List<Group> GetGroups()
        {
            return _context.Groups.ToList();
        }

        public Group AddGroup(Group group)
        {
            _context.Groups.Add(group);
            _context.SaveChanges();
            return group;
        }

        public Group UpdateGroup(Group group)
        {
            if (!_context.Groups.Local.Any(c => c.Id == group.Id))
            {
                _context.Groups.Attach(group);
            }
            _context.Entry(group).State = EntityState.Modified;
            _context.SaveChanges();
            return group;
        }

        public void RemoveGroups(IEnumerable<Group> groups)
        {
            _context.RemoveRange(groups);
            _context.SaveChanges();
        }

        #endregion Groups
    }
}