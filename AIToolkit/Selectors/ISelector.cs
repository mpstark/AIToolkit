namespace AIToolkit.Selectors
{
    public interface ISelector<T>
    {
        bool Select(string selectString, T obj);
    }
}
