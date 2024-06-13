using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.Realms.Peripherals
{
    internal abstract class Peripheral<T> : PeripheralBase
        where T : Peripheral<T>, new()
    {

        protected static T ReportLoadErrorAndDefault(Exception e)
        {
            log.Warn($"{e}\nError loading peripheral {typeof(T).FullName}. Using default configuration.");
            return new T();
        }
    }
}
