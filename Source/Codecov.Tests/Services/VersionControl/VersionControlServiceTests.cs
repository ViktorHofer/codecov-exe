using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Codecov.Services.VersionControl
{
    public class VersionControlServiceTests
    {
        [Fact]
        public void Branch_Should_Be_Null_When_Enviornment_Variable_Does_Not_Exits()
        {
            // Given
            Environment.SetEnvironmentVariable("VCS_BRANCH_NAME", null);
            var versionControlService = new VersionControlService(string.Empty);

            // When
            var branch = versionControlService.Branch;

            // Then
            branch.Should().BeNull();
        }

        [Fact]
        public void Branch_Should_Be_Set_When_Enviornment_Variable_Exits()
        {
            // Given
            Environment.SetEnvironmentVariable("VCS_BRANCH_NAME", "develop");
            var versionControlService = new VersionControlService(string.Empty);

            // When
            var branch = versionControlService.Branch;

            // Then
            branch.Should().Be("develop");
        }

        [Fact]
        public void Detecter_Should_Be_False()
        {
            // Given
            var versionControlService = new VersionControlService(string.Empty);

            // When
            bool exists = versionControlService.Exists;

            // Then
            exists.Should().BeFalse();
        }

        [Fact]
        public void Pr_Should_Be_Null_When_Enviornment_Variable_Does_Not_Exits()
        {
            // Given
            Environment.SetEnvironmentVariable("VCS_PULL_REQUEST", null);
            var versionControlService = new VersionControlService(string.Empty);

            // When
            string pr = versionControlService.PR;

            // Then
            pr.Should().BeNull();
        }

        [Fact]
        public void Pr_Should_Be_Set_When_Enviornment_Variable_Exits()
        {
            // Given
            Environment.SetEnvironmentVariable("VCS_PULL_REQUEST", "123");
            var versionControlService = new VersionControlService(string.Empty);

            // When
            var pr = versionControlService.PR;

            // Then
            pr.Should().Be("123");
        }

        [Fact]
        public void Repo_Root_Should_Be_A_Normalized_Path_When_Options_Are_Set()
        {
            // Given
            var versionControlService = new VersionControlService("c:/fake/github/");

            // When
            var repoRoot = versionControlService.RepoRoot;

            // Then
            repoRoot.Should().Be(@"c:\fake\github");
        }

        [Theory, InlineData(""), InlineData(null)]
        public void Repo_Root_Should_Default_To_Current_Directory_When_Options_Are_Null_Or_Empty_String(string repoRootArg)
        {
            // Given
            var versionControlService = new VersionControlService(repoRootArg);

            // When
            string repoRoot = versionControlService.RepoRoot;

            // Then
            repoRoot.Should().Be(Directory.GetCurrentDirectory());
        }

        [Fact]
        public void Slug_Should_Be_Null_When_Enviornment_Variable_Does_Not_Exits()
        {
            // Given
            Environment.SetEnvironmentVariable("VCS_SLUG", null);
            var versionControlService = new VersionControlService(string.Empty);

            // When
            string slug = versionControlService.Slug;

            // Then
            slug.Should().BeNull();
        }

        [Fact]
        public void Slug_Should_Be_Set_When_Enviornment_Variable_Exits()
        {
            // Given
            Environment.SetEnvironmentVariable("VCS_SLUG", "owner/repo");
            var versionControlService = new VersionControlService(string.Empty);

            // When
            var slug = versionControlService.Slug;

            // Then
            slug.Should().Be("owner/repo");
        }

        [Fact]
        public void SourceCode_Should_Return_Empty_Collection_If_No_Files_Exit()
        {
            // Given
            Directory.CreateDirectory("./FakeRepo");
            Directory.CreateDirectory("./FakeRepo/Src");

            var versionControlService = new VersionControlService("./FakeRepo");

            // When
            IEnumerable<string> sourceCode = versionControlService.SourceCode;

            // Then
            sourceCode.Should().BeEmpty();

            // Clean up
            Directory.Delete("./FakeRepo", true);
        }

        [Fact]
        public void SourceCode_Should_Return_File_Collection_If_Files_Exit()
        {
            // Given
            var baseDirectory = $@"{Directory.GetCurrentDirectory()}\FakeRepo";
            Directory.CreateDirectory("./FakeRepo");
            Directory.CreateDirectory("./FakeRepo/Src");
            using (File.Create("./FakeRepo/.git")) { }
            using (File.Create("./FakeRepo/Src/class1.cs")) { }
            using (File.Create("./FakeRepo/README.md")) { }
            var versionControlService = new VersionControlService("./FakeRepo");

            // When
            List<string> sortedSourceCode = versionControlService.SourceCode.ToList();
            sortedSourceCode.Sort();

            // Then
            sortedSourceCode[0].Should().Be($@"{baseDirectory}\.git");
            sortedSourceCode[1].Should().Be($@"{baseDirectory}\README.md");
            sortedSourceCode[2].Should().Be($@"{baseDirectory}\Src\class1.cs");

            // Clean up
            Directory.Delete("./FakeRepo", true);
        }

        [Fact]
        public void Tag_Should_Be_Null_When_Enviornment_Variable_Does_Not_Exits()
        {
            // Given
            Environment.SetEnvironmentVariable("VCS_TAG", null);
            var versionControlService = new VersionControlService(string.Empty);

            // When
            string tag = versionControlService.Tag;

            // Then
            tag.Should().BeNull();
        }

        [Fact]
        public void Tag_Should_Be_Set_When_Enviornment_Variable_Exits()
        {
            // Given
            Environment.SetEnvironmentVariable("VCS_TAG", "v1.0.0");
            var versionControlService = new VersionControlService(string.Empty);

            // When
            string tag = versionControlService.Tag;

            // Then
            tag.Should().Be("v1.0.0");
        }
    }
}
