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
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public MessageHub(IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();

            var groupName = GetGroupName(httpContext.User.Identity.Name, otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var messages = await unitOfWork.MessageRepository.GetMessageThread(httpContext.User.Identity.Name, otherUser);

            if (unitOfWork.HasChanges())
            {
                await unitOfWork.Complete();
            }
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

            var sender = await unitOfWork.UserRepository.GetUserByNameAsync(username);

            var recipient = await unitOfWork.UserRepository.GetUserByNameAsync(createMessageDTO.RecipientUsername);

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

            unitOfWork.MessageRepository.AddMessage(message);

            if (await unitOfWork.Complete())
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