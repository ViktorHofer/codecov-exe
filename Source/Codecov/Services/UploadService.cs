using Codecov.Services.Url;
using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Codecov.Services
{
    internal class UploadService
    {
        private readonly IUrlService _urlService;
        private readonly IReportService _reportService;
        private readonly ApiVersion _apiVersion;
        private readonly HttpClient _httpClient;

        public UploadService(IUrlService urlService, IReportService reportService, ApiVersion apiVersion)
        {
            _urlService = urlService;
            _reportService = reportService;
            _apiVersion = apiVersion;

            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }
            _httpClient = new HttpClient(handler);
        }

        public async Task Upload()
        {
            Uri postUrl = _urlService.GetUrl(_apiVersion);
            string displayUrl = Regex.Replace(postUrl.ToString(), @"token=\w{8}-\w{4}-\w{4}-\w{4}-\w{12}&", string.Empty);

            Log.Information("Uploading Reports.");
            Log.Information($"url: {postUrl.Scheme}://{postUrl.Authority}");
            Log.Verbose($"api endpoint: {postUrl}");
            Log.Information($"query: {displayUrl}");
            Log.Information($"api version: { _apiVersion}");
            Log.Information("Pinging Codecov");

            string postResponse = await Post(postUrl);

            Log.Verbose($"response: {postResponse}");
            string[] postResponseSplit = postResponse.Split('\n');

            string reportUrl = postResponseSplit[_apiVersion == ApiVersion.V4 ? 0 : 1];
            Log.Information($"View reports at: {reportUrl}");

            // PUT request on V4
            if (_apiVersion == ApiVersion.V4)
            {
                Uri putUrl = new Uri(postResponseSplit[1]);
                Log.Information($"Uploading to S3 {putUrl.Scheme}://{putUrl.Authority}");

                await _httpClient.PutAsync(putUrl, new StringContent(_reportService.AggregatedContent));
            }
        }

        private async Task<string> Post(Uri url)
        {
            string content;
            switch (_apiVersion)
            {
                case ApiVersion.V2:
                    content = _reportService.AggregatedContent;
                    break;
                case ApiVersion.V4:
                    content = "";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_apiVersion));
            }

            HttpResponseMessage response = await _httpClient.PostAsync(url, new StringContent(content));
            return await response.Content.ReadAsStringAsync();
        }
    }
}
