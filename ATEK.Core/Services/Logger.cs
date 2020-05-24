using MvvmCross.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ATEK.Core.Services
{
    internal class Logger : IMvxLog
    {
        public bool IsLogLevelEnabled(MvxLogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public bool Log(MvxLogLevel logLevel, Func<string> messageFunc, Exception exception = null, params object[] formatParameters)
        {
            throw new NotImplementedException();
        }
    }
}