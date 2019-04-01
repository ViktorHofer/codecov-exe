using System;

namespace Codecov.Services.Url
{
    internal interface IUrlService
    {
        Uri GetUrl(ApiVersion apiVersion);
    }
}
