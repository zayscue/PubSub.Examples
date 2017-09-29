﻿using System;

namespace Publisher.Core
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
                eventBus.Publish(new MessageIntegrationEvent(message));
                eventBus.Publish(new TestIntegrationEvent(new Test()));
            }
            //Environment.Exit(0);
        }

        public static string GetMessage(string[] args)
            => args.Length > 0 ? string.Join(" ", args) : "Hello World!";
    }
}
