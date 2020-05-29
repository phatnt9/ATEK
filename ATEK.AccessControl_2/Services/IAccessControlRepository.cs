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
        #region Groups

        List<Group> GetGroups();

        Group AddGroup(Group group);

        Group UpdateGroup(Group group);

        void RemoveGroups(IEnumerable<Group> groups);

        #endregion Groups

        #region Profiles

        List<Profile> GetProfiles();

        Profile AddProfile(Profile profile);

        void AddProfiles(IEnumerable<Profile> profiles);

        Profile UpdateProfile(Profile profile);

        void RemoveProfiles(IEnumerable<Profile> profiles);

        #endregion Profiles

        #region Classes

        List<Class> GetClasses();

        Class AddClass(Class @class);

        Class AddProfileToClass(int classId, Profile profile);

        Class AddProfilesToClass(int classId, List<Profile> profiles);

        Class UpdateClass(Class @class);

        void RemoveClasses(IEnumerable<Class> classes);

        Class GetClassIncludeProfiles(int classId);

        int CheckClassNameValid(string className);

        Class LoadClassProfiles(int classId);

        #endregion Classes
    }
}