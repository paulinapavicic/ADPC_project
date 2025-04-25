using Project_2.Models;
using Project_2.Services.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2.Services.Data
{
    public class ClinicalDataService
    {
        private readonly MongoService _mongoService;

        public ClinicalDataService(MongoService mongoService)
        {
            _mongoService = mongoService;
        }

        public async Task ImportClinicalDataAsync(string filePath)
        {
            try
            {
                var clinicalData = ParseClinicalDataTsv(filePath);
                await _mongoService.InsertClinicalDataAsync(clinicalData);
                await _mongoService.MergeClinicalWithGeneExpressionAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing clinical data: {ex.Message}");
                throw;
            }
        }

        private List<ClinicalSurvival> ParseClinicalDataTsv(string filePath)
        {
            var result = new List<ClinicalSurvival>();

            using var reader = new StreamReader(filePath);

            // Read header
            var headerLine = reader.ReadLine();
            var headers = headerLine.Split('\t');

            // Find column indices
            int barcodeIndex = Array.IndexOf(headers, "bcr_patient_barcode");
            int dssIndex = Array.IndexOf(headers, "DSS");
            int osIndex = Array.IndexOf(headers, "OS");
            int stageIndex = Array.IndexOf(headers, "clinical_stage");

            if (barcodeIndex == -1 || dssIndex == -1 || osIndex == -1 || stageIndex == -1)
                throw new Exception("Missing required columns in clinical data file");

            // Process data rows
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split('\t');
                if (parts.Length <= Math.Max(Math.Max(barcodeIndex, dssIndex), Math.Max(osIndex, stageIndex)))
                    continue;

                var barcode = parts[barcodeIndex];

                if (!int.TryParse(parts[dssIndex], out int dss))
                    dss = -1; // Unknown

                if (!int.TryParse(parts[osIndex], out int os))
                    os = -1; // Unknown

                result.Add(new ClinicalSurvival
                {
                    PatientBarcode = barcode,
                    DiseaseSpecificSurvival = dss,
                    OverallSurvival = os,
                    ClinicalStage = parts[stageIndex]
                });
            }

            return result;
        }
    }
}
