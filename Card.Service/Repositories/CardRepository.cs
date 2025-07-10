using Card.Service.Interfaces;
using Card.Service.Models;
using Card.Service.Models.Enums;

namespace Card.Service.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly ILogger<CardRepository> _logger;
        private readonly Dictionary<string, Dictionary<string, CardDetails>> _userCards;
        private static Dictionary<string, Dictionary<string, CardDetails>> CreateSampleUserCards(){
          var userCards = new Dictionary<string, Dictionary<string, CardDetails>>();
          for(var i = 1; i <= 3; i++){
            var cards = new Dictionary<string, CardDetails>();
            var cardIndex = 1;
            foreach(CardType cardType in Enum.GetValues(typeof(CardType))){
                foreach(CardStatus cardStatus in Enum.GetValues(typeof(CardStatus))){
                    var cardNumber = $"Card{i}{cardIndex}";
                    cards.Add(cardNumber,
                    new CardDetails(
                    CardNumber: cardNumber,
                    CardType: cardType,
                    CardStatus: cardStatus,
                    IsPinSet: cardIndex % 2 == 0));
                    cardIndex++;
                }
            }
            var userId = $"User{i}";
            userCards.Add(userId, cards);
          }
          return userCards;
        }

       

        public CardRepository(ILogger<CardRepository> logger,  Dictionary<string, Dictionary<string, CardDetails>>? userCard = null)

        {
            _logger = logger;
            _userCards = userCard ?? CreateSampleUserCards();
        }


        public async Task<CardDetails?> GetCardDetails(string userId, string cardNumber)
        {
            _logger.LogInformation("Getting card details for user {UserId} and card {CardNumber}", userId, cardNumber);
            await Task.Delay(1000);

            if (!_userCards.TryGetValue(userId, out var cards)
                || !cards.TryGetValue(cardNumber, out var cardDetails))
                return null;

            return cardDetails;
        }
        
    }
}