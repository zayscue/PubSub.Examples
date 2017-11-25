using System;

namespace Subscriber.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var eventBus = new EventBusRabbitMQ.EventBusRabbitMQ("localhost"))
            {
                var handler = new MessageIntegrationEventHandler();
                eventBus.Subscribe(handler);
                var testHandler = new TestIntegrationEventHandler();
                eventBus.Subscribe(testHandler);
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
