using Card.Service.Models;

namespace Card.Service.Interfaces
{
    public interface IMatchingEngineService
    {
        IEnumerable<string> ExtractActions(CardDetails cardDetails);
        
    }
}