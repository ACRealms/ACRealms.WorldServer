using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRealms.Rulesets
{
    public interface IPropertyContainers
    {
        IDictionaryConvertibleKeyStructValue<ushort> CovariantPropertiesInt { get; }
        IDictionaryConvertibleKeyStructValue<ushort> CovariantPropertiesInt64 { get; }
        IDictionaryConvertibleKeyStructValue<ushort> CovariantPropertiesFloat { get; }
        IDictionaryConvertibleKeyStructValue<ushort> CovariantPropertiesBool { get; }
        IDictionaryConvertibleKeyStructValue<ushort> CovariantPropertiesString { get; }
    }
}
