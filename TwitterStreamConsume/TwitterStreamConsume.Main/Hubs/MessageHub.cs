using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterStreamConsume.Main.Hubs
{
    public class MessageHub : Hub //<IMessageHub>
    {
        protected IHubContext<MessageHub> _context;

        public MessageHub(IHubContext<MessageHub> context)
        {
            _context = context;
        }

        public async Task SendMessage(string message)
        {
            await _context.Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task SendMQMessage(string messages)
        {
            await _context.Clients.All.SendAsync("ReceiveMQMessage", messages);
        }
    }
}
