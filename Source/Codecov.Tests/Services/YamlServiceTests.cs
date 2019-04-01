using Codecov.Services.VersionControl;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Codecov.Services
{
    public class YamlServiceTests
    {
        [Fact]
        public void Should_Find_Yaml_File_If_Exits()
        {
            // Given
            var sourceCode = Substitute.For<ISourceService>();
            sourceCode.Files.Returns(new[] { @"C:\Fake\Class.cs", @"C:\Fake\Interface\IClass.cs", @"C:\Fake\.git", @"C:\Fake\codecov.yaml" });
            var yaml = new YamlService(null, sourceCode);

            // When
            string fileName = yaml.FileName;

            // Then
            fileName.Should().Be("codecov.yaml");
        }

        [Fact]
        public void Should_Return_Empty_String_If_Yaml_File_Does_Not_Exit()
        {
            // Given
            var sourceCode = Substitute.For<ISourceService>();
            sourceCode.Files.Returns(new[] { @"C:\Fake\Class.cs", @"C:\Fake\Interface\IClass.cs", @"C:\Fake\.git" });
            var yaml = new YamlService(null, sourceCode);

            // When
            string fileName = yaml.FileName;

            // Then
            fileName.Should().BeNull();
        }
    }
}
