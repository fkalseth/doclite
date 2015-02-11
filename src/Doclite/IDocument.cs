using System;
using Newtonsoft.Json;

namespace Doclite
{
    public interface IDocument
    {
        [JsonIgnore]
        string Key { get; set; }

        [JsonIgnore]
        DateTimeOffset Timestamp { get; set; }
    }
}