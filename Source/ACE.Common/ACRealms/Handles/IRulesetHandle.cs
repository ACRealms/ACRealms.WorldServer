using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Rulesets
{
    public interface IRulesetHandle
    {
        void Modulate(IModulatable target);
    }
}
