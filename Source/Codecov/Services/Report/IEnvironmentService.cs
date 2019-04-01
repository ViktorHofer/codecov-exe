using System.Collections.Generic;

namespace Codecov.Services.Report
{
    internal interface IEnvironmentService
    {
        IDictionary<string, string> Variables { get; }
    }
}
