using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker presenceTracker;

        public PresenceHub(PresenceTracker presenceTracker)
        {
            this.presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            await presenceTracker.UserConnected(Context.User.Identity.Name, Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOnline", Context.User.Identity.Name);

            var currentUsers = await presenceTracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await presenceTracker.UserDisconnected(Context.User.Identity.Name, Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOffline", Context.User.Identity.Name);



            var currentUsers = await presenceTracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

            await base.OnDisconnectedAsync(exception);
        }
    }
}