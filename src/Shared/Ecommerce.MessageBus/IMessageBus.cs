using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;  
using RabbitMQ.Client.Events;

namespace ECommerce.MessageBus
{
    public interface IMessageBus
    {
        Task PublishAsync<T>(T message, string queueName) where T : class;
        Task SubscribeAsync<T>(string queueName, Func<T, Task> handler) where T : class;
    }
}
