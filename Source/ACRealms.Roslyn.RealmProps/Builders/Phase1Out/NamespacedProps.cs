using System;
using System.Collections.Generic;
using System.Text;

namespace ACRealms.Roslyn.RealmProps.Builders.Phase1
{
    // Builds the classes Props.XXX.YYY
    internal static class NamespacedProps
    {
        internal static string GenerateNamespacedPropsSourceCode(NamespaceData data)
        {
            try
            {
                return $$"""
                    {{data.ToCompilationSource()}}
                    """;
            }
            catch (Exception ex)
            {
                Helpers.ReThrowWrappedException($"NamespacedProps.GenerateNamespacedPropsSourceCode ({data.NamespaceFull})", ex);
                throw;
            }
        }
    }
}
