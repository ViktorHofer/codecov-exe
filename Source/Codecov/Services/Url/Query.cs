using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Codecov.Services.ContinuousIntegration;

namespace Codecov.Services.Url
{
    internal class Query : IQuery
    {
        private readonly IQueryOptions _options;
        private readonly ContinuousIntegrationService _continuousIntegrationService;
        private readonly IEnumerable<IRepositoryService> _repositoryServices;
        private readonly IYamlService _yamlService;
        private readonly Dictionary<string, string> _queryParameters = new Dictionary<string, string>();
        private readonly Lazy<string> _value;

#pragma warning disable SA1305 // Field names should not use Hungarian notation
        public Query(IQueryOptions options, IEnumerable<IRepositoryService> repositoryServices, ContinuousIntegrationService continuousIntegrationService, IYamlService yamlService)
#pragma warning restore SA1305 // Field names should not use Hungarian notation
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _repositoryServices = repositoryServices ?? throw new ArgumentNullException(nameof(_repositoryServices));
            _continuousIntegrationService = continuousIntegrationService ?? throw new ArgumentNullException(nameof(continuousIntegrationService));
            _yamlService = yamlService ?? throw new ArgumentNullException(nameof(yamlService));

            _value = new Lazy<string>(() =>
            {
                SetBranch();
                SetCommit();
                SetBuild();
                SetTag();
                SetPr();
                SetName();
                SetFlags();
                SetSlug();
                SetToken();
                SetPackage();
                SetBuildUrl();
                SetYaml();
                SetJob();
                SetService();

                return string.Join("&", _queryParameters.Select(x => $"{x.Key}={x.Value}"));
            });
        }

        public string Value => _value.Value;

        private static string EscapeKnownProblematicCharacters(string data)
        {
            var knownChars = new Dictionary<char, string>
            {
                { '#', "%23" },
            };

            var result = new StringBuilder();

            foreach (var c in data)
            {
                if (knownChars.ContainsKey(c))
                {
                    result.Append(knownChars[c]);
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        private bool OverrideIfNotEmptyOrNull(string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _queryParameters[key] = value.RemoveAllWhiteSpace();
                return true;
            }

            return false;
        }

        private void SetBranch()
        {
            if (OverrideIfNotEmptyOrNull("branch", _options.Branch)) return;

            foreach (var repository in _repositoryServices)
            {
                if (!string.IsNullOrWhiteSpace(repository.Branch))
                {
                    _queryParameters["branch"] = EscapeKnownProblematicCharacters(repository.Branch);
                    break;
                }
            }
        }

        private void SetBuild()
        {
            string build = string.IsNullOrWhiteSpace(_options.Build) ? _continuousIntegrationService.Build : _options.Build;
            OverrideIfNotEmptyOrNull("build", build);
        }

        private void SetBuildUrl()
        {
            OverrideIfNotEmptyOrNull("build_url", _continuousIntegrationService.BuildUrl);
        }

        private void SetCommit()
        {
            if (OverrideIfNotEmptyOrNull("commit", _options.Commit)) return;

            foreach (var repository in _repositoryServices)
            {
                if (!string.IsNullOrWhiteSpace(repository.Commit))
                {
                    _queryParameters["commit"] = repository.Commit;
                    break;
                }
            }
        }

        private void SetFlags()
        {
            if (string.IsNullOrWhiteSpace(_options.Flags))
            {
                return;
            }

            var flagsSeperatedByCommasAndNoExtraWhiteSpace = string.Join(",", _options.Flags.Split(',').Select(x => x.Trim()));
            _queryParameters["flags"] = flagsSeperatedByCommasAndNoExtraWhiteSpace;
        }

        private void SetJob()
        {
            if (_continuousIntegrationService.Job == null)
            {
                return;
            }

            var escapedJob = Uri.EscapeDataString(_continuousIntegrationService.Job);

            // Due to the + sign being escaped, we need to unescape that character
            escapedJob = escapedJob.Replace("%2B", "%252B");

            _queryParameters["job"] = escapedJob;
        }

        private void SetName()
        {
            OverrideIfNotEmptyOrNull("name", _options.Name);
        }

        private void SetPackage()
        {
            _queryParameters["package"] = Extensions.Version;
        }

        private void SetPr()
        {
            if (OverrideIfNotEmptyOrNull("pr", _options.PR)) return;

            foreach (var repository in _repositoryServices)
            {
                if (!string.IsNullOrWhiteSpace(repository.PR))
                {
                    _queryParameters["pr"] = repository.PR;
                    break;
                }
            }
        }

        private void SetService()
        {
            OverrideIfNotEmptyOrNull("service", _continuousIntegrationService.Service);
        }

        private void SetSlug()
        {
            if (!string.IsNullOrWhiteSpace(_options.Slug))
            {
                _queryParameters["slug"] = WebUtility.UrlEncode(_options.Slug.Trim());
                return;
            }

            string slugEnv = Environment.GetEnvironmentVariable("CODECOV_SLUG");
            if (!string.IsNullOrWhiteSpace(slugEnv))
            {
                _queryParameters["slug"] = WebUtility.UrlEncode(slugEnv.Trim());
                return;
            }

            foreach (IRepositoryService repository in _repositoryServices)
            {
                if (!string.IsNullOrWhiteSpace(repository.Slug))
                {
                    _queryParameters["slug"] = WebUtility.UrlEncode(repository.Slug);
                    break;
                }
            }
        }

        private void SetTag()
        {
            if (OverrideIfNotEmptyOrNull("tag", _options.Tag)) return;

            foreach (var repository in _repositoryServices)
            {
                if (!string.IsNullOrWhiteSpace(repository.Tag))
                {
                    _queryParameters["tag"] = repository.Tag;
                    break;
                }
            }
        }

        private void SetToken()
        {
            if (!string.IsNullOrWhiteSpace(_options.Token) && Guid.TryParse(_options.Token, out Guid _))
            {
                _queryParameters["token"] = _options.Token.RemoveAllWhiteSpace();
                return;
            }

            string tokenEnv = Environment.GetEnvironmentVariable("CODECOV_TOKEN");
            if (!string.IsNullOrWhiteSpace(tokenEnv))
            {
                _queryParameters["token"] = tokenEnv.RemoveAllWhiteSpace();
            }
        }

        private void SetYaml()
        {
            OverrideIfNotEmptyOrNull("yaml", _yamlService.FileName);
        }
    }
}
