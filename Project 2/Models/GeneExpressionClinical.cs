using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2.Models
{
   public class GeneExpressionClinical:GeneExpression
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("patient_id")]
        public string PatientId { get; set; }

        [BsonElement("cancer_cohort")]
        public string CancerCohort { get; set; }

        [BsonElement("gene_expressions")]
        public Dictionary<string, double> GeneExpressions { get; set; } = new Dictionary<string, double>();

        [BsonElement("dss")]
        public int? DiseaseSpecificSurvival { get; set; }

        [BsonElement("os")]
        public int? OverallSurvival { get; set; }

        [BsonElement("clinical_stage")]
        public string ClinicalStage { get; set; }

        [BsonElement("source_file")]
        public string SourceFile { get; set; }

        [BsonElement("uploaded_at")]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
