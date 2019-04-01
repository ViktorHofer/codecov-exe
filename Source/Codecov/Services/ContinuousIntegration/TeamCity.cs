using System;

namespace Codecov.Services.ContinuousIntegration
{
    internal class TeamCity : ContinuousIntegrationService
    {
        public override string Branch => Environment.GetEnvironmentVariable("TEAMCITY_BUILD_BRANCH");

        public override string Build => Environment.GetEnvironmentVariable("TEAMCITY_BUILD_ID");

        public override string BuildUrl => LoadBuildUrl();

        public override string Commit => LoadCommit();

        public override bool Exists => LoadDetecter();

        public override string Service => "teamcity";

        public override string Slug => LoadSlug();

        public override void Activate()
        {
            Log.Information("TeamCity detected.");
            if (string.IsNullOrWhiteSpace(Branch))
            {
                Log.Warning("Teamcity does not automatically make build parameters available as environment variables.\nAdd the following environment parameters to the build configuration.\nenv.TEAMCITY_BUILD_BRANCH = %teamcity.build.branch%.\nenv.TEAMCITY_BUILD_ID = %teamcity.build.id%.\nenv.TEAMCITY_BUILD_URL = %teamcity.serverUrl%/viewLog.html?buildId=%teamcity.build.id%.\nenv.TEAMCITY_BUILD_COMMIT = %system.build.vcs.number%.\nenv.TEAMCITY_BUILD_REPOSITORY = %vcsroot.<YOUR TEAMCITY VCS NAME>.url%.");
            }
        }

        private static string LoadBuildUrl()
        {
            string buildUrl = Environment.GetEnvironmentVariable("TEAMCITY_BUILD_URL");
            return !string.IsNullOrWhiteSpace(buildUrl) ? Uri.EscapeDataString(buildUrl) : null;
        }

        private static string LoadCommit()
        {
            string commit = Environment.GetEnvironmentVariable("TEAMCITY_BUILD_COMMIT");
            if (!string.IsNullOrWhiteSpace(commit))
            {
                return commit;
            }

            return Environment.GetEnvironmentVariable("BUILD_VCS_NUMBER");
        }

        private static bool LoadDetecter()
        {
            string teamCity = Environment.GetEnvironmentVariable("TEAMCITY_VERSION");
            return !string.IsNullOrWhiteSpace(teamCity);
        }

        private static string LoadSlug()
        {
            var buildRepository = Environment.GetEnvironmentVariable("TEAMCITY_BUILD_REPOSITORY");
            if (string.IsNullOrWhiteSpace(buildRepository))
            {
                return null;
            }

            var temp = buildRepository.Split(':');
            if (temp.Length > 0)
            {
                temp[0] = string.Empty;
            }

            buildRepository = string.Join(string.Empty, temp);

            var splitBuildRepository = buildRepository.Split('/');
            if (splitBuildRepository.Length > 1)
            {
                var repo = splitBuildRepository[splitBuildRepository.Length - 1].Replace(".git", string.Empty);
                var owner = splitBuildRepository[splitBuildRepository.Length - 2];
                return $"{owner}/{repo}";
            }

            return null;
        }
    }
}
