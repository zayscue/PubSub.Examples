namespace Publisher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var eventBus = new EventBusRabbitMQ.EventBusRabbitMQ("s99-rabbitmq.ecsdev.com"))
            {
                var message = new Models.Message
                {
                    Type = Models.MessageType.Info,
                    Description = GetMessage(args)
                };
                for (var i = 0; i < 100; i++)
                {
                    eventBus.Publish(new MessageIntegrationEvent(message));
                    eventBus.Publish(new TestIntegrationEvent(new Test()));
                }
            }
        }

        public static string GetMessage(string[] args)
            => args.Length > 0 ? string.Join(" ", args) : "info: Hello World!";
    }
}
