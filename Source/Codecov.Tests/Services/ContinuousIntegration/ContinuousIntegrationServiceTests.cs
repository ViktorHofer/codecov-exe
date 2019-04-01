using System;
using FluentAssertions;
using Xunit;

namespace Codecov.Services.ContinuousIntegration
{
    public class ContinuousIntegrationServiceTests
    {
        [Fact]
        public void Branch_Should_Be_Null()
        {
            // Given
            var continuousIntegrationService = new ContinuousIntegrationService();

            // When
            string branch = continuousIntegrationService.Branch;

            // Then
            branch.Should().BeNull();
        }

        [Fact]
        public void Build_Should_Be_Null_When_Enviornment_Variable_Does_Not_Exits()
        {
            // Given
            Environment.SetEnvironmentVariable("CI_BUILD_ID", null);
            var continuousIntegrationService = new ContinuousIntegrationService();

            // When
            string build = continuousIntegrationService.Build;

            // Then
            build.Should().BeNull();
        }

        [Fact]
        public void Build_Should_Be_Set_When_Enviornment_Variable_Exits()
        {
            // Given
            Environment.SetEnvironmentVariable("CI_BUILD_ID", "123");
            var continuousIntegrationService = new ContinuousIntegrationService();

            // When
            string build = continuousIntegrationService.Build;

            // Then
            build.Should().Be("123");
        }

        [Fact]
        public void BuildUrl_Should_Be_Null_When_Enviornment_Variable_Does_Not_Exits()
        {
            // Given
            Environment.SetEnvironmentVariable("CI_BUILD_URL", null);
            var continuousIntegrationService = new ContinuousIntegrationService();

            // When
            string buildUrl = continuousIntegrationService.BuildUrl;

            // Then
            buildUrl.Should().BeNull();
        }

        [Fact]
        public void BuildUrl_Should_Be_Set_When_Enviornment_Variable_Exits()
        {
            // Given
            Environment.SetEnvironmentVariable("CI_BUILD_URL", "www.google.com");
            var continuousIntegrationService = new ContinuousIntegrationService();

            // When
            string buildUrl = continuousIntegrationService.BuildUrl;

            // Then
            buildUrl.Should().Be("www.google.com");
        }

        [Fact]
        public void Commit_Should_Null()
        {
            // Given
            var continuousIntegrationService = new ContinuousIntegrationService();

            // When
            string commit = continuousIntegrationService.Commit;

            // Then
            commit.Should().BeNull();
        }

        [Fact]
        public void Exists_Should_False()
        {
            // Given
            var continuousIntegrationService = new ContinuousIntegrationService();

            // when
            bool exists = continuousIntegrationService.Exists;

            // Then
            exists.Should().BeFalse();
        }

        [Fact]
        public void GetEnviornmentVariables_Should_Empty_Dictionary()
        {
            // Given
            var continuousIntegrationService = new ContinuousIntegrationService();

            // When
            var enviornmentVariables = continuousIntegrationService.EnvironmentVariables;

            // Then
            enviornmentVariables.Should().BeEmpty();
        }

        [Fact]
        public void Job_Should_Be_Null_When_Enviornment_Variable_Does_Not_Exits()
        {
            // Given
            Environment.SetEnvironmentVariable("CI_JOB_ID", null);
            var continuousIntegrationService = new ContinuousIntegrationService();

            // When
            string job = continuousIntegrationService.Job;

            // Then
            job.Should().BeNull();
        }

        [Fact]
        public void Job_Should_Be_Set_When_Enviornment_Variable_Exits()
        {
            // Given
            Environment.SetEnvironmentVariable("CI_JOB_ID", "123");
            var continuousIntegrationService = new ContinuousIntegrationService();

            // When
            string job = continuousIntegrationService.Job;

            // Then
            job.Should().Be("123");
        }

        [Fact]
        public void PR_Should_Null()
        {
            // Given
            var continuousIntegrationService = new ContinuousIntegrationService();

            // when
            string pr = continuousIntegrationService.PR;

            // Then
            pr.Should().BeNull();
        }

        [Fact]
        public void Service_Should_Null()
        {
            // Given
            var continuousIntegrationServer = new ContinuousIntegrationService();

            // When
            string service = continuousIntegrationServer.Service;

            // Then
            service.Should().BeNull();
        }

        [Fact]
        public void Slug_Should_Null()
        {
            // Given
            var continuousIntegrationService = new ContinuousIntegrationService();

            // When
            string slug = continuousIntegrationService.Slug;

            // Then
            slug.Should().BeNull();
        }

        [Fact]
        public void Tag_Should_Null()
        {
            // Given
            var continuousIntegrationService = new ContinuousIntegrationService();

            // When
            string slug = continuousIntegrationService.Tag;

            // Then
            slug.Should().BeNull();
        }
    }
}
