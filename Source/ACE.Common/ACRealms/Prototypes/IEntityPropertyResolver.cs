using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Prototypes
{
    public interface IEntityPropertyResolver
    {
        static virtual bool RespondsTo(string key) => throw new NotImplementedException();
        static abstract Type TypeOfProperty(string key);
    }
}
