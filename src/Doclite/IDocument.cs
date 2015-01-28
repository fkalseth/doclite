using System;

namespace Doclite
{
    public interface IDocument
    {
        string Key { get; set; }
        DateTimeOffset Timestamp { get; set; }
    }
}