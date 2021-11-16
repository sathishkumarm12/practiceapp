using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUsers
            = new Dictionary<string, List<string>>();

        public Task UserConnected(string username, string userId)
        {
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(userId);
                }
                else
                {
                    OnlineUsers.Add(username, new List<string> { userId });
                }
            }

            return Task.CompletedTask;
        }

        public Task UserDisconnected(string username, string userId)
        {

            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Remove(userId);

                    if (OnlineUsers[username].Count == 0)
                        OnlineUsers.Remove(username);
                }

            }

            return Task.CompletedTask;
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] userList;

            lock (OnlineUsers)
            {
                userList = OnlineUsers.Keys.ToArray();
            }

            return Task.FromResult(userList);
        }
    }
}