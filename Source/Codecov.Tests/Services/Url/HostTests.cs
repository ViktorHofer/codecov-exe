using FluentAssertions;
using System;
using Xunit;

namespace Codecov.Services.Url
{
    public class HostTests
    {
        [Fact]
        public void Should_Set_Default_Host()
        {
            // Given
            var host = new Host(string.Empty);

            // When
            var getHost = host.Value;

            // Then
            getHost.Should().Be("https://codecov.io");
        }

        [Fact]
        public void Should_Set_From_Commandline()
        {
            // Given
            var host = new Host("www.google.com/");

            // When
            var getHost = host.Value;

            // Then
            getHost.Should().Be("www.google.com");
        }

        [Fact]
        public void Should_Set_From_EnvironmentVariable()
        {
            // Given
            Environment.SetEnvironmentVariable("CODECOV_URL", "www.google.com/");
            var host = new Host(string.Empty);

            // When
            var getHost = host.Value;

            // Then
            getHost.Should().Be("www.google.com");

            // Cleanup
            Environment.SetEnvironmentVariable("CODECOV_URL", null);
        }
    }
}
