using ArquivoMate.Application;
using ArquivoMate.Application.Interfaces;
using ArquivoMate.Shared;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Infrastructure.Services.Communication
{
    public class SignalRCommunicationHub : Hub
    {
        private readonly ILogger<SignalRCommunicationHub> logger;

        public SignalRCommunicationHub(ILogger<SignalRCommunicationHub> logger)
        {
            this.logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            logger.LogDebug("Client connected: {0}", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
            {
                logger.LogError(exception, "Client disconnected with exception: {0}", Context.ConnectionId);
            }
            else
                logger.LogDebug("Client disconnected: {0}", Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }
    }

    public class SignalRCommunicationService : ICommunicationHub
    {
        private readonly IHubContext<SignalRCommunicationHub> hubContext;

        public SignalRCommunicationService(IHubContext<SignalRCommunicationHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task SendDocumentStatus(string connectionId, string documentId, HubResponse<DocumentProcessingFinishedData> status)
        {
            await hubContext.Clients.All.SendAsync("DocumentStatus", documentId, status);
        }
    }
}
