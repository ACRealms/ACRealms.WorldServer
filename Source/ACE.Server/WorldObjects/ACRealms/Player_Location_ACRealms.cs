using ACE.Entity.Enum.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.WorldObjects
{
    partial class Player
    {
        public ushort DefaultShortInstanceID
        {
            get
            {
                if (GetProperty(PropertyBool.AttemptUniqueInstanceID) == true)
                    return (ushort)((Character.Id % 0xFFFE) + 1);
                return 0;
            }
        }
    }
}
