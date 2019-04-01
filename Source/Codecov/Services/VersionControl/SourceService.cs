using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Codecov.Services.VersionControl
{
    internal class SourceService : ISourceService
    {
        private readonly IVersionControlService _versionControlService;
        private Lazy<IEnumerable<string>> _files;
        private Lazy<IEnumerable<string>> _filteredFiles;

        public SourceService(IVersionControlService versionControlService)
        {
            _versionControlService = versionControlService;
            _files = new Lazy<IEnumerable<string>>(() => _versionControlService.SourceCode?.Select(Extensions.NormalizePath));
            _filteredFiles = new Lazy<IEnumerable<string>>(() => Files != null ? GetFilteredFiles() : null);
        }

        public string Directory => _versionControlService.RepoRoot;

        public IEnumerable<string> Files => _files.Value;

        public IEnumerable<string> FilteredFiles => _filteredFiles.Value;

        private IEnumerable<string> GetFilteredFiles()
        {
            return Files
                .Where(file => !file.ToLower().Contains(@"\.git"))
                .Where(file => !file.ToLower().Contains(@"\bin\debug"))
                .Where(file => !file.ToLower().Contains(@"\bin\release"))
                .Where(file => !file.ToLower().Contains(@"\obj\debug"))
                .Where(file => !file.ToLower().Contains(@"\obj\release"))
                .Where(file => !file.ToLower().Contains(@"\.vscode"))
                .Where(file => !file.ToLower().Contains(@"\.vs"))
                .Where(file => !file.ToLower().Contains(@"\obj\project.assets.json"))
                .Where(file => !file.ToLower().EndsWith(".csproj.nuget.g.targets"))
                .Where(file => !file.ToLower().EndsWith(".csproj.nuget.g.props"))
                .Where(file => !Path.GetExtension(file).Equals(".csproj.nuget.g.props", StringComparison.OrdinalIgnoreCase))
                .Where(file => !Path.GetExtension(file).Equals(".dll", StringComparison.OrdinalIgnoreCase))
                .Where(file => !Path.GetExtension(file).Equals(".exe", StringComparison.OrdinalIgnoreCase))
                .Where(file => !Path.GetExtension(file).Equals(".gif", StringComparison.OrdinalIgnoreCase))
                .Where(file => !Path.GetExtension(file).Equals(".jpg", StringComparison.OrdinalIgnoreCase))
                .Where(file => !Path.GetExtension(file).Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
                .Where(file => !Path.GetExtension(file).Equals(".md", StringComparison.OrdinalIgnoreCase))
                .Where(file => !Path.GetExtension(file).Equals(".png", StringComparison.OrdinalIgnoreCase))
                .Where(file => !Path.GetExtension(file).Equals(".psd", StringComparison.OrdinalIgnoreCase))
                .Where(file => !Path.GetExtension(file).Equals(".ptt", StringComparison.OrdinalIgnoreCase))
                .Where(file => !Path.GetExtension(file).Equals(".pptx", StringComparison.OrdinalIgnoreCase))
                .Where(file => !Path.GetExtension(file).Equals(".numbers", StringComparison.OrdinalIgnoreCase))
                .Where(file => !Path.GetExtension(file).Equals(".pages", StringComparison.OrdinalIgnoreCase))
                .Where(file => !Path.GetExtension(file).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                .Where(file => !Path.GetExtension(file).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                .Where(file => !Path.GetExtension(file).Equals(".docx", StringComparison.OrdinalIgnoreCase))
                .Where(file => !Path.GetExtension(file).Equals(".doc", StringComparison.OrdinalIgnoreCase))
                .Where(file => !Path.GetExtension(file).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                .Where(file => !Path.GetExtension(file).Equals(".yml", StringComparison.OrdinalIgnoreCase))
                .Where(file => !Path.GetExtension(file).Equals(".yaml", StringComparison.OrdinalIgnoreCase))
                .Where(file => !Path.GetExtension(file).Equals(".gitignore", StringComparison.OrdinalIgnoreCase))
                .Where(file => !file.ToLower().Contains(@"\virtualenv"))
                .Where(file => !file.ToLower().Contains(@"\.virtualenv"))
                .Where(file => !file.ToLower().Contains(@"\virtualenvs"))
                .Where(file => !file.ToLower().Contains(@"\.virtualenvs"))
                .Where(file => !file.ToLower().Contains(@"\env"))
                .Where(file => !file.ToLower().Contains(@"\.env"))
                .Where(file => !file.ToLower().Contains(@"\envs"))
                .Where(file => !file.ToLower().Contains(@"\.envs"))
                .Where(file => !file.ToLower().Contains(@"\venv"))
                .Where(file => !file.ToLower().Contains(@"\.venv"))
                .Where(file => !file.ToLower().Contains(@"\venvs"))
                .Where(file => !file.ToLower().Contains(@"\.venvs"))
                .Where(file => !file.ToLower().Contains(@"\build\lib"))
                .Where(file => !file.ToLower().Contains(@"\.egg-info"))
                .Where(file => !file.ToLower().Contains(@"\shunit2-2.1.6"))
                .Where(file => !file.ToLower().Contains(@"\vendor"))
                .Where(file => !file.ToLower().Contains(@"\js\generated\coverage"))
                .Where(file => !file.ToLower().Contains(@"\__pycache__"))
                .Where(file => !file.ToLower().Contains(@"\__pycache__"))
                .Where(file => !file.ToLower().Contains(@"\node_modules"));
        }
    }
}
