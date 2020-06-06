using ATEK.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2.Services
{
    public interface IAccessControlRepository
    {
        void TestFirebase();

        void SaveChanges();

        IEnumerable<Profile> GetProfiles();

        IEnumerable<Class> GetClasses();

        IEnumerable<Gate> GetGates();

        IEnumerable<Group> GetGroups();

        bool AddProfile(Profile profile);

        void AddClass(Class @class);

        void AddGroup(Group group);

        bool AddProfileGroup(Profile profile, Group group);

        bool AddProfileGate(Profile profile, Gate gate);

        void UpdateProfile(Profile profile);

        void UpdateClass(Class @class);

        void UpdateGroup(Group group);

        void RemoveProfiles(IEnumerable<Profile> profiles);

        void RemoveClasses(IEnumerable<Class> classes);

        void RemoveGroups(IEnumerable<Group> groups);

        bool RemoveProfileGroup(Profile profile, Group group);

        bool RemoveProfileGate(Profile profile, Gate gate);

        IEnumerable<Profile> LoadProfilesOfGroup(int groupId);

        IEnumerable<Group> LoadGroupsOfProfile(int profileId);

        IEnumerable<Gate> LoadGatesOfProfile(int profileId);
    }
}