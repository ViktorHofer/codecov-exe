using System;

namespace Codecov.Services.Url
{
    internal class UrlService : IUrlService
    {
        private readonly IHost _host;
        private readonly IQuery _query;
        private readonly IRoute _route;

        public UrlService(IHost host, IRoute route, IQuery query)
        {
            _host = host;
            _route = route;
            _query = query;
        }

        public Uri GetUrl(ApiVersion apiVersion) => new Uri($"{_host.Value}/{_route.GetRoute(apiVersion)}?{_query.Value}");
    }
}
