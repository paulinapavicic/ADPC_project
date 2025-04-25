using Newtonsoft.Json;
using Project_2.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2.Models.Database
{
    public class XenaCohort
    {
        [JsonProperty("cohort")]
        public string CohortName { get; set; }

        [JsonProperty("datasets")]
        public List<XenaDataset> Datasets { get; set; } = new List<XenaDataset>();
    }
}
