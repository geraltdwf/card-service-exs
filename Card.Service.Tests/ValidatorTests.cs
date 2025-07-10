using Card.Service.Interfaces;
using Card.Service.Models;
using Card.Service.Models.Enums;
using Card.Service.Models.Exceptions;
using Card.Service.Repositories;
using Card.Service.Validators;
using Microsoft.Extensions.Logging;
using Moq;

namespace Card.Service.Tests
{
    public class ValidatorTests 
    {
        private readonly ICardValidator _cardValidator;
        public ValidatorTests()
        {
            _cardValidator = new CardValidator();
        }

        [Fact]
        public void GetCardDetails_Should_ThrowUnknownCardStatusException_When_CardTypeIsInvalid()
        {
        var invalidCardStatus= (CardStatus)991;
           Assert.ThrowsAny<UnknownCardStatusException>(() =>_cardValidator.Validate(new CardDetails("Card999", CardType.Credit, invalidCardStatus, true)));
        }

        [Fact]
        public void GetCardDetails_Should_ThrowUnknownCardTypeException_When_CardStatusIsInvalid()
        {
            var invalidCardType = (CardType)999;
            Assert.ThrowsAny<UnknownCardTypeException>(() => _cardValidator.Validate(new CardDetails("Card123", invalidCardType, CardStatus.Blocked, true)));
        }

        [Fact]
        public void GetCardDetails_Should_NotThrowException_When_CardStatusIsValid()
        {
            var cardDetails = new CardDetails("Card123", CardType.Credit, CardStatus.Blocked, true);
            var exception = Record.Exception(() => _cardValidator.Validate(cardDetails));
            Assert.Null(exception);
        }

    }
}