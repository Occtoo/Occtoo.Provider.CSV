using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using Occtoo.Onboarding.Sdk;
using System.Globalization;
using System.Text;

namespace Occtoo.Provider.CSV
{
    internal class Program
    {
        private static readonly string dataSource = "product";

        static async Task Main(string[] args)
        {
            // Get secrets
            var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
            var config = builder.Build();
            var providerId = config["providerid"];
            var providerSecret = config["providersecret"];

            // Read and parse file
            var filestream = File.Open(@"Product_Data_Sample.csv", FileMode.Open);
            var ids = new List<string>() { "Id" };
            var products = ParseCvlFile<Product>(filestream, ids);

            // Upload entities to onboardService
            var onboardingService = new OnboardingServiceClient(providerId, providerSecret);
            var response = await onboardingService.StartEntityImportAsync(dataSource, products);
            if (response.StatusCode != 202)
            {
                throw new Exception($"Batch import was not successful - status code: {response.StatusCode}. {response.Message}");
            }

            Console.WriteLine($"{products.Count} {dataSource} got onboarded to Occtoo!");
        }

        public static List<DynamicEntity> ParseCvlFile<T>(Stream filestream, List<string> idValues)
        {
            var response = new List<DynamicEntity>();
            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                };
                using (var reader = new StreamReader(filestream, Encoding.GetEncoding("ISO-8859-1"), true))
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        var record = csv.GetRecord<T>();

                        if (!RecordContainsIdValuess(record, idValues))
                        {
                            continue;
                        }

                        var dynamicEntity = new DynamicEntity
                        {
                            Key = GetKeyString(record, idValues)
                        };

                        var props = record.GetType().GetProperties();
                        foreach (var prop in props)
                        {
                            if (prop.Name == "Id")
                            {
                                continue;
                            }

                            var value = prop.GetValue(record).ToString();
                            dynamicEntity.Properties.Add(new DynamicProperty
                            {
                                Id = prop.Name,
                                Value = value
                            });
                        }

                        response.Add(dynamicEntity);
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine($"The file could not be read:");
                Console.WriteLine(e.Message);
            }

            return response;
        }

        private static string GetKeyString<T>(T record, List<string> idValues)
        {
            var props = record.GetType().GetProperties();
            var result = "";
            foreach (var id in idValues)
            {
                var prop = props.Where(p => p.Name == id).FirstOrDefault();
                result += $"{prop.GetValue(record)}_";
            }

            return result.TrimEnd('_');
        }

        private static bool RecordContainsIdValuess<T>(T record, List<string> idValues)
        {
            var props = record.GetType().GetProperties();
            foreach (var id in idValues)
            {
                var prop = props.Where(p => p.Name == id).FirstOrDefault();
                if (prop == null || string.IsNullOrEmpty(prop.GetValue(record).ToString()))
                {
                    return false;
                }
            }

            return true;
        }

    }
}