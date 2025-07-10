using Card.Service.Models;

namespace Card.Service.Interfaces
{
    public interface ICardRepository
    {
        Task<CardDetails?> GetCardDetails(string userId, string cardNumber);
    }
}