﻿using System.Collections.Generic;
using System.Linq;

namespace codecov.Services.Utils
{
    public class Url : IUrl
    {
        protected IDictionary<string, string> QueryParameters { get; } =
            new Dictionary<string, string>
            {
                { "branch", string.Empty},
                { "commit", string.Empty},
                { "build", string.Empty},
                { "build_url", string.Empty},
                { "tag", string.Empty},
                { "slug", string.Empty},
                { "yaml", string.Empty},
                { "service", string.Empty},
                { "flags", string.Empty},
                { "pr", string.Empty},
                { "job", string.Empty}
            };

        public string Query(Options options)
        {
            var query = string.Join("&", QueryParameters.Select(x => $"{x.Key}={x.Value ?? string.Empty}"));
            return $"package=exe-beta&token={options.Token}&{query}";
        }
    }
}