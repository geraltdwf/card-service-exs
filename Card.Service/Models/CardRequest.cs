using System.ComponentModel.DataAnnotations;

namespace Card.Service.Models
{
    public class CardRequest
    {
        [Required]
        public required string UserId { get; set; }
        [Required]
        public required string CardNumber { get; set; }
    }
}