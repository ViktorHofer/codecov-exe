using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Codecov.Services.VersionControl
{
    public class GitTests
    {
        [Fact]
        public void Branch_Should_Return_Correct_Branch_If_Exits()
        {
            // Given
            var terminal = Substitute.For<ITerminalService>();
            terminal.Run("git", $@"-C ""{Directory.GetCurrentDirectory()}"" rev-parse --abbrev-ref HEAD").Returns("develop");
            var git = new Git(string.Empty, terminal);

            // When
            var branch = git.Branch;

            // Then
            branch.Should().Be("develop");
        }

        [Theory, InlineData(null), InlineData(""), InlineData("HEAD")]
        public void Branch_Should_Return_Null(string branchData)
        {
            // Given
            var terminal = Substitute.For<ITerminalService>();
            terminal.Run("git", $@"-C ""{Directory.GetCurrentDirectory()}"" rev-parse --abbrev-ref HEAD").Returns(branchData);
            var git = new Git(string.Empty, terminal);

            // When
            var branch = git.Branch;

            // Then
            branch.Should().BeNull();
        }

        [Fact]
        public void Commit_Should_Return_Correct_Commit_If_Exits()
        {
            // Given
            var terminal = Substitute.For<ITerminalService>();
            terminal.Run("git", $@"-C ""{Directory.GetCurrentDirectory()}"" rev-parse HEAD").Returns("11");
            var git = new Git(string.Empty, terminal);

            // When
            var commt = git.Commit;

            // Then
            commt.Should().Be("11");
        }

        [Theory, InlineData(null), InlineData("")]
        public void Commit_Should_Return_Null(string commitData)
        {
            // Given
            var terminal = Substitute.For<ITerminalService>();
            terminal.Run("git", $@"-C ""{Directory.GetCurrentDirectory()}"" rev-parse HEAD").Returns(commitData);
            var git = new Git(string.Empty, terminal);

            // When
            var commit = git.Commit;

            // Then
            commit.Should().BeNull();
        }

        [Theory, InlineData(null), InlineData("")]
        public void Detecter_Should_Be_False_If_Returns_Null_Or_Empty_String(string terminalData)
        {
            // Given
            var terminal = Substitute.For<ITerminalService>();
            terminal.Run("git", "--version").Returns(terminalData);
            Directory.CreateDirectory(".git");
            var git = new Git(string.Empty, terminal);

            // When
            bool exists = git.Exists;

            // Then
            exists.Should().BeFalse();

            // Clean up
            Directory.Delete(".git");
        }

        [Fact]
        public void Detecter_Should_Be_False_If_Dot_Git_Directory_Does_Not_Exit()
        {
            // Given
            var terminal = Substitute.For<ITerminalService>();
            terminal.Run("git", "--version").Returns("foo");
            var git = new Git(string.Empty, terminal);

            // When
            bool exists = git.Exists;

            // Then
            exists.Should().BeFalse();
        }

        [Fact]
        public void Detecter_Should_Be_True_If_Does_Not_Returns_Null_Or_Empty_String()
        {
            // Given
            var terminal = Substitute.For<ITerminalService>();
            terminal.Run("git", "--version").Returns("foo");
            Directory.CreateDirectory(".git");
            var git = new Git(string.Empty, terminal);

            // When
            bool exists = git.Exists;

            // Then
            exists.Should().BeTrue();

            // Clean up
            Directory.Delete(".git");
        }

        [Theory, InlineData(null), InlineData("")]
        public void RepoRoot_Should_Default_To_Current_Directory_If_Get_Fails(string terminalData)
        {
            // Given
            var terminal = Substitute.For<ITerminalService>();
            terminal.Run("git", "rev-parse --show-toplevel").Returns(terminalData);
            var git = new Git(string.Empty, terminal);

            // When
            string repoRoot = git.RepoRoot;

            // Then
            repoRoot.Should().Be(Directory.GetCurrentDirectory());
        }

        [Theory, InlineData(null), InlineData("")]
        public void RepoRoot_Should_Get_From_Git_If_Options_Are_Not_Set(string repoRootArg)
        {
            // Given
            var terminal = Substitute.For<ITerminalService>();
            terminal.Run("git", "rev-parse --show-toplevel").Returns(@"C:\Fake");
            var git = new Git(repoRootArg, terminal);

            // When
            string repoRoot = git.RepoRoot;

            // Then
            repoRoot.Should().Be(@"C:\Fake");
        }

        [Theory, InlineData(@".\Fake"), InlineData(@"./Fake")]
        public void RepoRoot_Should_Return_Absolute_Path_When_Given_Relative_Path(string repoRootArg)
        {
            // Given
            var terminal = Substitute.For<ITerminalService>();
            var git = new Git(repoRootArg, terminal);

            // When
            string repoRoot = git.RepoRoot;

            // Then
            repoRoot.Should().Be($@"{Directory.GetCurrentDirectory()}\Fake");
        }

        [Fact]
        public void Should_Get_SourceCode()
        {
            // Given
            var terminal = Substitute.For<ITerminalService>();
            terminal.Run("git", $@"-C ""{Directory.GetCurrentDirectory()}"" ls-tree --full-tree -r HEAD --name-only").Returns("Class.cs");
            var git = new Git(string.Empty, terminal);

            // When
            IEnumerable<string> sourceCode = git.SourceCode.ToArray();

            // Then
            sourceCode.Count().Should().Be(1);
            sourceCode.ElementAt(0).Should().Be($@"{Directory.GetCurrentDirectory()}\Class.cs");
        }

        [Theory, InlineData("Class.cs\nIClass.cs"), InlineData("\nClass.cs\nIClass.cs\n\n")]
        public void Should_Get_SourceCode_Seperated_By_NewLines(string terminalData)
        {
            // Given
            var terminal = Substitute.For<ITerminalService>();
            terminal.Run("git", $@"-C ""{Directory.GetCurrentDirectory()}"" ls-tree --full-tree -r HEAD --name-only").Returns(terminalData);
            var git = new Git(string.Empty, terminal);

            // When
            IEnumerable<string> sourceCode = git.SourceCode.ToArray();

            // Then
            sourceCode.Count().Should().Be(2);
            sourceCode.ElementAt(0).Should().Be($@"{Directory.GetCurrentDirectory()}\Class.cs");
            sourceCode.ElementAt(1).Should().Be($@"{Directory.GetCurrentDirectory()}\IClass.cs");
        }

        [Theory, InlineData(null), InlineData("")]
        public void Should_Return_Null_If_SourceCode_Does_Not_Exit(string terminalData)
        {
            // Given
            var terminal = Substitute.For<ITerminalService>();
            terminal.Run("git", $@"-C ""{Directory.GetCurrentDirectory()}"" ls-tree --full-tree -r HEAD --name-only").Returns(terminalData);
            var git = new Git(string.Empty, terminal);

            // When
            IEnumerable<string> sourceCode = git.SourceCode;

            // Then
            sourceCode.Should().BeNull();
        }

        [Theory, InlineData("https://github.com/larzw/codecov-exe.git"), InlineData("git@github.com:larzw/codecov-exe.git")]
        public void Slug_Should_Return_Correct_Result(string slugData)
        {
            // Given
            var terminal = Substitute.For<ITerminalService>();
            terminal.Run("git", $@"-C ""{Directory.GetCurrentDirectory()}"" config --get remote.origin.url").Returns(slugData);
            var git = new Git(null, terminal);

            // When
            string slug = git.Slug;

            // Then
            slug.Should().Be("larzw/codecov-exe");
        }

        [Theory, InlineData(null), InlineData(""), InlineData("NotASlug")]
        public void Slug_Should_Return_Null(string slugData)
        {
            // Given
            var terminal = Substitute.For<ITerminalService>();
            terminal.Run("git", $@"-C ""{Directory.GetCurrentDirectory()}"" config --get remote.origin.url").Returns(slugData);
            var git = new Git(null, terminal);

            // When
            string slug = git.Slug;

            // Then
            slug.Should().BeNull();
        }

        [Theory, InlineData(null), InlineData("")]
        public void SourceCode_Should_Be_Null_If_Git_Returns_Null_Or_Empty_String(string terminalData)
        {
            // Given
            var terminal = Substitute.For<ITerminalService>();
            terminal.Run("git", $@"-C ""{Directory.GetCurrentDirectory()}"" ls-tree --full-tree -r HEAD --name-only").Returns(terminalData);
            var git = new Git(null, terminal);

            // When
            IEnumerable<string> sourceCode = git.SourceCode;

            // Then
            sourceCode.Should().BeNull();
        }
    }
}
