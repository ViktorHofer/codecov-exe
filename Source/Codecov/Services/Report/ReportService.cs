using Codecov.Services.Report;
using Codecov.Services.VersionControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Codecov.Services
{
    internal class ReportService : IReportService
    {
        private readonly bool _disableNetwork;
        private readonly IEnvironmentService _environmentService;
        private readonly ISourceService _sourceService;
        private readonly IEnumerable<string> _reportFiles;
        private readonly Lazy<IEnumerable<ReportFile>> _reports;
        private readonly Lazy<string> _aggregatedContent;

        public ReportService(bool disableNetwork, IEnvironmentService environmentService, ISourceService sourceService, IEnumerable<string> reportFiles)
        {
            _disableNetwork = disableNetwork;
            _environmentService = environmentService;
            _sourceService = sourceService;
            _reportFiles = reportFiles;

            _reports = new Lazy<IEnumerable<ReportFile>>(GetReportsImpl);
            _aggregatedContent = new Lazy<string>(() => $"{GetEnvironmentVariables()}{GetNetwork()}{GetCombinedCoverage()}");
        }

        public string AggregatedContent => _aggregatedContent.Value;

        public IEnumerable<ReportFile> GetReports() => _reports.Value;

        private IEnumerable<ReportFile> GetReportsImpl()
        {
            IEnumerable<ReportFile> reports = _reportFiles?.Where(path =>
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    Log.Warning("Invalid report path.");
                    return false;
                }

                if (!File.Exists(path))
                {
                    Log.Warning($"The file {path} does not exist.");
                    return false;
                }

                return true;
            })?.Select(x => new ReportFile(x, File.ReadAllText(x)))
            ?.ToArray();

            if (reports == null || !reports.Any())
            {
                throw new Exception("No Report detected.");
            }

            return reports;
        }

        private string GetEnvironmentVariables()
        {
            return !_environmentService.Variables.Any() ? string.Empty : $"{string.Join("\n", _environmentService.Variables.Select(x => x.Key.Trim() + "=" + x.Value.Trim()).ToArray())}\n<<<<<< ENV\n";
        }

        private string GetCombinedCoverage()
        {
            return GetReports().Aggregate(string.Empty, (current, coverage) => current + $"# path={coverage.File}\n{coverage.Content}\n<<<<<< EOF\n");
        }

        private string GetNetwork()
        {
            if (_disableNetwork)
            {
                return string.Empty;
            }

            const string network = "<<<<<< network\n";
            IEnumerable<string> sourceCode = _sourceService?.FilteredFiles?.ToArray();
            if (sourceCode == null || !sourceCode.Any())
            {
                return network;
            }

            return $"{sourceCode.Aggregate(string.Empty, (current, source) => current + $"{ConvertToRelativePath(source)}\n")}{network}";
        }

        private string ConvertToRelativePath(string absolutePath) =>
            absolutePath.Replace($@"{_sourceService.Directory}\", string.Empty);
    }
}
