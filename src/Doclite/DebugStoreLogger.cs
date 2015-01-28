using System.Diagnostics;

namespace Doclite
{
    public class DebugStoreLogger : IStoreLogger
    {
        public void Info(string message, params object[] arguments)
        {
            Debug.WriteLine(message, arguments);
        }
    }
}