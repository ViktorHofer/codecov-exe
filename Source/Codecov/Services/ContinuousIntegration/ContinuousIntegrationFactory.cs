using System.Linq;

namespace Codecov.Services.ContinuousIntegration
{
    internal static class ContinuousIntegrationFactory
    {
        public static ContinuousIntegrationService Create()
        {
            var contiuousIntegrationService = new ContinuousIntegrationService[] { new AppVeyor(), new TeamCity() };
            var buildServer = contiuousIntegrationService.FirstOrDefault(ci => ci.Exists);

            return buildServer ?? new ContinuousIntegrationService();
        }
    }
}
