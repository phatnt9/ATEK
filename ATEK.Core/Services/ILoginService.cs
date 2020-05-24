using System;
using System.Collections.Generic;
using System.Text;

namespace ATEK.Core.Services
{
    public interface ILoginService
    {
        bool Login(string userName, string password);
    }
}