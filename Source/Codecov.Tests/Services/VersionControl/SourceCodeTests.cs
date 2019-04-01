using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Codecov.Services.VersionControl
{
    public class SourceCodeTests
    {
        [Fact]
        public void GetAll_Should_Get_All_The_Source_Code()
        {
            // Given
            var versionControlService = Substitute.For<IVersionControlService>();
            versionControlService.SourceCode.Returns(new[] { @"C:\Fake\Class.cs", @"C:\Fake\Interface\IClass.cs", @"C:\Fake\.git" });
            var sourceService = new SourceService(versionControlService);

            // When
            List<string> files = sourceService.Files.ToList();

            // Then
            files.Count.Should().Be(3);
            files[0].Should().Be(@"C:\Fake\Class.cs");
            files[1].Should().Be(@"C:\Fake\Interface\IClass.cs");
            files[2].Should().Be(@"C:\Fake\.git");
        }

        [Fact]
        public void GetAllButCodecovIgnored_Should_Get_All_Source_Code_That_Is_Not_Ignored_By_Codecov()
        {
            // Given
            var versionControlService = Substitute.For<IVersionControlService>();
            versionControlService.SourceCode.Returns(new[] { @"C:\Fake\Class.cs", @"C:\Fake\Interface\IClass.cs", @"C:\Fake\.git" });
            var sourceService = new SourceService(versionControlService);

            // When
            List<string> filteredFiles = sourceService.FilteredFiles.ToList();

            // Then
            filteredFiles.Count.Should().Be(2);
            filteredFiles[0].Should().Be(@"C:\Fake\Class.cs");
            filteredFiles[1].Should().Be(@"C:\Fake\Interface\IClass.cs");
        }
    }
}
