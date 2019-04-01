using System;
using System.Collections.Generic;
using System.Linq;
using Codecov.Services.ContinuousIntegration;

namespace Codecov.Services.Report
{
    internal class EnvironmentService : IEnvironmentService
    {
        private readonly Lazy<IDictionary<string, string>> _variables;
        private readonly IEnumerable<string> _environmentVariableNames;
        private readonly ContinuousIntegrationService _continuousIntegrationService;

        public EnvironmentService(IEnumerable<string> environmentVariableNames, ContinuousIntegrationService continuousIntegrationService)
        {
            _environmentVariableNames = environmentVariableNames ?? throw new ArgumentNullException(nameof(environmentVariableNames));
            _continuousIntegrationService = continuousIntegrationService ?? throw new ArgumentNullException(nameof(continuousIntegrationService));

            _variables = new Lazy<IDictionary<string, string>>(GetVariables);
        }

        public IDictionary<string, string> Variables => _variables.Value;

        private IDictionary<string, string> GetVariables()
        {
            var environmentVariables = new Dictionary<string, string>(_continuousIntegrationService.EnvironmentVariables);

            List<string> environmentVariableNames = _environmentVariableNames.ToList();
            environmentVariableNames.Add("CODECOV_ENV");

            foreach (var environmentVariableName in environmentVariableNames)
            {
                if (environmentVariables.ContainsKey(environmentVariableName))
                {
                    continue;
                }

                var value = Environment.GetEnvironmentVariable(environmentVariableName);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    environmentVariables[environmentVariableName] = value;
                }
            }

            return environmentVariables;
        }
    }
}
