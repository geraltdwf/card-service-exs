using Card.Service.Interfaces;
using Card.Service.Models;
using RulesEngine.Models;

namespace Card.Service.Services
{
    public class MatchingEngineService : IMatchingEngineService
    {
       private readonly RulesEngine.RulesEngine _rulesEngine;
       private readonly ILogger<MatchingEngineService> _logger;

       public MatchingEngineService(RulesEngine.RulesEngine rulesEngine, ILogger<MatchingEngineService> logger)
       {
            _rulesEngine = rulesEngine;
            _logger = logger;
       }

       public IEnumerable<string> ExtractActions(CardDetails cardDetails){
            _logger.LogInformation($"Executing rules for card {cardDetails.CardType } | {cardDetails.CardStatus} | {cardDetails.IsPinSet}");
            List<RuleResultTree> resultList = _rulesEngine.ExecuteAllRulesAsync("ActionsRules",
            new { CardType = (int)cardDetails.CardType,
                    CardStatus = (int)cardDetails.CardStatus,
                    IsPinSet = cardDetails.IsPinSet}).Result;

            foreach (var result in resultList)
            _logger.LogInformation($"Rule: {result.Rule.RuleName}, Success: {result.IsSuccess}, Error: {result.ExceptionMessage}");

            var actions = resultList.Where(result => result.IsSuccess).Select(result => result.Rule.SuccessEvent);
            return actions;
       }
    }
}