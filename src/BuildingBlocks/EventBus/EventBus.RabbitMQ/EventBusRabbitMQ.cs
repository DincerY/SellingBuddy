﻿using System.Net.Sockets;
using System.Text;
using EventBus.Base;
using EventBus.Base.Events;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

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
            //var connJson = JsonConvert.SerializeObject(EventBusConfig.Connection, new JsonSerializerSettings()
            //{
            //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
               
            //});

            //connectionFactory = JsonConvert.DeserializeObject<ConnectionFactory>(connJson);

            connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri(config.Connection.ToString())
            };

        }
        //Eğer bir connection girilmezse bu kısım çalışacak
        else
        {
            connectionFactory = new ConnectionFactory();
        }
        
        persistentConnection = new RabbitMQPersistentConnection(connectionFactory, config.ConnectionRetryCount);

        consumerChannel = CreateConsumerChannel();

        SubsManager.OnEventRemoved += SubsManager_OnEventRemoved;
    }

    

    public override void Publish(IntegrationEvent @event)
    {
        if (!persistentConnection.IsConnected)
        {
            persistentConnection.TryConnect();
        }

        var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(EventBusConfig.ConnectionRetryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (ex, time) =>
                {
                    //logging
                });
         
        var eventName = @event.GetType().Name;
        eventName = ProcessEventName(eventName);

        consumerChannel.ExchangeDeclare(exchange:EventBusConfig.DefaultTopicName,type:"direct");


        var message = JsonConvert.SerializeObject(@event);
        var body = Encoding.UTF8.GetBytes(message);

        policy.Execute(() =>
        {
            var properties = consumerChannel.CreateBasicProperties();
            properties.DeliveryMode = 2; //Persistent

            //consumerChannel.QueueDeclare(queue: GetSubName(eventName),
            //    durable: true,
            //    exclusive: false,
            //    autoDelete: false,
            //    arguments: null);

            //consumerChannel.QueueBind(queue: GetSubName(eventName),
            //    exchange: EventBusConfig.DefaultTopicName,
            //    routingKey: eventName);

            consumerChannel.BasicPublish(
                exchange: EventBusConfig.DefaultTopicName,
                routingKey: eventName,
                mandatory: true,
                basicProperties: properties,
                body: body);
        });

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

        SubsManager.AddSubscription<T,TH>();
        StartBasicConsume(eventName);
    
    }

    public override void UnSubscribe<T, TH>()
    {
        SubsManager.RemoveSubscription<T,TH>();
    }

    private void SubsManager_OnEventRemoved(object? sender, string eventName)
    {
        eventName = ProcessEventName(eventName);
        if (!persistentConnection.IsConnected)
        {
            persistentConnection.TryConnect();
        }

        consumerChannel.QueueUnbind(queue: eventName,
            exchange: EventBusConfig.DefaultTopicName,
            routingKey: eventName);

        if (SubsManager.IsEmpty)
        {
            consumerChannel.Close();
        }
    }

    private IModel CreateConsumerChannel()
    {
        if (!persistentConnection.IsConnected)
        {
            persistentConnection.TryConnect();
        }

        IModel channel = persistentConnection.CreateModel();

        channel.ExchangeDeclare(exchange:EventBusConfig.DefaultTopicName,type:"direct");

        return channel;
    }

    private void StartBasicConsume(string eventName)
    {
        if (consumerChannel != null)
        {
            var consumer = new EventingBasicConsumer(consumerChannel);

            consumer.Received += Consumer_Received;

            consumerChannel.BasicConsume(
                queue: GetSubName(eventName),
                autoAck: false,
                consumer: consumer);
        }
    }

    private async void Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
    {
        var eventName = eventArgs.RoutingKey;
        eventName = ProcessEventName(eventName);
        var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

        try
        {
            await ProcessEvent(eventName, message);
        }
        catch (Exception e)
        {
            //logging
        }

        consumerChannel.BasicAck(eventArgs.DeliveryTag,multiple:false);
    }

}