using ArquivoMate.Application.Interfaces;
using MassTransit;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Infrastructure.Services.MessageQueue
{
    public class MassTransitMessageQueue<T> : IMessageQueue<T> where T : class
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitMessageQueue(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task EnqueueMessage(T message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            await _publishEndpoint.Publish(message);
        }
    }
}
