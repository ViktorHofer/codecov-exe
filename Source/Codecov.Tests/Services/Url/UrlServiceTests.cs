using System;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Codecov.Services.Url
{
    public class UrlServiceTests
    {
        [Fact]
        public void Should_Return_Url()
        {
            // Given
            var host = Substitute.For<IHost>();
            host.Value.Returns("https://codecov.io");
            var route = Substitute.For<IRoute>();
            route.GetRoute(ApiVersion.V4).Returns("upload/v4");
            var query = Substitute.For<IQuery>();
            query.Value.Returns("branch=develop&commit=123");
            var url = new UrlService(host, route, query);

            // When
            var getUrl = url.GetUrl(ApiVersion.V4);

            // Then
            getUrl.Should().Be(new Uri("https://codecov.io/upload/v4?branch=develop&commit=123"));
        }
    }
}
