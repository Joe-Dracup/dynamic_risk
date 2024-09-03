using FluentAssertions;
using Xunit;

namespace Shoolese.Data.Test
{
    public class ShooleseDatabaseIntegrationTests
    {
        public class CreateTests
        {
            [Fact]
            public void Given_A_Valid_Connection_String_Returns_An_Instance()
            {
                var connstring = Constants.LocalConnectionString;

                var result = ShooleseDatabase.Create(connstring);

                result.IsSuccess.Should().BeTrue("Because this is a real connection string");
            }

            [Fact]
            public void Given_An_Invalid_Connection_String_Returns_An_Error()
            {
                var result = ShooleseDatabase.Create("trash");

                result.IsFailure.Should().BeTrue("Because this connection string is trash");
            }
        }
    }
}
