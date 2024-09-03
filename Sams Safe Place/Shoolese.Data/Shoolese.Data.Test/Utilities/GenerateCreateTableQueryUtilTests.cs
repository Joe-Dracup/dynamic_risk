using FluentAssertions;
using Shoolese.Data.Utilities;
using Shoolese.Models.PersonalModels.PomodoroForMeModels.DataModels;
using System.Linq;
using Xunit;

namespace Shoolese.Data.Test.Utilities
{
    public class GenerateCreateTableQueryUtilTests
    {
        public class GenerateCreateTableCommandTests { 
            
            [Fact]
            public void Given_Type_Argument_Has_Complex_Types_Then_Should_Refuse_To_Generate_Command()
            {
                var result = GenerateCreateTableQueryUtil.GenerateCreateTableCommand<ClassWithComplexTypes>();

                result.IsFailure.Should().BeTrue("Because it should not be able to create a CreateTable command for this type, since the type has non-primitive properties");
                result.Error.Should().Contain(ReflectionHelper.FailureReasons.TypeWasNotInAllowedList(typeof(Question).Name, Constants.AllowedTypes.Select(x => x.Name)), "Because this is the expected error message");
            }

            [Fact]
            public void Given_Type_Argument_Does_Not_Have_Int_Id_As_First_Property_Should_Refuse_To_Generate_Command()
            {
                var result = GenerateCreateTableQueryUtil.GenerateCreateTableCommand<ClassWithoutComplexTypesButFirstPropertyIsntId>();

                result.IsFailure.Should().BeTrue("Because it should not be able to create a CreateTable command for this type, since it's first property is not an Integer called Id");
                result.Error.Should().Contain(ReflectionHelper.FailureReasons.FirstPropertyOfTypeIsNotAnIntegerId(typeof(ClassWithoutComplexTypesButFirstPropertyIsntId).Name, "Derp"), "Because this is the expected error message");
            }
        }
    }

    public class ClassWithComplexTypes
    {
        public int Id { get; set; }
        public Question Question { get; set; }
        public int Errr { get; set; }
    }

    public class ClassWithoutComplexTypesButFirstPropertyIsntId
    {
        public string Derp { get; set; }
        public int Derp2 { get; set; }
    }
}
