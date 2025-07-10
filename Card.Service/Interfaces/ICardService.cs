namespace Card.Service.Interfaces
{
    public interface ICardService
    {
        Task<IEnumerable<string>> GetAllowedActions(string userId, string cardNumber);
    }
}