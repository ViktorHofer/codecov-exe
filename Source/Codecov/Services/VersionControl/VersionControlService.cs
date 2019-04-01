using System;
using System.Collections.Generic;
using System.IO;

namespace Codecov.Services.VersionControl
{
    internal class VersionControlService : IVersionControlService
    {
        private readonly string _repoRoot;

        public VersionControlService(string repoRoot)
        {
            _repoRoot = repoRoot;
        }

        public virtual string Branch { get; } = Environment.GetEnvironmentVariable("VCS_BRANCH_NAME");

        public virtual string Commit { get; } = Environment.GetEnvironmentVariable("VCS_COMMIT_ID");

        public string PR { get; } = Environment.GetEnvironmentVariable("VCS_PULL_REQUEST");

        public virtual string RepoRoot => !_repoRoot.RemoveAllWhiteSpace().Equals(string.Empty) ? Extensions.NormalizePath(_repoRoot) : Extensions.NormalizePath(".");

        public virtual string Slug { get; } = Environment.GetEnvironmentVariable("VCS_SLUG");

        public virtual IEnumerable<string> SourceCode => Directory.EnumerateFiles(RepoRoot, "*.*", SearchOption.AllDirectories);

        public virtual string Tag { get; } = Environment.GetEnvironmentVariable("VCS_TAG");

        public virtual bool Exists { get; } = false;

        public virtual void Activate()
        {
            Log.Warning("No Version Control System detected.");
        }
    }
}
