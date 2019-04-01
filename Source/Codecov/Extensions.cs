using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Codecov
{
    internal static class Extensions
    {
        public static string RemoveAllWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str) ? string.Empty : Regex.Replace(str.Trim(), @"\s+", string.Empty);

        public static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            var absolutePath = Path.GetFullPath(path);

            return !string.IsNullOrWhiteSpace(absolutePath) ? Path.GetFullPath(new Uri(absolutePath).LocalPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) : string.Empty;
        }

        public static string Version
        {
            get
            {
                var assemblyVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
                return $"exe-{assemblyVersion}";
            }
        }
    }
}
