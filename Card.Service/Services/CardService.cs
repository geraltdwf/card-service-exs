using Card.Service.Interfaces;
using Card.Service.Models.Exceptions;

namespace Card.Service.Services
{
    public class CardService : ICardService
    {
       private readonly ICardRepository _cardRepository;
       private readonly ILogger<CardService> _logger;
       private readonly ICardCacheService _cardCacheService;
       private readonly IMatchingEngineService _matchingEngineService;
       private readonly ICardValidator _cardValidator;

       
       public CardService(ICardRepository cardRepository, IMatchingEngineService matchingEngineService,
        ICardCacheService cardCacheService,ICardValidator cardValidator, ILogger<CardService> logger)
       {
            _cardRepository = cardRepository;
            _matchingEngineService = matchingEngineService;
            _cardCacheService = cardCacheService;
            _cardValidator = cardValidator;
            _logger = logger;
       }

        public async Task<IEnumerable<string>> GetAllowedActions(string userId, string cardNumber)
        {

            var cacheKey = $"{userId}_{cardNumber}";
            var cachedActions = _cardCacheService.Get<List<string>>(cacheKey);
            if(cachedActions is not null)
            {
                _logger.LogInformation($"Returning cached actions for user {userId} and card {cardNumber}");
                return cachedActions;
            }

            var cardDetails = await _cardRepository.GetCardDetails(userId, cardNumber);
            if(cardDetails is null)
                throw new CardNotFoundException(userId, cardNumber);

            _cardValidator.Validate(cardDetails);

            var actions = _matchingEngineService.ExtractActions(cardDetails).ToList();
            _cardCacheService.Set(cacheKey,actions, TimeSpan.FromMinutes(1));
            if(!actions.Any())
                _logger.LogInformation($"No actions found for user {userId} and card {cardNumber}");

            return actions;
        }

       
    }
}