namespace Card.Service.Models
{
    public class CardResponse
    {
        public required IEnumerable<string> AllowedActions { get; set; }
    }
}