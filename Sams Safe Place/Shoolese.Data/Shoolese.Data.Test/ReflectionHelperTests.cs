using CSharpFunctionalExtensions;
using FluentAssertions;
using Shoolese.Models.EverythingElse;
using Shoolese.Models.PersonalModels.PomodoroForMeModels.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xunit;

namespace Shoolese.Data.Test
{
    public class ReflectionHelperTests
    {
        public class VerifyOnlyExpectedTypesTests
        {
            [Theory]
            [InlineData(typeof(string),true)]
            [InlineData(typeof(DateTime),true)]
            [InlineData(typeof(int),true)]
            [InlineData(typeof(long),true)]
            [InlineData(typeof(bool),true)]
            [InlineData(typeof(Question),false)]
            [InlineData(typeof(BookNote), false)]
            public void Given_The_Inbound_Type_Was_In_Or_Out_Of_The_Allowed_List_Should_Return_Accordingly(Type type, bool isSuccess)
            {
                var inputList = new List<Type>() { type }.AsEnumerable();
                var allowedList = Constants.AllowedTypes;

                var result = ReflectionHelper.VerifyOnlyExpectedTypes(inputList, allowedList);

                result.IsSuccess.Should().Be(isSuccess, "Because this is what is expected when the inbound type is what it is.");

                if (!result.IsSuccess)
                    result.Error.Should().Be(ReflectionHelper.FailureReasons.TypeWasNotInAllowedList(type.Name, allowedList.Select(x => x.Name)));
            }
        }

        public class VerifyFirstPropertyContainsTheSubstringIdAndIsAnIntTests
        {
            [Fact]
            public void Given_A_Type_That_Does_Not_Have_Int_Id_As_First_Property_Should_Result_Fail()
            {
                var result = ReflectionHelper.VerifyFirstPropertyContainsTheSubstringIdAndIsAnIntOrLong<Employee>();

                result.IsFailure.Should().BeTrue("Because this class does not have a first type of 'Id' that is an int");
                result.Error.Should().Be(ReflectionHelper.FailureReasons.FirstPropertyOfTypeIsNotAnIntegerId(typeof(Employee).Name, "FirstName"));
            }

            [Fact]
            public void Given_A_Type_That_Has_An_Int_Id_As_First_Property_Should_Result_Success()
            {
                var result = ReflectionHelper.VerifyFirstPropertyContainsTheSubstringIdAndIsAnIntOrLong<Question>();

                result.IsSuccess.Should().BeTrue("Because this class has a field called QuestionId");
            }
        }

        public class MapCSharpTypesToSqlDbTypesTests
        {
            [Fact]
            public void Given_All_Types_Supported_Then_Should_Be_Able_To_Create_IEnumerable_As_Expected()
            {
                var expected = new List<KeyValuePair<string, SqlDbType>>()
                {
                    new KeyValuePair<string, SqlDbType>( "FirstType", SqlDbType.VarChar ),
                    new KeyValuePair<string, SqlDbType>( "SecondType", SqlDbType.Int ),
                    new KeyValuePair<string, SqlDbType>( "ThirdType", SqlDbType.BigInt ),
                    new KeyValuePair<string, SqlDbType>( "FourthType", SqlDbType.Bit )
                };

                var result = ReflectionHelper.MapCSharpTypesToSqlDbTypes<SupportedLongerType>();

                result.IsSuccess.Should().BeTrue("Because none of the properties of this type should fail to be converted to SqlDbTypes");
                result.Value.Select(x => new KeyValuePair<string, SqlDbType>(x.Key.Name, x.Value)).Should().BeEquivalentTo(expected, because: "Because that is what the output should look like");
            }
        }

        public class ResolveTypeToSqlTypeTests
        {
        }
    }

    public class UnsupportedFirstType
    {
        public Question FirstType { get; set; }
    }

    public class SupportedFirstType
    {
        public string FirstType { get; set; }
    }

    public class SupportedLongerType
    {
        public string FirstType { get; set; }
        public int SecondType { get; set; }
        public long ThirdType { get; set; }
        public bool FourthType { get; set; }
    }

    public class UnsupportedLongerType
    {
        public string FirstType { get; set; }
        public int SecondType { get; set; }
        public Question ThirdType { get; set; }
        public bool FourthType { get; set; }
    }
}
