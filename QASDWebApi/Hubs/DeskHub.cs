using Microsoft.AspNetCore.SignalR;
using QuadActuatorStandupDesk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QASDWebApi.Hubs
{
    public class DeskHub : Hub
    {
        public async Task SendDeskStatus(DeskStatus deskStatus)
        {
            await Clients.All.SendAsync("ReceiveDeskStatus", deskStatus);
        }
    }
}
