﻿using System;
using Codecov.Utilities;

namespace Codecov.Services.ContinuousIntegrationServers
{
    internal class AppVeyor : ContinuousIntegrationServer
    {
        private readonly Lazy<string> _branch = new Lazy<string>(() => EnviornmentVariable.GetEnvironmentVariable("APPVEYOR_REPO_BRANCH"));
        private readonly Lazy<string> _build = new Lazy<string>(LoadBuild);
        private readonly Lazy<string> _commit = new Lazy<string>(() => EnviornmentVariable.GetEnvironmentVariable("APPVEYOR_REPO_COMMIT"));
        private readonly Lazy<bool> _detecter = new Lazy<bool>(LoadDetecter);
        private readonly Lazy<string> _job = new Lazy<string>(LoadJob);
        private readonly Lazy<string> _pr = new Lazy<string>(() => EnviornmentVariable.GetEnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER"));
        private readonly Lazy<string> _slug = new Lazy<string>(() => EnviornmentVariable.GetEnvironmentVariable("APPVEYOR_REPO_NAME"));

        public override string Branch => _branch.Value;

        public override string Build => _build.Value;

        public override string Commit => _commit.Value;

        public override bool Detecter => _detecter.Value;

        public override string Job => _job.Value;

        public override string Pr => _pr.Value;

        public override string Service => "appveyor";

        public override string Slug => _slug.Value;

        private static string LoadBuild()
        {
            var build = EnviornmentVariable.GetEnvironmentVariable("APPVEYOR_JOB_ID");
            return !string.IsNullOrWhiteSpace(build) ? Uri.EscapeDataString(build) : string.Empty;
        }

        private static bool LoadDetecter()
        {
            var appVeyor = EnviornmentVariable.GetEnvironmentVariable("APPVEYOR");
            var ci = EnviornmentVariable.GetEnvironmentVariable("CI");

            if (string.IsNullOrWhiteSpace(appVeyor) || string.IsNullOrWhiteSpace(ci))
            {
                return false;
            }

            return appVeyor.Equals("True") && ci.Equals("True");
        }

        private static string LoadJob()
        {
            var accountName = EnviornmentVariable.GetEnvironmentVariable("APPVEYOR_ACCOUNT_NAME");
            var slug = EnviornmentVariable.GetEnvironmentVariable("APPVEYOR_PROJECT_SLUG");
            var version = EnviornmentVariable.GetEnvironmentVariable("APPVEYOR_BUILD_VERSION");

            if (string.IsNullOrWhiteSpace(accountName) || string.IsNullOrWhiteSpace(slug) || string.IsNullOrWhiteSpace(version))
            {
                return string.Empty;
            }

            var job = $"{accountName}/{slug}/{version}";

            return job;
        }
    }
}
