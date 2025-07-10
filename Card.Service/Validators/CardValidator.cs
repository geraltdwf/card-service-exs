using Card.Service.Interfaces;
using Card.Service.Models;
using Card.Service.Models.Enums;
using Card.Service.Models.Exceptions;

namespace Card.Service.Validators
{
    public class CardValidator : ICardValidator
    {
        public void Validate(CardDetails card)
        {
            if(!Enum.IsDefined(typeof(CardType), card.CardType))
                throw new UnknownCardTypeException(card.CardType.ToString());

            if(!Enum.IsDefined(typeof(CardStatus), card.CardStatus))
                throw new UnknownCardStatusException(card.CardStatus.ToString());
        }
    }
}