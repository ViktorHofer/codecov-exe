using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Codecov.Services.VersionControl
{
    internal class Git : VersionControlService
    {
        private readonly string _repoRoot;
        private readonly ITerminalService _terminalService;

        public Git(string repoRoot, ITerminalService terminalService)
            : base(repoRoot)
        {
            _repoRoot = repoRoot;
            _terminalService = terminalService;
        }

        public override string Branch => LoadBranch();

        public override string Commit => LoadCommit();

        public override string RepoRoot => LoadRepoRoot();

        public override string Slug => LoadSlug();

        public override IEnumerable<string> SourceCode => LoadSourceCode();

        public override bool Exists => LoadDetecter();

        public override void Activate()
        {
            Log.Information($"GIT detected.");
        }

        private string LoadBranch()
        {
            string branch = RunGit(@"rev-parse --abbrev-ref HEAD");
            if (string.IsNullOrWhiteSpace(branch) || branch.Equals("HEAD"))
            {
                return null;
            }

            return branch;
        }

        private string LoadCommit()
        {
            string commit = RunGit(@"rev-parse HEAD");
            return !string.IsNullOrWhiteSpace(commit) ? commit : null;
        }

        private bool LoadDetecter()
        {
            return !string.IsNullOrWhiteSpace(_terminalService.Run("git", "--version")) && Directory.Exists($"{RepoRoot}/.git");
        }

        private string LoadRepoRoot()
        {
            if (!_repoRoot.RemoveAllWhiteSpace().Equals(string.Empty))
            {
                return Extensions.NormalizePath(_repoRoot);
            }

            string root = _terminalService.Run("git", "rev-parse --show-toplevel");
            return string.IsNullOrWhiteSpace(root) ? Extensions.NormalizePath(".") : Extensions.NormalizePath(root);
        }

        private string LoadSlug()
        {
            string remote = RunGit("config --get remote.origin.url");

            if (string.IsNullOrWhiteSpace(remote))
            {
                return null;
            }

            var splitColon = remote.Split(':');
            if (splitColon.Length <= 1)
            {
                return null;
            }

            var splitSlash = splitColon[1].Split('/');
            if (splitSlash.Length <= 1)
            {
                return null;
            }

            return splitSlash[splitSlash.Length - 2] + "/" + splitSlash[splitSlash.Length - 1].TrimEnd('t').TrimEnd('i').TrimEnd('g').TrimEnd('.');
        }

        private IEnumerable<string> LoadSourceCode()
        {
            string sourceCode = RunGit("ls-tree --full-tree -r HEAD --name-only");
            return string.IsNullOrWhiteSpace(sourceCode) ? null : sourceCode.Trim('\n').Split('\n').Select(Extensions.NormalizePath);
        }

        private string RunGit(string commandArguments) =>
            _terminalService.Run("git", $@"-C ""{RepoRoot}"" {commandArguments}");
    }
}
