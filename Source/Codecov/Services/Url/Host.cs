using System;

namespace Codecov.Services.Url
{
    internal class Host : IHost
    {
        private readonly string _url;

        public Host(string url)
        {
            _url = url;
        }

        public string Value
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_url))
                {
                    return _url.Trim().TrimEnd('/');
                }

                // Try to get it from enviornment variable else just use default url.
                var urlEnv = Environment.GetEnvironmentVariable("CODECOV_URL");
                return !string.IsNullOrWhiteSpace(urlEnv) ? urlEnv.Trim().TrimEnd('/') : "https://codecov.io";
            }
        }
    }
}
