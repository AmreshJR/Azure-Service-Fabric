/*
 * <Your-Product-Name>
 * Copyright (c) <Year-From>-<Year-To> <Your-Company-Name>
 *
 * Please configure this header in your SonarCloud/SonarQube quality profile.
 * You can also set it in SonarLint.xml additional file for SonarLint or standalone NuGet analyzer.
 */
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NotificationService.BusinessRules.Hub;

namespace NotificationService.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class ChatHub : Hub
    {
        private readonly ConnectionMapping _connections;

        public ChatHub(ConnectionMapping connections)
        {
            _connections = connections; 
        }

        public override Task OnConnectedAsync()
        {
            string? userId = Context.User?.Claims.Where(x => x.Type == "UserLoginId").Select(x => x.Value).FirstOrDefault();
            if(!string.IsNullOrEmpty(userId))
            {
                _connections.Add(userId, Context.ConnectionId);
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            string? userId = Context.User?.Claims.Where(x => x.Type == "UserLoginId").Select(x => x.Value).FirstOrDefault();
            if(!string.IsNullOrEmpty(userId))
            {
                _connections.Remove(userId, Context.ConnectionId);
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
