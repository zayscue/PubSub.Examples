using System;
using RabbitMQ.Client;
using Models;

namespace Publisher.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "logs", type: "fanout");

                var message = new Message
                {
                    Type = MessageType.Info,
                    Description = GetMessage(args)
                };
                channel.BasicPublish(exchange: "logs", routingKey: "", basicProperties: null, body: message.Serialize());
                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        public static string GetMessage(string[] args)
            => args.Length > 0 ? string.Join(" ", args) : "info: Hello World!";
    }
}
