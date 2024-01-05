using EventBus.Base.Events;
namespace EventBus.Base.Abstraction;

public interface IEventBus
{
    void Publish(IntegraionEvent @event);

    void Subscribe<T, TH>() where T : IntegraionEvent where TH : IIntegrationEventHandler<T>;

    void UnSubscribe<T, TH>() where T : IntegraionEvent where TH : IIntegrationEventHandler<T>;




}