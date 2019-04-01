using System;
using System.Collections.Generic;

namespace Codecov.Services.ContinuousIntegration
{
    internal class ContinuousIntegrationService : IRepositoryService
    {
        public virtual string Branch { get; }

        public virtual string Build => Environment.GetEnvironmentVariable("CI_BUILD_ID");

        public virtual string BuildUrl => Environment.GetEnvironmentVariable("CI_BUILD_URL");

        public virtual string Commit { get; }

        public virtual IDictionary<string, string> EnvironmentVariables { get; } = new Dictionary<string, string>();

        public virtual bool Exists { get; } = false;

        public virtual string Job => Environment.GetEnvironmentVariable("CI_JOB_ID");

        public virtual string PR { get; }

        public virtual string Service { get; }

        public virtual string Slug { get; }

        public virtual string Tag { get; }

        public virtual void Activate()
        {
            Log.Warning("No CI detected.");
        }
    }
}
