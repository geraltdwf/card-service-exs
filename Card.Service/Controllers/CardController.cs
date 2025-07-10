using Asp.Versioning;
using Card.Service.Interfaces;
using Card.Service.Models;
using Card.Service.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Card.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;
        private readonly ILogger<CardController> _logger;

        public CardController(ICardService cardService, ILogger<CardController> logger){
            _cardService = cardService;
            _logger = logger;
        }

        [HttpGet("actions")]
        [ProducesResponseType(typeof(CardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllowedActions([FromQuery] CardRequest request){
            try{
                _logger.LogInformation("Getting allowed actions for user {UserId} and card {CardNumber}", request.UserId, request.CardNumber);
                var cardDetails = await _cardService.GetAllowedActions(request.UserId, request.CardNumber);
                return Ok(new CardResponse{AllowedActions = cardDetails});
            }
            catch(CardNotFoundException ex){
                _logger.LogError(ex, "Card not found for user {UserId} and card {CardNumber}", request.UserId, request.CardNumber);
                return BadRequest(new ErrorResponse(){
                    Message = ex.Message,
                    ErrorCode = ErrorCodes.CardNotFound
                });
            }
            catch(UnknownCardTypeException ex){
                _logger.LogError(ex, "Unknown card type for user {UserId} and card {CardNumber}", request.UserId, request.CardNumber);
                return BadRequest(new ErrorResponse(){
                    Message = ex.Message,
                    ErrorCode = ErrorCodes.InvalidCardType
                });
            }
            catch(UnknownCardStatusException ex){
                _logger.LogError(ex, "Unknown card status for user {UserId} and card {CardNumber}", request.UserId, request.CardNumber);
                return BadRequest(new ErrorResponse(){
                    Message = ex.Message,
                    ErrorCode = ErrorCodes.InvalidCardStatus
                });
            }
        }
    }
}