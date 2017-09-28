using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBusRabbitMQ
{
    public class DefaultRabbitMQPersisterConnection : IRabbitMQPersistentConnection
    {
        private readonly IConnectionFactory _connectionFactory;

        IConnection _connection;
        bool _disposed;

        object sync_root = new object();

        public DefaultRabbitMQPersisterConnection(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));    
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _connection.Dispose();
        }

        public bool TryConnect()
        {
            lock(sync_root)
            {
                _connection = _connectionFactory.CreateConnection();
                if(IsConnected)
                {
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionBlocked += OnConnectionBlocked;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if(_disposed) return;
            TryConnect();
        }

        void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if(_disposed) return;
            TryConnect();
        }

        void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if(_disposed) return;
            TryConnect();
        }
    }
}