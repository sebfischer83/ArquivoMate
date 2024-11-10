using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Infrastructure.Identity
{
    public class CheckTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConnectionMultiplexer connectionMultiplexer;

        public CheckTokenMiddleware(RequestDelegate next, IConnectionMultiplexer connectionMultiplexer)
        {
            _next = next;
            this.connectionMultiplexer = connectionMultiplexer;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var authorizeMetadata = endpoint.Metadata.GetMetadata<AuthorizeAttribute>();

                if (authorizeMetadata != null)
                {
                    var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                    var db = connectionMultiplexer.GetDatabase();
                    var tokenExists = await db.KeyExistsAsync($"Revoke:{token}");
                    if (tokenExists)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
