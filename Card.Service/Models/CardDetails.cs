using Card.Service.Models.Enums;

namespace Card.Service.Models
{
    public record CardDetails(string CardNumber, CardType CardType, CardStatus CardStatus, bool IsPinSet);
}