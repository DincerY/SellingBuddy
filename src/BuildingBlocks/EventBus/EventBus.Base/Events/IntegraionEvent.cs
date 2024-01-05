using Newtonsoft.Json;

namespace EventBus.Base.Events;


public class IntegraionEvent
{
    
    [JsonConstructor]
    public IntegraionEvent(Guid id, DateTime createdDate)
    {
        Id = id;
        CreatedDate = createdDate;
    }
        
    public IntegraionEvent()
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.Now;
    }
    [JsonProperty]
    public Guid Id { get; private set; }
    [JsonProperty]
    public DateTime CreatedDate { get; private set; }


}