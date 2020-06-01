using ATEK.Data.Contexts;
using ATEK.Domain.Models;
using Microsoft.Data.SqlClient;
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

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        #region Profiles

        public IEnumerable<Profile> GetProfiles()
        {
            return _context.Profiles.ToList();
        }

        public Profile GetProfilesWithAllRelatedData(Profile profile)
        {
            return _context.Profiles
                 .Include(p => p.Class)
                 .Include(p => p.ProfileGates)
                 .ThenInclude(p => p.Gate)
                 .Include(p => p.ProfileGroups)
                 .ThenInclude(p => p.Group)
                 .FirstOrDefault(p => p.Id == profile.Id);
        }

        public void AddProfile(Profile profile)
        {
            _context.Profiles.Add(profile);
            _context.SaveChanges();
        }

        public void AddProfiles(IEnumerable<Profile> profiles)
        {
            try
            {
                _context.Profiles.AddRange(profiles);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public void UpdateProfile(Profile profile)
        {
            if (!_context.Profiles.Local.Any(c => c.Id == profile.Id))
            {
                _context.Profiles.Attach(profile);
            }
            _context.Entry(profile).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void RemoveProfiles(IEnumerable<Profile> profiles)
        {
            _context.RemoveRange(profiles);
            _context.SaveChanges();
        }

        #endregion Profiles

        #region Classes

        public IEnumerable<Class> GetClasses()
        {
            return _context.Classes.ToList();
        }

        public void AddClass(Class @class)
        {
            _context.Classes.Add(@class);
            _context.SaveChanges();
        }

        public void UpdateClass(Class @class)
        {
            if (!_context.Classes.Local.Any(c => c.Id == @class.Id))
            {
                _context.Classes.Attach(@class);
            }
            _context.Entry(@class).State = EntityState.Modified;
            _context.SaveChanges();
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

        public void AddProfilesToClass(int classId, IEnumerable<Profile> profiles)
        {
            var @class = _context.Classes.Find(classId);
            @class.Profiles.AddRange(profiles);
            _context.SaveChanges();
        }

        public void AddProfileToClass(int classId, Profile profile)
        {
            var @class = _context.Classes.Find(classId);
            @class.Profiles.Add(profile);
            _context.SaveChanges();
        }

        public IEnumerable<Profile> LoadClassProfiles(int classId)
        {
            Class @class = _context.Classes.Include(c => c.Profiles).SingleOrDefault(c => c.Id == classId);
            return @class.Profiles;
        }

        #endregion Classes

        #region Groups

        public IEnumerable<Group> GetGroups()
        {
            return _context.Groups.ToList();
        }

        public Group GetGroupWithAllRelatedData(int groupId)
        {
            return _context.Groups
                .Include(g => g.ProfileGroups)
                .ThenInclude(g => g.Profile)
                .First(g => g.Id == groupId);
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
            _context.SaveChanges();
        }

        public void UpdateGroup(Group group)
        {
            if (!_context.Groups.Local.Any(c => c.Id == group.Id))
            {
                _context.Groups.Attach(group);
            }
            _context.Entry(group).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void RemoveGroups(IEnumerable<Group> groups)
        {
            _context.RemoveRange(groups);
            _context.SaveChanges();
        }

        public IEnumerable<Profile> LoadGroupProfiles(int groupId)
        {
            var group = _context.Groups.Where(g => g.Id == groupId)
                .Select(g => new
                {
                    Group = g,
                    Profiles = g.ProfileGroups.Select(pg => pg.Profile)
                }).FirstOrDefault();
            return group.Profiles;
        }

        public bool AddProfileToGroup(Group group, Profile profile)
        {
            var addItem = new ProfileGroup() { GroupId = group.Id, ProfileId = profile.Id };
            _context.Add(addItem);
            _context.SaveChanges();
            return true;
            //var data = group.ProfileGroups.FirstOrDefault(g => g.ProfileId == profile.Id);
            //if (data != null)
            //{
            //    Console.WriteLine($"Co ne.{profile.Id}");
            //    _context.Entry(data).State = EntityState.Added;
            //    _context.SaveChanges();
            //    return true;
            //}
            //else
            //{
            //    Console.WriteLine($"Ko co.{profile.Id}");
            //    var addItem = new ProfileGroup() { GroupId = group.Id, ProfileId = profile.Id };
            //    _context.Add(addItem);
            //    _context.SaveChanges();
            //    return true;
            //}
        }

        public async Task AddProfilesToGroupAsync(Group group, IEnumerable<Profile> collection)
        {
            Console.WriteLine("ADD:");
            foreach (var item in collection)
            {
                var data = group.ProfileGroups.FirstOrDefault(g => g.ProfileId == item.Id);
                if (data != null)
                {
                    Console.WriteLine($"Add Co ne.{item.Id}");
                    _context.Entry(data).State = EntityState.Added;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine($"Add Ko co.{item.Id}");
                    var addItem = new ProfileGroup() { GroupId = group.Id, ProfileId = item.Id };
                    _context.Add(addItem);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public bool RemoveProfileFromGroup(Group group, Profile profile)
        {
            var data = group.ProfileGroups.FirstOrDefault(g => g.ProfileId == profile.Id);
            _context.Entry(data).State = EntityState.Deleted;
            _context.SaveChanges();
            return true;
            //var data = group.ProfileGroups.FirstOrDefault(g => g.ProfileId == profile.Id);
            //if (data != null)
            //{
            //    Console.WriteLine($"Remove Co ne.{profile.Id}");
            //    _context.Entry(data).State = EntityState.Deleted;
            //    _context.SaveChanges();
            //    return true;
            //}
            //else
            //{
            //    Console.WriteLine($"Remove Ko co.{profile.Id}");
            //    var removeItem = new ProfileGroup() { GroupId = group.Id, ProfileId = profile.Id };
            //    _context.Remove(removeItem);
            //    _context.SaveChanges();
            //    return true;
            //}
        }

        public async Task RemoveProfilesFromGroupAsync(Group group, IEnumerable<Profile> collection)
        {
            Console.WriteLine("REMOVE:");
            foreach (var item in collection)
            {
                var data = group.ProfileGroups.FirstOrDefault(g => g.ProfileId == item.Id);
                if (data != null)
                {
                    Console.WriteLine($"Co ne.{item.Id}");
                    _context.Entry(data).State = EntityState.Deleted;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine($"Ko co.{item.Id}");
                    var removeItem = new ProfileGroup() { GroupId = group.Id, ProfileId = item.Id };
                    _context.Remove(removeItem);
                    await _context.SaveChangesAsync();
                }
            }
        }

        #endregion Groups

        public virtual void HandleException(Exception exception)
        {
            Console.WriteLine("ERROR IN DATABASE CONTEXT");
            SqlException innerException = exception.InnerException as SqlException;
            if (innerException != null)
            {
                switch (innerException.Number)
                {
                    case 2601:
                        {
                            Console.WriteLine(innerException.Message);
                            Console.WriteLine("Duplicated Pinno");
                            break;
                        }
                    case 2627:
                        {
                            Console.WriteLine(innerException.Message);
                            Console.WriteLine("Duplicated GroupProfiles");
                            break;
                        }
                    default:
                        {
                            throw new NotImplementedException();
                        }
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}