using System;
using Newtonsoft.Json;

namespace Doclite
{
    public abstract class Document : IDocument
    {
        public string Key { get; set; }

        public DateTimeOffset Timestamp { get; set; }
    }
}