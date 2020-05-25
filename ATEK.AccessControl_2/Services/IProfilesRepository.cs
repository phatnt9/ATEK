using ATEK.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2.Services
{
    public interface IProfilesRepository
    {
        Task<List<Profile>> GetProfilesAsync();

        List<Profile> GetProfiles();
    }
}