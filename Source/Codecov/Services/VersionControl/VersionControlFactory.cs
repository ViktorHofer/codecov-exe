using System.Linq;

namespace Codecov.Services.VersionControl
{
    internal static class VersionControlFactory
    {
        public static IVersionControlService Create(string repoRoot, ITerminalService terminal)
        {
            var versionControlSystems = new IVersionControlService[] { new Git(repoRoot, terminal) };
            var vcs = versionControlSystems.FirstOrDefault(x => x.Exists);

            return vcs ?? new VersionControlService(repoRoot);
        }
    }
}
