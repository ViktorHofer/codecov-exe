using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codecov.Services;
using Codecov.Services.ContinuousIntegration;
using Codecov.Services.Report;
using Codecov.Services.Url;
using Codecov.Services.VersionControl;
using CommandLine;

namespace Codecov
{
    /// <summary>
    /// Main entry point for program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main entry point for program.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>0 if successfull and 1 otherwise.</returns>
        internal static async Task<int> Main(string[] args)
        {
            int failureErrorCode = 0;

            try
            {
                CommandLineOptions options = ((Parsed<CommandLineOptions>)Parser.Default.ParseArguments<CommandLineOptions>(args)).Value;
                failureErrorCode = options.Required ? 1 : 0;
                Log.Create(options.Verbose, options.NoColor);
                if (!options.NoLogo)
                {
                    PrintLogo();
                }
                await Work(options);
            }
            catch (Exception e)
            {
                Log.Fatal($"{e.Message}\n{e.StackTrace}");
                return failureErrorCode;
            }

            return 0;
        }

        private static void PrintLogo()
        {
            Console.WriteLine($@"
              _____          _
             / ____|        | |
            | |     ___   __| | ___  ___ _____   __
            | |    / _ \ / _  |/ _ \/ __/ _ \ \ / /
            | |___| (_) | (_| |  __/ (_| (_) \ V /
             \_____\___/ \____|\___|\___\___/ \_/
                                         {Extensions.Version}
            ");
        }

        private static async Task Work(CommandLineOptions options)
        {
            // Create repository services
            var continuousIntegrationService = ContinuousIntegrationFactory.Create();
            continuousIntegrationService.Activate();
            var versionControlService = VersionControlFactory.Create(options.RepoRoot, new TerminalService());
            versionControlService.Activate();
            IEnumerable<IRepositoryService> repositoryServices = RepositoryFactory.Create(versionControlService, continuousIntegrationService);

            // Create report service
            var environmentService = new EnvironmentService(options.Values, continuousIntegrationService);
            var sourceService = new SourceService(versionControlService);
            var reportService = new ReportService(options.DisableNetwork, environmentService, sourceService, options.Files);

            // Create query service
            var yamlService = new YamlService(options.YamlPath, sourceService);
            var queryOptions = new QueryOptions(options.Branch, options.Build, options.Commit, options.Flags, options.Name, options.PR, options.Slug, options.Tag, options.Token);
            var urlService = new UrlService(new Host(options.Url), new Route(), new Query(queryOptions, repositoryServices, continuousIntegrationService, yamlService));

            Log.Information($"Project root: {versionControlService.RepoRoot}");
            if (string.IsNullOrWhiteSpace(yamlService.FileName))
            {
                Log.Information("Yaml not found, that's ok! Learn more at http://docs.codecov.io/docs/codecov-yaml");
            }

            Log.Information("Reading reports.");
            Log.Information(string.Join("\n", reportService.GetReports().Select(x => x.File)));

            if (environmentService.Variables.Any())
            {
                Log.Information("Appending build variables");
                Log.Information(string.Join("\n", environmentService.Variables.Select(x => x.Key.Trim()).ToArray()));
            }

            ApiVersion apiVersion = options.DisableS3 ? ApiVersion.V2 : ApiVersion.V4;

            if (options.Dump)
            {
                Log.Warning("Skipping upload and dumping contents.");
                Log.Information($"url: {urlService.GetUrl(apiVersion)}");
                Log.Information(reportService.AggregatedContent);
            }

            // Create upload service
            var uploadService = new UploadService(urlService, reportService, apiVersion);
            await uploadService.Upload();
        }
    }
}
