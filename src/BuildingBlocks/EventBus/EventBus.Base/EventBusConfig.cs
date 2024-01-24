
namespace EventBus.Base;


public class EventBusConfig
{
    public int ConnectionRetryCount { get; set; } = 5;
    public string DefaultTopicName { get; set; } = "SellingBuddyEventBus";
    public string EventBusConnectionString { get; set; } = String.Empty;
    public string SubscriberClientAppName { get; set; } = String.Empty;
    public string EventNamePrefix { get; set; } = String.Empty;
    public string EventNameSuffix { get; set; } = String.Empty;
    public EventBusType EventBusType { get; set; } = EventBusType.RabbitMQ;
    //object olmasının sebebi diğer servislerde bu class ı uygulayacağı zaman bağımlı kütüphaneler
    //onlar içinde yüklenmiş olacak.
    public object Connection { get; set; }

    public bool DeleteEventPrefix => !String.IsNullOrEmpty(EventNamePrefix);
    public bool DeleteEventSuffix => !String.IsNullOrEmpty(EventNameSuffix);

}

public enum EventBusType
{
    RabbitMQ = 0,
    AzureServiceBus = 1
}
