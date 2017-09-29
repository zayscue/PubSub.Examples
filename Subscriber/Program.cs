using System;

namespace Subscriber
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var eventBus = new EventBusRabbitMQ.EventBusRabbitMQ("s99-rabbitmq.ecsdev.com"))
            {
                var handler = new MessageIntegrationEventHandler();
                eventBus.Subscribe(handler);
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
