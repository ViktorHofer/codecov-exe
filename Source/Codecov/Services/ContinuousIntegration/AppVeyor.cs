using System;

namespace Codecov.Services.ContinuousIntegration
{
    internal class AppVeyor : ContinuousIntegrationService
    {
        public override string Branch => Environment.GetEnvironmentVariable("APPVEYOR_REPO_BRANCH");

        public override string Build => LoadBuild();

        public override string Commit => Environment.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT");

        public override bool Exists => LoadDetecter();

        public override string Job => LoadJob();

        public override string PR => Environment.GetEnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER");

        public override string Service => "appveyor";

        public override string Slug => Environment.GetEnvironmentVariable("APPVEYOR_REPO_NAME");

        public override void Activate()
        {
            Log.Information("AppVeyor detected.");
        }

        private static string LoadBuild()
        {
            string build = Environment.GetEnvironmentVariable("APPVEYOR_JOB_ID");
            return !string.IsNullOrWhiteSpace(build) ? Uri.EscapeDataString(build) : null;
        }

        private static bool LoadDetecter()
        {
            string appveyorEnv = Environment.GetEnvironmentVariable("APPVEYOR")?.ToLowerInvariant();
            string ciEnv = Environment.GetEnvironmentVariable("CI")?.ToLowerInvariant();

            return appveyorEnv == "true" && ciEnv == "true";
        }

        private static string LoadJob()
        {
            string accountName = Environment.GetEnvironmentVariable("APPVEYOR_ACCOUNT_NAME");
            string slug = Environment.GetEnvironmentVariable("APPVEYOR_PROJECT_SLUG");
            string version = Environment.GetEnvironmentVariable("APPVEYOR_BUILD_VERSION");

            if (string.IsNullOrWhiteSpace(accountName) || string.IsNullOrWhiteSpace(slug) || string.IsNullOrWhiteSpace(version))
            {
                return null;
            }

            return $"{accountName}/{slug}/{version}";
        }
    }
}
