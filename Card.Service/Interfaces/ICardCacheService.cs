namespace Card.Service.Interfaces
{
    public interface ICardCacheService
    {
        T? Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan expiration);
    }
}