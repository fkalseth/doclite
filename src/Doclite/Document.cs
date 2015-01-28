using System;
using Newtonsoft.Json;

namespace Doclite
{
    public abstract class IDocument
    {
        [JsonIgnore]
        public string Key { get; set; }

        [JsonIgnore]
        public DateTime Timestamp { get; set; }

        public string ETag { get; set; }
    }
}