using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2.Models
{
    public class ClinicalSurvival
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("bcr_patient_barcode")]
        public string PatientBarcode { get; set; }

        [BsonElement("dss")]
        public int DiseaseSpecificSurvival { get; set; }  // 1 - survived, 0 - did not survive

        [BsonElement("os")]
        public int OverallSurvival { get; set; }  // 1 - survived, 0 - did not survive

        [BsonElement("clinical_stage")]
        public string ClinicalStage { get; set; }
    }
}
