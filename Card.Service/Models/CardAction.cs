using Card.Service.Models.Enums;

namespace Card.Service.Models
{
   
    public class CardAction{
        public string ActionType { get; set; } = string.Empty;
        public CardType CardType { get; set; }
        public CardStatus CardStatus { get; set; }
        public bool IsPinSet { get; set; }
    }
}