using ATEK.AccessControl_2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.AccessControl_2.Gates
{
    public class GatesViewModel : ValidatableBindableBase
    {
        private readonly IAccessControlRepository repo;

        public GatesViewModel(IAccessControlRepository repo)
        {
            this.repo = repo;
        }
    }
}