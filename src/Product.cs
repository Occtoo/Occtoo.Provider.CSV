using CsvHelper.Configuration.Attributes;

namespace Occtoo.Provider.CSV
{
    internal class Product
    {
        [Index(0)]
        public string Id { get; set; }

        [Index(1)]
        public string Sku { get; set; }

        [Index(2)]
        public string Name { get; set; }

        [Index(3)]
        public bool Published { get; set; }

        [Index(4)]
        public string ShortDescription { get; set; }

        [Index(5)]
        public string Description { get; set; }

        [Index(6)]
        public bool InStock { get; set; }

        [Index(7)]
        public string Stock { get; set; }

        [Index(8)]
        public string Categories { get; set; }
    }
}