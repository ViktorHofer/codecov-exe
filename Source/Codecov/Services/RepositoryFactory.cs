using System.Collections.Generic;
using Codecov.Services.ContinuousIntegration;
using Codecov.Services.VersionControl;

namespace Codecov.Services
{
    internal static class RepositoryFactory
    {
        public static IEnumerable<IRepositoryService> Create(IVersionControlService versionControlService, ContinuousIntegrationService continuousIntegrationService)
        {
            if (continuousIntegrationService.Exists)
            {
                yield return continuousIntegrationService;
            }

            yield return versionControlService;
        }
    }
}
