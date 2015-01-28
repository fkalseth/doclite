using System;
using Newtonsoft.Json;

namespace Doclite
{
    public abstract class Document : IDocument
    {
        [JsonIgnore]
        public string Key { get; set; }

        [JsonIgnore]
        public DateTimeOffset Timestamp { get; set; }
    }
}