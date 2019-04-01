using Codecov.Services.VersionControl;
using System;
using System.Linq;

namespace Codecov.Services
{
    internal class YamlService : IYamlService
    {
        private readonly string _yamlPath;
        private readonly ISourceService _sourceService;
        private readonly Lazy<string> _fileName;

        public YamlService(string yamlPath, ISourceService sourceService)
        {
            _yamlPath = yamlPath;
            _sourceService = sourceService ?? throw new ArgumentNullException(nameof(sourceService));
            _fileName = new Lazy<string>(LoadFileName);
        }

        public string FileName => _fileName.Value;

        private string LoadFileName()
        {
            if (!string.IsNullOrWhiteSpace(_yamlPath))
            {
                return _yamlPath;
            }

            string codecovYamlFullPath = _sourceService?.Files?.FirstOrDefault(file =>
            {
                string fileName = file.ToLower();
                return fileName.EndsWith(@"\.codecov.yaml") ||
                    fileName.EndsWith(@"\codecov.yaml") ||
                    fileName.EndsWith(@"\.codecov.yml") ||
                    fileName.EndsWith(@"\codecov.yml");
            });

            if (codecovYamlFullPath == null)
            {
                return null;
            }

            string[] codecovYamlFullPathSplit = codecovYamlFullPath.Split('\\');
            return codecovYamlFullPathSplit.Length == 0 ? string.Empty : codecovYamlFullPathSplit[codecovYamlFullPathSplit.Length - 1];
        }
    }
}
