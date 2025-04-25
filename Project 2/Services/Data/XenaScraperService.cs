using MongoDB.Bson.IO;
using Project_2.Models.Constraints;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Project_2.Models.Database;

namespace Project_2.Services.Data
{
    public class XenaScraperService
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _baseUrl;

        public XenaScraperService()
        {
            _baseUrl = Constraints.XenaApiBaseUrl;
            _client.BaseAddress = new Uri(_baseUrl);
        }

        public async Task<List<string>> GetAvailableCohortsAsync()
        {
            var response = await _client.GetAsync("/api/cohorts");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var cohorts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(content);
            return cohorts.Where(c => c.StartsWith("TCGA-")).ToList();
        }

        public async Task<Stream> FindIlluminaHiSeqDatasetAsync(string cohortId)
        {
            try
            {
                // Get cohort information
                var response = await _client.GetAsync($"/api/cohort/{cohortId}");
                response.EnsureSuccessStatusCode();
                var cohortContent = await response.Content.ReadAsStringAsync();
                var cohort = Newtonsoft.Json.JsonConvert.DeserializeObject<XenaCohort>(cohortContent);
                // Find IlluminaHiSeq pancan normalized gene file
                var dataset = cohort.Datasets.FirstOrDefault(d =>
                    d.Type == "genomicMatrix" &&
                    d.LongTitle != null &&
                    d.LongTitle.Contains("IlluminaHiSeq") &&
                    d.LongTitle.Contains("pancan normalized"));

                if (dataset == null)
                    return null;

                // Download the dataset
                var dataResponse = await _client.GetAsync(dataset.Url);
                dataResponse.EnsureSuccessStatusCode();
                return await dataResponse.Content.ReadAsStreamAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving dataset for {cohortId}: {ex.Message}");
                return null;
            }
        }
    }
}
