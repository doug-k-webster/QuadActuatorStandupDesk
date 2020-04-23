namespace QASDWebApi.Hubs
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.SignalR;

    using QASDCommon;

    public class DeskHub : Hub
    {
        public async Task SendDeskStatus(DeskStatus deskStatus)
        {
            await Clients.All.SendAsync("ReceiveDeskStatus", deskStatus);
        }
    }
}