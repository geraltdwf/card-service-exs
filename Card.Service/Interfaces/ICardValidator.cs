using Card.Service.Models;

namespace Card.Service.Interfaces
{
    public interface ICardValidator
    {
        void Validate(CardDetails card);
    }
}