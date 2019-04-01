namespace Codecov.Services.Url
{
    internal interface IRoute
    {
        string GetRoute(ApiVersion version);
    }
}
