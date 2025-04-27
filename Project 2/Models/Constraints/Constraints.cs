using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2.Models.Constraints
{
    public static class Constraints
    {
        public static readonly string[] TargetGenes = new string[]
          {
            "C6orf150", // cGAS
            "CCL5",
            "CXCL10",
            "TMEM173", // STING
            "CXCL9",
            "CXCL11",
            "NFKB1",
            "IKBKE",
            "IRF3",
            "TREX1",
            "ATM",
            "IL6",
            "IL8" // CXCL8
          };

        public static readonly string MinioEndpoint = "localhost:9000";
        public static readonly string MinioAccessKey = "admin";
        public static readonly string MinioSecretKey = "admin123";
        public static readonly string MinioBucketName = "tcga-data";

        public static readonly string XenaApiBaseUrl = "https://ucscpublic.xenahubs.net";
        public static readonly string[] DefaultCohorts = new string[]
        {
            "TCGA-BRCA", "TCGA-LUAD", "TCGA-LUSC", "TCGA-GBM", "TCGA-OV"
        };
    
}
}
