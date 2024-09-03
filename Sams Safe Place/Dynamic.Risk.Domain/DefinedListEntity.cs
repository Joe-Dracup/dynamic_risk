namespace Dynamic.Risk.Domain
{
    public class DefinedListEntity
    {
        public int DefinedListId { get; set; }
        public string Name { get; set; }
    }

    public class DefinedListDetailEntity
    {
        public int UniqueId { get; set; }
        public int DefinedListId { get; set; }
        public string Name { get; set; }
        public string ABICode { get; set; }
    }
}
