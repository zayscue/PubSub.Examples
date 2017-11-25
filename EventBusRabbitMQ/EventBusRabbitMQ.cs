using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventBus.Abstractions;
using EventBus.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBusRabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        const string BROKER_NAME = "direct_logs";
        
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly Dictionary<string, List<IIntegrationEventHandler>> _handlers;
        private readonly List<Type> _eventTypes;
        
        private IModel _consumerChannel;
        private string _queueName; 

        public EventBusRabbitMQ(string connectionString) 
        {
            if(string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException($"{nameof(connectionString)} can not be null, empty, or whitespace.");
            _persistentConnection = new DefaultRabbitMQPersisterConnection(new ConnectionFactory { HostName = connectionString, UserName = "user", Password = "password" });
            _handlers = new Dictionary<string, List<IIntegrationEventHandler>>();
            _eventTypes = new List<Type>();
            _consumerChannel = CreateConsumerChannel();
        }

        public void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using (var channel = _persistentConnection.CreateModel())
            {
                var eventName = @event.GetType()
                    .Name;

                channel.ExchangeDeclare(exchange: BROKER_NAME,
                                    type: "direct");

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: BROKER_NAME,
                                     routingKey: eventName,
                                     basicProperties: null,
                                     body: body);
            }
        }

        public void Subscribe<T>(IIntegrationEventHandler<T> handler) where T : IntegrationEvent
        {
            var eventName = typeof(T).Name;
            if(_handlers.ContainsKey(eventName))
            {
                _handlers[eventName].Add(handler);
            }
            else
            {
                if(!_persistentConnection.IsConnected)
                {
                    _persistentConnection.TryConnect();
                }

                using(var channel = _persistentConnection.CreateModel())
                {
                    channel.QueueBind(queue: _queueName,
                                        exchange: BROKER_NAME,
                                        routingKey: eventName);
                }
                _handlers.Add(eventName, new List<IIntegrationEventHandler>());
                _handlers[eventName].Add(handler);
                _eventTypes.Add(typeof(T));
            }
        }

        public void Unsubscribe<T>(IIntegrationEventHandler<T> handler) where T : IntegrationEvent
        {
            if(handler == null) return;
            var eventName = typeof(T).Name;
            _handlers[eventName].Remove(handler);
            var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
            if(eventType != null)
            {
                _eventTypes.Remove(eventType);
            }
        }

        private IModel CreateConsumerChannel()
        {
            if(!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");

            _queueName = channel.QueueDeclare().QueueName;

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) => 
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);
                await ProcessEvent(eventName, message);
            };

            channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
            channel.CallbackException += (sender, ea) => 
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };

            return channel;
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if(_handlers.ContainsKey(eventName))
            {
                var subscriptions = _handlers[eventName];
                foreach(var subscription in subscriptions)
                {
                    var eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
                    var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                    await (Task)concreteType.GetMethod("Handle").Invoke(subscription, new object[] { integrationEvent });
                }
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if(_consumerChannel != null)
                    {
                        _consumerChannel.Dispose();
                    }
                    if (_persistentConnection != null)
                    {
                        _persistentConnection.Dispose();
                    }
                    _handlers.Clear();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~EventBusRabbitMQ() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
