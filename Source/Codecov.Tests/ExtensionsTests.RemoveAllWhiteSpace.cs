using FluentAssertions;
using Xunit;

namespace Codecov
{
    public partial class ExtensionsTests
    {
        [Fact]
        public void RemoveAllWhiteSpace_Should_Be_Empty_If_String_Is_Null()
        {
            // Given
            const string str = null;

            // When
            var flattenedString = str.RemoveAllWhiteSpace();

            // Then
            flattenedString.Should().BeEmpty();
        }

        [Fact]
        public void RemoveAllWhiteSpace_Should_Remove_All_White_Space()
        {
            // Given
            const string str = "  Hello      World   ";

            // When
            var flattenedString = str.RemoveAllWhiteSpace();

            // Then
            flattenedString.Should().Be("HelloWorld");
        }
    }
}
