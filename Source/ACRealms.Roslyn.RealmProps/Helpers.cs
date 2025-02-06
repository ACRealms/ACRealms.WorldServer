using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ACRealms.Roslyn.RealmProps
{
    public static class Helpers
    {
        const bool USE_VERBOSE_STACKTRACE = true;

        public static string RemoveJsonComments(string jsonc, CancellationToken token)
        {
            var sb = new StringBuilder();
            using var reader = new StringReader(jsonc);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                token.ThrowIfCancellationRequested();
                // Remove single-line comments
                int index = line.IndexOf("//");
                if (index >= 0)
                    line = line[..index];

                // Remove block comments (simple implementation)
                // You can enhance this to handle multi-line block comments
                sb.AppendLine(line);
            }

            return sb.ToString();
        }

        private static string FormatStackTrace(Exception ex)
        {
            if (ex.StackTrace == null || ex.StackTrace.Length == 0)
                return "None";
            IEnumerable<string> frames = ex.StackTrace.Split('\n');
            if (!USE_VERBOSE_STACKTRACE)
#pragma warning disable CS0162 // Unreachable code detected
                frames = frames.Where(x => x.Contains(nameof(NamespacedRealmPropertyGenerator)));
#pragma warning restore CS0162 // Unreachable code detected
            return string.Join("; ", frames).Replace("\r", "").Replace("\n", "");
        }

        [DoesNotReturn]
        internal static void ReThrowWrappedException(string identifier, Exception originalException)
        {
            if (originalException.InnerException != null)
                ReThrowWrappedException(identifier, originalException.InnerException);
            throw new Exception($"Exception compiling namespace '{identifier}': {originalException.Message ?? originalException.Message}; StackTrace: {FormatStackTrace(originalException)}", originalException);
        }
    }
}
