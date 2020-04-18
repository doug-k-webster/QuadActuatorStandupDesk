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
        private readonly ILogger logger;
        private readonly CancellationToken _cancellationToken;
        private readonly IHubContext<DeskHub> deskHubContext;
        private readonly Progress<Log> progress;

        public DeskMonitor(
            ILogger<DeskMonitor> logger,
            IHostApplicationLifetime applicationLifetime,
            IHubContext<DeskHub> deskHubContext)
        {
            this.logger = logger;
            this.progress = new Progress<Log>();
            this.progress.ProgressChanged += Progress_ProgressChanged;
            this._cancellationToken = applicationLifetime.ApplicationStopping;
            this.deskHubContext = deskHubContext;
        }

        public void StartMonitorLoop()
        {
            logger.LogInformation("Desk Monitor is starting...");

            // Run a console user input loop in a background thread
            Task.Run(() => Monitor());
        }

        public async Task Monitor()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                // report desk status to the signal r hub
                //QuadActuatorStandupDesk.Desk.Instance.BackLeftActuator.CurrentExtensionInches;
                

                Desk.Instance.CorrectDeviatingActuators(this.progress);

                var deskStatus = Desk.Instance.GetStatus();
                //this.logger.LogInformation($"back left: {deskStatus.BackLeftActuatorState.Height}");
                //this.logger.LogInformation($"front left: {deskStatus.FrontLeftActuatorState.Height}");
                //this.logger.LogInformation($"back left: {deskStatus.BackRightActuatorState.Height}");
                //this.logger.LogInformation($"front right: {deskStatus.FrontRightActuatorState.Height}");
                //this.logger.LogInformation($"front right extension speed: {Desk.Instance.FrontRightActuator.ExtensionSpeedInchesPerSecond}");
                //this.logger.LogInformation($"front right time to extend milliseconds: {Desk.Instance.FrontRightActuator.TimeToExtend.Milliseconds}");
                //this._logger.LogInformation($"DeskMonitor: height: {deskStatus.Height}");
                await this.deskHubContext.Clients.All.SendAsync("ReceiveDeskStatus", deskStatus);
                await Task.Delay(200);
            }
        }

        private void Progress_ProgressChanged(object sender, Log e)
        {
            switch (e.LogLevel)
            {
                case 0:
                    this.logger.LogDebug(e.Text);
                    break;
                case 1:
                    this.logger.LogInformation(e.Text);
                    break;
                case 2:
                    this.logger.LogWarning(e.Text);
                    break;
                case 3:
                    this.logger.LogError(e.Text);
                    break;
                case 4:
                    this.logger.LogCritical(e.Text);
                    break;
                default:
                    this.logger.LogInformation(e.Text);
                    break;
            }
        }
    }
}
