using Project_2.Models.Constraints;
using Project_2.Models;
using Project_2.Services.Data;
using Project_2.Services.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2.Services
{
    public class CohortDataService
    {
        private readonly XenaScraperService _xenaScraper;
        private readonly MinioService _minioService;
        private readonly MongoService _mongoService;

        public CohortDataService(XenaScraperService xenaScraper, MinioService minioService, MongoService mongoService)
        {
            _xenaScraper = xenaScraper;
            _minioService = minioService;
            _mongoService = mongoService;
        }

        public async Task<bool> ProcessCohortAsync(string cohortId)
        {
            try
            {
                // Retrieve dataset from Xena Browser
                using var dataStream = await _xenaScraper.FindIlluminaHiSeqDatasetAsync(cohortId);
                if (dataStream == null)
                {
                    Console.WriteLine($"No IlluminaHiSeq pancan normalized data found for {cohortId}");
                    return false;
                }

                // Store in MinIO
                string objectName = $"{cohortId}_gene_expression.tsv";
                var tags = new Dictionary<string, string>
                {
                    { "cohort", cohortId },
                    { "content_type", "gene_expression" },
                    { "upload_date", DateTime.UtcNow.ToString("o") }
                };

                await _minioService.UploadObjectAsync(Constraints.MinioBucketName, objectName, dataStream, tags);

                // Read and process the data for MongoDB
                dataStream.Position = 0;
                using var reader = new StreamReader(dataStream);
                var geneExpressions = ParseGeneExpressionTsv(reader, cohortId, objectName);

                // Store gene expressions in MongoDB
                await _mongoService.InsertGeneExpressionsAsync(geneExpressions);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing cohort {cohortId}: {ex.Message}");
                return false;
            }
        }

        private List<GeneExpression> ParseGeneExpressionTsv(StreamReader reader, string cohortId, string sourceFile)
        {
            var result = new Dictionary<string, GeneExpression>();

            // Read header with patient IDs
            var headerLine = reader.ReadLine();
            var headers = headerLine.Split('\t');

            // Process each line
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split('\t');
                if (parts.Length < 2) continue;

                var geneName = parts[0];
                if (!Constraints.TargetGenes.Contains(geneName)) continue;

                // Process each patient (starting from column 1)
                for (int i = 1; i < parts.Length && i < headers.Length; i++)
                {
                    var patientId = headers[i];
                    if (!double.TryParse(parts[i], out double expressionValue))
                        continue;

                    // Create or update gene expression document
                    if (!result.ContainsKey(patientId))
                    {
                        result[patientId] = new GeneExpression
                        {
                            PatientId = patientId,
                            CancerCohort = cohortId,
                            SourceFile = sourceFile,
                            GeneExpressions = new Dictionary<string, double>()
                        };
                    }

                    result[patientId].GeneExpressions[geneName] = expressionValue;
                }
            }

            return result.Values.ToList();
        }
    
}
}
