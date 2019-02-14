using System;

namespace Codecov.Utilities
{
    internal static class EnviornmentVariable
    {
        internal static string GetEnvironmentVariable(string name)
        {
            var env = Environment.GetEnvironmentVariable(name);
            return string.IsNullOrWhiteSpace(env) ? string.Empty : env;
        }
    }
}
