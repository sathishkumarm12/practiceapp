using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageRepository messageRepository;
        private readonly IMapper mapper;
        private readonly IUserRepository userRepository;

        public MessageHub(IMessageRepository messageRepository, IMapper mapper, IUserRepository userRepository)
        {
            this.messageRepository = messageRepository;
            this.mapper = mapper;
            this.userRepository = userRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();

            var groupName = GetGroupName(httpContext.User.Identity.Name, otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var messages = await messageRepository.GetMessageThread(httpContext.User.Identity.Name, otherUser);

            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task CreateMessage(CreateMessageDTO createMessageDTO)
        {
            var username = Context.User.Identity.Name;

            if (username == createMessageDTO.RecipientUsername)
                throw new HubException("Yoou cannot sent message to self");

            var sender = await userRepository.GetUserByNameAsync(username);

            var recipient = await userRepository.GetUserByNameAsync(createMessageDTO.RecipientUsername);

            if (recipient == null)
                throw new HubException("Recipient not found");

            var message = new Message()
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDTO.Content
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);

            // if (!string.IsNullOrEmpty(groupName))
            // {
            //     message.DateRead = DateTime.Now;
            // }

            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync())
            {

                await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDTO>(message));
            }
            else
                throw new HubException("Failed to save message");
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) > 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}