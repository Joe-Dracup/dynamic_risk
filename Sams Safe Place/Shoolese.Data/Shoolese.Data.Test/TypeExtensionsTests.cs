using FluentAssertions;
using Shoolese.Models.PersonalModels.PomodoroForMeModels.DataModels;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Shoolese.Data.Test
{
    public class TypeExtensionsTests
    {
        public class IsNullableTests
        {
            [Fact]
            public void Given_Type_Is_Not_Nullable_Should_Return_False()
            {
                var type = typeof(int);

                var result = type.IsNullable();

                result.Should().BeFalse("Because int is not a nullable type");
            }

            [Fact]
            public void Given_Type_Is_Nullable_Should_Return_True()
            {
                var type = typeof(int?);

                var result = type.IsNullable();

                result.Should().BeTrue("Because int? is a nullable type");
            }
        }

        public class GetTypeOrUnderlyingTypeTests
        {
            [Fact]
            public void Given_Passing_In_A_Nullable_Type_Will_Return_The_Underlying_Type()
            {
                var type = typeof(int?);

                var result = type.GetTypeOrUnderlyingType();

                result.Should().Be(typeof(int), "Because this is a Nullable<T>-wrapped integer");
            }

            [Fact]
            public void Given_Passing_In_A_Non_Nullable_Type_Will_Return_The_Original_Type()
            {
                var type = typeof(int);

                var result = type.GetTypeOrUnderlyingType();

                result.Should().Be(typeof(int), "Because there is nothing to unwrap here");
            }
        }

        public class GetAllPropertyTypesTests{ }

        public class GetAllRootPropertyTypesTests{
            [Fact]
            public void Given_A_Type_Should_Return_All_Expected_Property_Types_Of_That_Type()
            {
                var expectedFlatPropertyTypes = new List<string>() { "Int32", "String", "String", "DateTime", "Int32", "Int32", "Boolean", "Int32" };

                var result = typeof(Question).GetAllRootPropertyTypes();

                result.Select(x => x.Name).Should().BeEquivalentTo(expectedFlatPropertyTypes, "Because these are the names of the properties types for Question");
            }
        }
    }
}
