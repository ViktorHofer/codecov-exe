using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Codecov.Services.VersionControl;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Codecov.Services.Report
{
    public class ReportServiceTests
    {
        [Fact]
        public void Should_Generate_A_Report()
        {
            // Given
            var environmentService = Substitute.For<IEnvironmentService>();
            environmentService.Variables.Returns(new Dictionary<string, string> { { "foo", "bar" }, { "fizz", "bizz" } });
            var sourceService = Substitute.For<ISourceService>();
            sourceService.FilteredFiles.Returns(new[] { @"C:\foo\Class.cs", @"C:\foo\Interface\IClass.cs" });
            sourceService.Directory.Returns(@"C:\foo");
            File.WriteAllText("./coverageUnit.xml", "Unit Tests.");
            File.WriteAllText("./coverageIntegration.xml", "Integration Tests.");
            var reportFiles = new string[] { "./coverageUnit.xml", "./coverageIntegration.xml" };
            var reportService = new ReportService(false, environmentService, sourceService, reportFiles);

            // When
            string aggregatedContent = reportService.AggregatedContent;

            // Then
            aggregatedContent.Should().Be("foo=bar\nfizz=bizz\n<<<<<< ENV\nClass.cs\nInterface\\IClass.cs\n<<<<<< network\n# path=./coverageUnit.xml\nUnit Tests.\n<<<<<< EOF\n# path=./coverageIntegration.xml\nIntegration Tests.\n<<<<<< EOF\n");

            // Clean Up
            File.Delete("./coverageUnit.xml");
            File.Delete("./coverageIntegration.xml");
        }

        [Fact]
        public void If_DisableNetwork_Is_True_The_Source_Code_Should_Be_Empty()
        {
            // Given
            var environmentService = Substitute.For<IEnvironmentService>();
            environmentService.Variables.Returns(new Dictionary<string, string> { { "foo", "bar" }, { "fizz", "bizz" } });
            var sourceService = Substitute.For<ISourceService>();
            sourceService.FilteredFiles.Returns(new[] { @"C:\foo\Class.cs", @"C:\foo\Interface\IClass.cs" });
            sourceService.Directory.Returns(@"C:\foo");
            File.WriteAllText("./coverageUnit.xml", "Unit Tests.");
            File.WriteAllText("./coverageIntegration.xml", "Integration Tests.");
            var reportFiles = new string[] { "./coverageUnit.xml", "./coverageIntegration.xml" };
            var reportService = new ReportService(true, environmentService, sourceService, reportFiles);

            // When
            string aggregatedContent = reportService.AggregatedContent;

            // Then
            aggregatedContent.Should().Be("foo=bar\nfizz=bizz\n<<<<<< ENV\n# path=./coverageUnit.xml\nUnit Tests.\n<<<<<< EOF\n# path=./coverageIntegration.xml\nIntegration Tests.\n<<<<<< EOF\n");
        }

        [Theory, InlineData(null), InlineData(""), InlineData("./ coverage.xml")]
        public void CoverageReport_Should_Be_Empty_If_The_File_Does_Not_Exits(string fileData)
        {
            // Given
            var environmentService = Substitute.For<IEnvironmentService>();
            Log.Create(false, false);
            var sourceService = Substitute.For<ISourceService>();
            var reportService = new ReportService(false, environmentService, sourceService, new string[] { fileData });

            // When
            Action coverageReport = () =>
            {
                var x = reportService.GetReports();
            };

            // Then
            coverageReport.Should().Throw<Exception>().WithMessage("No Report detected.");
        }

        [Fact]
        public void Should_Read_Multiple_Coverage_Reports()
        {
            // Given
            var environmentService = Substitute.For<IEnvironmentService>();
            var sourceService = Substitute.For<ISourceService>();
            var reportFiles = new string[] { "./coverageUnit.xml", "./coverageIntegration.xml" };
            File.WriteAllText("./coverageUnit.xml", "Unit Tests.");
            File.WriteAllText("./coverageIntegration.xml", "Integration Tests.");
            var reportService = new ReportService(false, environmentService, sourceService, reportFiles);

            // When
            var coverageReport = reportService.GetReports().ToList();

            // Then
            coverageReport.Count.Should().Be(2);
            coverageReport[0].File.Should().Be("./coverageUnit.xml");
            coverageReport[0].Content.Should().Be("Unit Tests.");
            coverageReport[1].File.Should().Be("./coverageIntegration.xml");
            coverageReport[1].Content.Should().Be("Integration Tests.");

            // Clean Up
            File.Delete("./coverageUnit.xml");
            File.Delete("./coverageIntegration.xml");
        }

        [Fact]
        public void Should_Read_Single_Coverage_Report()
        {
            // Given
            var environmentService = Substitute.For<IEnvironmentService>();
            var sourceService = Substitute.For<ISourceService>();
            var reportFiles = new string[] { "./coverage.xml" };
            File.WriteAllText("./coverage.xml", "Unit Tests.");
            var reportService = new ReportService(false, environmentService, sourceService, reportFiles);

            // When
            var coverageReport = reportService.GetReports().Single();

            // Then
            coverageReport.File.Should().Be("./coverage.xml");
            coverageReport.Content.Should().Be("Unit Tests.");

            // Clean Up
            File.Delete("./coverage.xml");
        }
    }
}
