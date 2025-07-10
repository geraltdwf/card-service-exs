namespace Card.Service.Models.Exceptions
{
    public class CardNotFoundException : Exception
    {
        public CardNotFoundException(string userId, string cardNumber) : base($"Card with number {cardNumber} not found for user {userId}"){}
    }

    public class UnknownCardTypeException : Exception
    {
        public UnknownCardTypeException(string cardType) : base($"Unknow card type: {cardType}"){}
    }

     public class UnknownCardStatusException : Exception
    {
        public UnknownCardStatusException(string cardStatus) : base($"Card unknown status: {cardStatus}"){}
    }
}
