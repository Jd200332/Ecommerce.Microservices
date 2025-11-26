using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ECommerce.MessageBus
{
    public class RabbitMQMessageBus : IMessageBus
    {
        private readonly IConnection connection;
        private readonly IModel channel;

        public RabbitMQMessageBus(string hostName)
        {
            var factory = new ConnectionFactory { HostName = hostName };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        public async Task PublishAsync<T>(T message, string queueName) where T : class
        {
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
            await Task.CompletedTask;
        }

        public async Task SubscribeAsync<T>(string queueName, Func<T, Task> handler) where T : class
        {
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<T>(json);
                await handler(message);
            };

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            await Task.CompletedTask;
        }
    }
}

