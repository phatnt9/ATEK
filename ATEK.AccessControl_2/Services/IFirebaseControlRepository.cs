using ATEK.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2.Services
{
    public interface IFirebaseControlRepository
    {
        void ReadDataFromFireBase();

        void AddProfile(Profile profile);
    }
}