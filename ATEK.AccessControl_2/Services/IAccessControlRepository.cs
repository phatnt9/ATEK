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
        void SaveChanges();

        #region Groups

        IEnumerable<Group> GetGroups();

        Group GetGroupWithAllRelatedData(int groupId);

        void AddGroup(Group group);

        void UpdateGroup(Group group);

        void RemoveGroups(IEnumerable<Group> groups);

        IEnumerable<Profile> LoadGroupProfiles(int groupId);

        bool AddProfileToGroup(Group group, Profile profile);

        Task AddProfilesToGroupAsync(Group group, IEnumerable<Profile> collection);

        bool RemoveProfileFromGroup(Group group, Profile profile);

        Task RemoveProfilesFromGroupAsync(Group group, IEnumerable<Profile> collection);

        #endregion Groups

        #region Profiles

        IEnumerable<Profile> GetProfiles();

        void AddProfile(Profile profile);

        void AddProfiles(IEnumerable<Profile> profiles);

        void UpdateProfile(Profile profile);

        void RemoveProfiles(IEnumerable<Profile> profiles);

        #endregion Profiles

        #region Classes

        IEnumerable<Class> GetClasses();

        void AddClass(Class @class);

        void AddProfileToClass(int classId, Profile profile);

        void AddProfilesToClass(int classId, IEnumerable<Profile> profiles);

        void UpdateClass(Class @class);

        void RemoveClasses(IEnumerable<Class> classes);

        int CheckClassNameValid(string className);

        IEnumerable<Profile> LoadClassProfiles(int classId);

        #endregion Classes
    }
}