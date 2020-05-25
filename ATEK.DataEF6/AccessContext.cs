using ATEK.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace ATEK.DataEF6
{
    public class AccessContext : DbContext
    {
        public DbSet<Profile> Profiles { get; set; }
    }
}