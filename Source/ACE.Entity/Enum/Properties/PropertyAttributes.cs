using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ACE.Entity.Enum.Properties
{
    public static class PropertyAttributes
    {
        private static Type[] pTypes = new Type[]
        {
            typeof(PropertyInt),
            typeof(PropertyInt64),
            typeof(PropertyFloat),
            typeof(PropertyString),
            typeof(PropertyBool)
        };

        private static Type[] aTypes = new Type[]
        {
            typeof(CloneAttribute)
        };

        private static Dictionary<Type, Dictionary<Type, object>> EnumDict;

        private static object GetProperties(Type ptype, Type atype)
        {
            var list = ptype.GetFields().Select(x => new
            {
                att = x.GetCustomAttributes(atype, false).FirstOrDefault(),
                member = x
            }).Where(x => x.att != null && x.member.Name != "value__")
            .Select(x => x.member.GetValue(null))
            .ToList();

            var method1 = typeof(Enumerable).GetMethods()
                .Where(x => x.Name == "Cast")
                .Single()
                .MakeGenericMethod(ptype);
            var list2 = method1.Invoke(null, new object[] { list });
            var method = typeof(System.Collections.Immutable.ImmutableHashSet)
                .GetMethods().Where(x => x.Name == "ToImmutableHashSet")
                .Where(x => x.GetParameters().Length == 1)
                .Where(x => x.GetParameters()[0].ParameterType.Name == "IEnumerable`1")
                .Single()
                .MakeGenericMethod(ptype);
            var hashSet = method.Invoke(null, new object[] { list2 });
            return hashSet;
        }

        private static bool _initialized = false;
        public static void Initialize()
        {
            if (_initialized)
                return;
            EnumDict = new Dictionary<Type, Dictionary<Type, object>>();
            foreach (var ptype in pTypes)
            {
                var dict = new Dictionary<Type, object>();
                EnumDict[ptype] = dict;
                foreach (var atype in aTypes)
                {
                    var hashSet = GetProperties(ptype, atype);
                    dict[atype] = hashSet;
                }
            }
            _initialized = true;
        }

        public static ImmutableHashSet<TProperty> GetProperties<TProperty, TAttribute>()
        {
            if (!EnumDict.TryGetValue(typeof(TProperty), out var dict2))
                return null;
            if (!dict2.TryGetValue(typeof(TAttribute), out var set))
                return null;
            return (ImmutableHashSet<TProperty>)set;
        }
    }
}
