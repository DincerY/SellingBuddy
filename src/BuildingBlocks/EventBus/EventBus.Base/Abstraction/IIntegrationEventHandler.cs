using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventBus.Base.Events;

namespace EventBus.Base.Abstraction;

public interface IIntegrationEventHandler<TIntegrationEvent> : IntegrationEventHandler  where TIntegrationEvent : IntegraionEvent
{
    Task Handle(TIntegrationEvent @event);
}

public interface IntegrationEventHandler
{

}