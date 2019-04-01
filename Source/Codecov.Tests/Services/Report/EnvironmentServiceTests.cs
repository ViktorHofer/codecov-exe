using System;
using System.Collections.Generic;
using System.Linq;
using Codecov.Services.ContinuousIntegration;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Codecov.Services.Report
{
    public class EnvironmentServiceTests
    {
        [Fact]
        public void GetEnviornmentVariables_Should_Never_Be_Null()
        {
            // Given
            var continuousIntegrationService = Substitute.For<ContinuousIntegrationService>();
            var environmentService = new EnvironmentService(Enumerable.Empty<string>(), continuousIntegrationService);

            // When
            IDictionary<string, string> variables = environmentService.Variables;

            // Then
            variables.Should().NotBeNull();
        }

        [Fact]
        public void Should_Include_CODECOV_ENV()
        {
            // Given
            var continuousIntegrationService = Substitute.For<ContinuousIntegrationService>();
            var environmentService = new EnvironmentService(Enumerable.Empty<string>(), continuousIntegrationService);
            Environment.SetEnvironmentVariable("CODECOV_ENV", "foo");

            // When
            IDictionary<string, string> variables = environmentService.Variables;

            // Then
            variables["CODECOV_ENV"].Should().Be("foo");
        }

        [Fact]
        public void Should_Include_EnviornmentVariables_From_ContiniousIntegrationServer()
        {
            // Given
            var continuousIntegrationService = Substitute.For<ContinuousIntegrationService>();
            continuousIntegrationService.EnvironmentVariables.Returns(new Dictionary<string, string> { { "foo", "bar" }, { "fizz", "bizz" } });
            var environmentService = new EnvironmentService(Enumerable.Empty<string>(), continuousIntegrationService);
            Environment.SetEnvironmentVariable("foo", null);
            Environment.SetEnvironmentVariable("fizz", null);

            // When
            IDictionary<string, string> variables = environmentService.Variables;

            // Then
            variables["foo"].Should().Be("bar");
            variables["fizz"].Should().Be("bizz");
        }

        [Fact]
        public void Should_Include_EnviornmentVariables_From_Options()
        {
            // Given
            var continuousIntegrationService = Substitute.For<ContinuousIntegrationService>();
            var environmentService = new EnvironmentService(new[] { "foo", "fizz" }, continuousIntegrationService);
            Environment.SetEnvironmentVariable("foo", "bar");
            Environment.SetEnvironmentVariable("fizz", "bizz");

            // When
            IDictionary<string, string> variables = environmentService.Variables;

            // Then
            variables["foo"].Should().Be("bar");
            variables["fizz"].Should().Be("bizz");
        }
    }
}
