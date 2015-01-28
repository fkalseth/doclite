namespace Doclite
{
    public interface IStoreLogger
    {
        void Info(string message, params object[] arguments);
    }
}