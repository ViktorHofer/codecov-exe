using System.Collections.Generic;

namespace Codecov.Services.VersionControl
{
    internal interface ISourceService
    {
        string Directory { get; }

        IEnumerable<string> Files { get; }

        IEnumerable<string> FilteredFiles { get; }
    }
}
