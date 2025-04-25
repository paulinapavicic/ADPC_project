using Minio;
using MongoDB.Bson;
using Project_2.Models.Constraints;
using Project_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Project_2.Services.Storage
{
    public class MongoService
    {
        private readonly IMongoCollection<GeneExpression> _geneExpressionCollection;
        private readonly IMongoCollection<ClinicalSurvival> _clinicalCollection;
        private readonly IMongoCollection<GeneExpressionClinical> _mergedCollection;

        public MongoService()
        {
            var settings = new MongoDBSettings();
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _geneExpressionCollection = database.GetCollection<GeneExpression>(settings.GeneExpressionCollection);
            _clinicalCollection = database.GetCollection<ClinicalSurvival>(settings.ClinicalDataCollection);
            _mergedCollection = database.GetCollection<GeneExpressionClinical>("gene_expression_clinical");

            CreateIndexes();
        }

        private void CreateIndexes()
        {
            var patientIdIndex = Builders<GeneExpression>.IndexKeys.Ascending(g => g.PatientId);
            _geneExpressionCollection.Indexes.CreateOne(new CreateIndexModel<GeneExpression>(patientIdIndex));

            var patientBarcodeIndex = Builders<ClinicalSurvival>.IndexKeys.Ascending(c => c.PatientBarcode);
            _clinicalCollection.Indexes.CreateOne(new CreateIndexModel<ClinicalSurvival>(patientBarcodeIndex));

            var mergedIndex = Builders<GeneExpressionClinical>.IndexKeys.Ascending(g => g.PatientId);
            _mergedCollection.Indexes.CreateOne(new CreateIndexModel<GeneExpressionClinical>(mergedIndex));
        }

        public async Task InsertGeneExpressionsAsync(List<GeneExpression> expressions)
        {
            if (expressions == null || !expressions.Any())
                return;

            // Use bulk write for efficiency
            var bulkOps = expressions.Select(expr =>
                new ReplaceOneModel<GeneExpression>(
                    Builders<GeneExpression>.Filter.Eq(g => g.PatientId, expr.PatientId),
                    expr)
                { IsUpsert = true }
            ).ToList();

            await _geneExpressionCollection.BulkWriteAsync(bulkOps);
        }

        public async Task InsertClinicalDataAsync(List<ClinicalSurvival> clinicalData)
        {
            if (clinicalData == null || !clinicalData.Any())
                return;

            var bulkOps = clinicalData.Select(data =>
                new ReplaceOneModel<ClinicalSurvival>(
                    Builders<ClinicalSurvival>.Filter.Eq(c => c.PatientBarcode, data.PatientBarcode),
                    data)
                { IsUpsert = true }
            ).ToList();

            await _clinicalCollection.BulkWriteAsync(bulkOps);
        }

        public async Task MergeClinicalWithGeneExpressionAsync()
        {
            // Get all gene expressions
            var expressions = await _geneExpressionCollection.Find(new BsonDocument()).ToListAsync();
            var clinical = await _clinicalCollection.Find(new BsonDocument()).ToListAsync();

            // Dictionary for quick lookup
            var clinicalDict = clinical.ToDictionary(c => c.PatientBarcode, c => c);

            // Create merged documents
            var mergedDocs = new List<GeneExpressionClinical>();
            foreach (var expr in expressions)
            {
                var merged = new GeneExpressionClinical
                {
                    PatientId = expr.PatientId,
                    CancerCohort = expr.CancerCohort,
                    GeneExpressions = expr.GeneExpressions,
                    SourceFile = expr.SourceFile,
                    UploadedAt = expr.UploadedAt
                };

                // Add clinical data if available
                if (clinicalDict.TryGetValue(expr.PatientId, out var clinicalData))
                {
                    merged.DiseaseSpecificSurvival = clinicalData.DiseaseSpecificSurvival;
                    merged.OverallSurvival = clinicalData.OverallSurvival;
                    merged.ClinicalStage = clinicalData.ClinicalStage;
                }

                mergedDocs.Add(merged);
            }

            // Insert merged documents
            if (mergedDocs.Any())
            {
                await _mergedCollection.DeleteManyAsync(new BsonDocument());
                await _mergedCollection.InsertManyAsync(mergedDocs);
            }
        }

        public async Task<List<GeneExpressionClinical>> GetPatientDataAsync(string patientId = null)
        {
            var filter = patientId != null
                ? Builders<GeneExpressionClinical>.Filter.Eq(g => g.PatientId, patientId)
                : Builders<GeneExpressionClinical>.Filter.Empty;

            return await _mergedCollection.Find(filter).ToListAsync();
        }

        public async Task<List<string>> GetUniquePatientsAsync()
        {
            return await _mergedCollection.Distinct(g => g.PatientId, new BsonDocument()).ToListAsync();
        }

        public async Task<List<string>> GetUniqueCancerCohortsAsync()
        {
            return await _mergedCollection.Distinct(g => g.CancerCohort, new BsonDocument()).ToListAsync();
        }
    }
}
