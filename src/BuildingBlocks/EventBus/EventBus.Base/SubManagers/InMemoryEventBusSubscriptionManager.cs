using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventBus.Base.Abstraction;
using EventBus.Base.Events;

namespace EventBus.Base.SubManagers;

public class InMemoryEventBusSubscriptionManager : IEventBusSubscriptionManager
{
    private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
    private readonly List<Type> _eventTypes;

    public event EventHandler<string> OnEventRemoved;
    public Func<string, string> eventNameGetter;


    public bool IsEmpty { get; }
    public void AddSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
    {
        throw new NotImplementedException();
    }

    public void RemoveSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
    {
        throw new NotImplementedException();
    }

    public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent
    {
        throw new NotImplementedException();
    }

    public bool HasSubscriptionsForEvent(string eventName)
    {
        throw new NotImplementedException();
    }

    public Type GetEventTypeByName(string eventName)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
    {
        throw new NotImplementedException();
    }

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName)
    {
        throw new NotImplementedException();
    }

    public string GetEventKey<T>()
    {
        throw new NotImplementedException();
    }
}