using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventBus.Base;
using EventBus.Base.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using EnvironmentVariableTarget = System.EnvironmentVariableTarget;

namespace EventBus.RabbitMQ;

public class EventBusRabbitMQ : BaseEventBus
{
    private RabbitMQPersistentConnection persistentConnection;
    private readonly IConnectionFactory connectionFactory;
    private readonly IModel consumerChannel;
    public EventBusRabbitMQ(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
    {
        if (config.Connection != null)
        {
            var connJson = JsonConvert.SerializeObject(EventBusConfig.Connection, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            connectionFactory = JsonConvert.DeserializeObject<ConnectionFactory>(connJson);

        }
        else
        {
            connectionFactory = new ConnectionFactory();
        }
        
        persistentConnection = new RabbitMQPersistentConnection(connectionFactory, config.ConnectionRetryCount);

        consumerChannel = CreateConsumerChannel();
    }

    public override void Publish(IntegrationEvent @event)
    {
        throw new NotImplementedException();
    }

    public override void Subscribe<T, TH>()
    {
        var eventName = typeof(T).Name;
        eventName = ProcessEventName(eventName);

        SubsManager.HasSubscriptionsForEvent(eventName);

        if (!SubsManager.HasSubscriptionsForEvent(eventName))
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            consumerChannel.QueueDeclare(queue: GetSubName(eventName),
                durable:true,
                exclusive:false,
                autoDelete:false,
                arguments:null);

            consumerChannel.QueueBind(queue:GetSubName(eventName),
                exchange:EventBusConfig.DefaultTopicName,
                routingKey:eventName);
        }
    
    }

    public override void UnSubscribe<T, TH>()
    {
        throw new NotImplementedException();
    }


    private IModel CreateConsumerChannel()
    {
        if (!persistentConnection.IsConnected)
        {
            persistentConnection.TryConnect();
        }

        var channel = persistentConnection.CreateModel();

        channel.ExchangeDeclare(exchange:EventBusConfig.DefaultTopicName,type:"direct");

        return channel;
    }

}