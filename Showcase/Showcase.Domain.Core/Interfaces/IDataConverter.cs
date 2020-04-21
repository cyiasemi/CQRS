namespace Showcase.Domain.Core.Interfaces
{
    public interface IDataConverter<T> where T : new()
    {
        T Deserialize(string data);
        T Deserialize(byte[] data);
    }
}
