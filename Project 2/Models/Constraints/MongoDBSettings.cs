using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2.Models.Constraints
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; } = "mongodb+srv://ppavicic:Bax1407pp@cluster0.un5ewq6.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";
        public string DatabaseName { get; set; } = "tcga_database";
        public string GeneExpressionCollection { get; set; } = "gene_expressions";
        public string ClinicalDataCollection { get; set; } = "clinical_data";
    }
}
