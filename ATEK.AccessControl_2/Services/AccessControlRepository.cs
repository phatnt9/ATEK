using ATEK.Data.Contexts;
using ATEK.Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2.Services
{
    public class AccessControlRepository : IAccessControlRepository
    {
        private AccessControlContext _context;
        private readonly IFirebaseControlRepository firebase;

        public AccessControlRepository(IFirebaseControlRepository _firebase)
        {
            _context = new AccessControlContext();
            firebase = _firebase;
            //TestFirebase();
        }

        public void TestFirebase()
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public IEnumerable<Profile> GetProfiles()
        {
            return _context.Profiles.Include(p => p.ProfileGates).Include(p => p.ProfileGroups).ToList();
        }

        public IEnumerable<Class> GetClasses()
        {
            return _context.Classes.Include(c => c.Profiles).ToList();
        }

        public IEnumerable<Gate> GetGates()
        {
            return _context.Gates.Include(g => g.ProfileGates).ToList();
        }

        public IEnumerable<Group> GetGroups()
        {
            return _context.Groups.Include(g => g.ProfileGroups).ToList();
        }

        public bool AddProfile(Profile profile)
        {
            try
            {
                _context.Profiles.Add(profile);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                if (HandleException(ex))
                {
                    _context.Profiles.Remove(profile);
                }
                return false;
            }
        }

        public void AddClass(Class @class)
        {
            _context.Classes.Add(@class);
            _context.SaveChanges();
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
            _context.SaveChanges();
        }

        public bool AddProfileGroup(Profile profile, Group group)
        {
            var addItem = new ProfileGroup() { GroupId = group.Id, ProfileId = profile.Id };
            _context.Add(addItem);
            _context.SaveChanges();
            return true;

            //var data = group.ProfileGroups.FirstOrDefault(g => g.ProfileId == profile.Id);
            //if (data != null)
            //{
            //    Console.WriteLine($"Add Co ne.{profile.Id}");
            //    Console.WriteLine(_context.Entry(data).State.ToString());
            //    _context.Entry(data).State = EntityState.Added;
            //    _context.SaveChanges();
            //    return true;
            //}
            //else
            //{
            //    Console.WriteLine($"Add Ko co.{profile.Id}");
            //    var addItem = new ProfileGroup() { GroupId = group.Id, ProfileId = profile.Id };
            //    _context.Add(addItem);
            //    Console.WriteLine(_context.Entry(addItem).State.ToString());
            //    _context.SaveChanges();
            //    return true;
            //}
        }

        public bool AddProfileGate(Profile profile, Gate gate)
        {
            var addItem = new ProfileGate() { GateId = gate.Id, ProfileId = profile.Id };
            _context.Add(addItem);
            _context.SaveChanges();
            return true;
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

        public void UpdateClass(Class @class)
        {
            if (!_context.Classes.Local.Any(c => c.Id == @class.Id))
            {
                _context.Classes.Attach(@class);
            }
            _context.Entry(@class).State = EntityState.Modified;
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

        public void RemoveProfiles(IEnumerable<Profile> profiles)
        {
            _context.RemoveRange(profiles);
            _context.SaveChanges();
        }

        public void RemoveClasses(IEnumerable<Class> classes)
        {
            _context.RemoveRange(classes);
            _context.SaveChanges();
        }

        public void RemoveGroups(IEnumerable<Group> groups)
        {
            _context.RemoveRange(groups);
            _context.SaveChanges();
        }

        public bool RemoveProfileGroup(Profile profile, Group group)
        {
            //var data = group.ProfileGroups.FirstOrDefault(g => g.ProfileId == profile.Id);
            //_context.Entry(data).State = EntityState.Deleted;
            //_context.SaveChanges();
            //return true;

            var data = group.ProfileGroups.FirstOrDefault(g => g.ProfileId == profile.Id);
            if (data != null)
            {
                Console.WriteLine($"Remove Co ne.{profile.Id}");
                _context.Entry(data).State = EntityState.Deleted;
                _context.SaveChanges();
                return true;
            }
            else
            {
                Console.WriteLine($"Remove Ko co.{profile.Id}");
                var removeItem = new ProfileGroup() { GroupId = group.Id, ProfileId = profile.Id };
                _context.Remove(removeItem);
                _context.SaveChanges();
                return true;
            }
        }

        public bool RemoveProfileGate(Profile profile, Gate gate)
        {
            var data = gate.ProfileGates.FirstOrDefault(g => g.ProfileId == profile.Id);
            if (data != null)
            {
                Console.WriteLine($"Remove Co ne.{profile.Id}");
                _context.Entry(data).State = EntityState.Deleted;
                _context.SaveChanges();
                return true;
            }
            else
            {
                Console.WriteLine($"Remove Ko co.{profile.Id}");
                var removeItem = new ProfileGate() { GateId = gate.Id, ProfileId = profile.Id };
                _context.Remove(removeItem);
                _context.SaveChanges();
                return true;
            }
        }

        public IEnumerable<Profile> LoadProfilesOfGroup(int groupId)
        {
            var group = _context.Groups.AsQueryable().Where(g => g.Id == groupId)
                .Select(g => new
                {
                    Group = g,
                    Profiles = g.ProfileGroups.Select(pg => pg.Profile)
                }).FirstOrDefault();
            return group.Profiles;
        }

        public IEnumerable<Group> LoadGroupsOfProfile(int profileId)
        {
            var profile = _context.Profiles.AsQueryable().Where(p => p.Id == profileId)
             .Select(p => new
             {
                 Profile = p,
                 Groups = p.ProfileGroups.Select(pg => pg.Group)
             }).FirstOrDefault();
            return profile.Groups;
        }

        public IEnumerable<Gate> LoadGatesOfProfile(int profileId)
        {
            var profile = _context.Profiles.AsQueryable().Where(p => p.Id == profileId)
             .Select(p => new
             {
                 Profile = p,
                 Gates = p.ProfileGates.Select(pg => pg.Gate)
             }).FirstOrDefault();
            return profile.Gates;
        }

        public virtual bool HandleException(Exception exception)
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
                            return true;
                        }
                    case 2627:
                        {
                            Console.WriteLine(innerException.Message);
                            Console.WriteLine("Duplicated GroupProfiles");
                            return true;
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