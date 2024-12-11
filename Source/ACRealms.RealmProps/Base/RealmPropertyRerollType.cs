using System;
using System.Collections.Generic;
using System.Text;

namespace ACRealms.Rulesets.Enums // Belongs in this namespace but must be used in this project
{
    public enum RealmPropertyRerollType : byte
    {
        never = 1,
        always = 2,
        landblock = 3,
        manual = 4,
    }
}
