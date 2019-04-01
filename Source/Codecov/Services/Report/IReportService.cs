using System.Collections.Generic;

namespace Codecov.Services
{
    internal interface IReportService
    {
        string AggregatedContent { get; }

        IEnumerable<ReportFile> GetReports();
    }
}
