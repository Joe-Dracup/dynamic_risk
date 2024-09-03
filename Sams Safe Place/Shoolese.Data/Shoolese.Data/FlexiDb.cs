using CSharpFunctionalExtensions;
using Dynamic.Risk.Domain;

namespace Shoolese.Data
{
    public interface IFlexiDb
    {
        public IFlexiTable<DefinedListEntity> DefinedLists { get; }
        public IFlexiTable<DefinedListDetailEntity> DefinedListDetails { get; }
    }

    public class FlexiDb : IFlexiDb
    {
        private readonly string _connectionString;
        
        private FlexiTable<DefinedListEntity> _definedLists { get; set; }
        private FlexiTable<DefinedListDetailEntity> _definedListDetails { get; set; }
        public IFlexiTable<DefinedListEntity> DefinedLists => _definedLists;
        public IFlexiTable<DefinedListDetailEntity> DefinedListDetails => _definedListDetails;

        protected FlexiDb(string connectionString)
        {
            _connectionString = connectionString;
            _definedLists = new FlexiTable<DefinedListEntity>("[dbo].[DefinedList]", connectionString, nameof(DefinedListEntity.DefinedListId));
            _definedListDetails = new FlexiTable<DefinedListDetailEntity>("[dbo].[DefinedListDetails]", connectionString, nameof(DefinedListDetailEntity.UniqueId));
        }

        public static Result<FlexiDb> Create(string connectionString)
        {
            // After this, can assume that _connectionString is _always_ valid.
            var connStringValid = SQLConnectionVerifier.VerifyConnectionString(connectionString);
            return connStringValid.IsSuccess
                ? Result.Success(new FlexiDb(connectionString))
                : Result.Failure<FlexiDb>(connStringValid.Error);
        }
    }
}
