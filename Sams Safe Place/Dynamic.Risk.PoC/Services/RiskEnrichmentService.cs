using Dynamic.Risk.Domain;
using Shoolese.Data;

namespace Dynamic.Risk.PoC.Services
{
    public class RiskEnrichmentService
    {
        private EnricherFactory _enricherFactory = new EnricherFactory();

        public Dictionary<string, RiskEntry> Enrich(Dictionary<string, RiskEntry> riskDictionary)
        {
            var newDict = new Dictionary<string, RiskEntry>();

            foreach(var entry in riskDictionary)
            {
                var enricher = _enricherFactory.GetEnricher(entry.Value.ParseType);

                var enrichmentResult = enricher.Enrich(entry.Key, entry.Value);

                foreach(var result in enrichmentResult)
                {
                    newDict.Add(result.Item1, result.Item2);
                }
            }

            return newDict;
        }
    }

    public class EnricherFactory
    {
        public IEnricher GetEnricher(ParseType parseType)
        {
            switch (parseType)
            {
                case ParseType.@string:
                    return new StringEnricher();
                case ParseType.number:
                    return new NumberEnricher();
                case ParseType.definedListDetail:
                    return new DefinedListDetailEnricher();
                case ParseType.date:
                    return new DateEnricher();
            }

            throw new Exception("No enricher configured for this ParseType");
        }
    }

    public interface IEnricher
    {
        IEnumerable<(string,RiskEntry)> Enrich(string name, RiskEntry entry);
    }

    public class StringEnricher : IEnricher
    {
        public IEnumerable<(string, RiskEntry)> Enrich(string name, RiskEntry entry) => new List<(string,RiskEntry)> { (name, entry) };
    }

    public class NumberEnricher : IEnricher
    {
        public IEnumerable<(string, RiskEntry)> Enrich(string name, RiskEntry entry) => new List<(string, RiskEntry)> { (name, entry) };
    }

    public class DefinedListDetailEnricher : IEnricher
    {
        private IFlexiDb _db;

        public DefinedListDetailEnricher()
        {
            _db = FlexiDb.Create("Data Source=localhost;Initial Catalog=innovation-03-09-24;Integrated Security=True").Value;
        }

        public IEnumerable<(string, RiskEntry)> Enrich(string name, RiskEntry entry)
        {
            var retrieved = _db.DefinedListDetails.GetById(int.Parse(entry.Value)).Value;

            return new List<(string, RiskEntry)> 
            { 
                ($"{name}.{nameof(DefinedListDetailEntity.Name)}", new RiskEntry(){ Value = retrieved.Name, ParseType = ParseType.@string }),
                ($"{name}.{nameof(DefinedListDetailEntity.ABICode)}", new RiskEntry(){ Value = retrieved.ABICode, ParseType = ParseType.@string }),
                ($"{name}.{nameof(DefinedListDetailEntity.UniqueId)}", new RiskEntry(){ Value = retrieved.UniqueId.ToString(), ParseType = ParseType.@string }),
            };
        }
    }

    public class DateEnricher : IEnricher
    {
        public IEnumerable<(string, RiskEntry)> Enrich(string name, RiskEntry entry) => new List<(string, RiskEntry)> { (name, entry) };
    }   
}
