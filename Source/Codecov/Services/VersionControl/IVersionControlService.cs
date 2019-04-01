using System.Collections.Generic;

namespace Codecov.Services.VersionControl
{
    internal interface IVersionControlService : IRepositoryService
    {
        string RepoRoot { get; }

        IEnumerable<string> SourceCode { get; }

        bool Exists { get; }

        void Activate();
    }
}
