using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2.Database
{
    public class XenaDataset
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("longTitle")]
        public string LongTitle { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("dataSubType")]
        public string DataSubType { get; set; }

        [JsonProperty("fileType")]
        public string FileType { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
