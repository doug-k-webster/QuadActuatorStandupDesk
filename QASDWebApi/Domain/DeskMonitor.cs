using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QASDWebApi.Hubs;
using QuadActuatorStandupDesk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QASDWebApi.Domain
{
   
    public class DeskMonitor
    {
        private readonly ILogger _logger;
        private readonly CancellationToken _cancellationToken;
        private readonly IHubContext<DeskHub> deskHubContext;

        public DeskMonitor(
            ILogger<DeskMonitor> logger,
            IHostApplicationLifetime applicationLifetime,
            IHubContext<DeskHub> deskHubContext)
        {
            this._logger = logger;
            this._cancellationToken = applicationLifetime.ApplicationStopping;
            this.deskHubContext = deskHubContext;
        }

        public void StartMonitorLoop()
        {
            _logger.LogInformation("Desk Monitor is starting...");

            // Run a console user input loop in a background thread
            Task.Run(() => Monitor());
        }

        public async Task Monitor()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                // report desk status to the signal r hub
                //QuadActuatorStandupDesk.Desk.Instance.BackLeftActuator.CurrentExtensionInches;
                var deskStatus = Desk.Instance.GetStatus();
                //this._logger.LogInformation($"DeskMonitor: height: {deskStatus.Height}");
                await this.deskHubContext.Clients.All.SendAsync("ReceiveDeskStatus", deskStatus);
                await Task.Delay(200);
            }
        }
    }
}
