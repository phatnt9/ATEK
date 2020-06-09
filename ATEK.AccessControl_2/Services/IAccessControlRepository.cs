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
        void Firebase_AddProfileAsync(Profile profile);

        IEnumerable<Gate> Firebase_GetGates();

        void SaveChanges();

        IEnumerable<Profile> GetProfiles();

        IEnumerable<Class> GetClasses();

        IEnumerable<Gate> GetGates();

        IEnumerable<Group> GetGroups();

        bool AddProfile(Profile profile);

        bool AddClass(Class @class);

        bool AddGate(Gate gate);

        bool AddGroup(Group group);

        bool AddProfileGroup(Profile profile, Group group);

        bool AddProfileGate(Profile profile, Gate gate);

        bool UpdateProfile(Profile profile);

        bool UpdateClass(Class @class);

        bool UpdateGate(Gate gate);

        bool UpdateGroup(Group group);

        bool RemoveProfiles(IEnumerable<Profile> profiles);

        bool RemoveClasses(IEnumerable<Class> classes);

        bool RemoveGates(IEnumerable<Gate> gates);

        bool RemoveGroups(IEnumerable<Group> groups);

        bool RemoveProfileGroup(Profile profile, Group group);

        bool RemoveProfileGate(Profile profile, Gate gate);

        IEnumerable<Profile> LoadProfilesOfGroup(int groupId);

        IEnumerable<Profile> LoadProfilesOfGate(int gateId);

        IEnumerable<Group> LoadGroupsOfProfile(int profileId);

        IEnumerable<Gate> LoadGatesOfProfile(int profileId);
    }
}