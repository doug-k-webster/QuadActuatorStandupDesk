using QuadActuatorStandupDesk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace QASDWin
{
    public class DeskClient
    {
        private readonly HttpClient httpClient;

        public DeskClient()
        {
            this.httpClient = new HttpClient();
        }


        internal Task Initialize(Progress<Log> progress)
        {
            // no op
            return Task.CompletedTask;
        }

        internal Task<ClientDeskStatus> Up(Progress<Log> progress) => this.SimpleDeskAction("up");

        internal Task<ClientDeskStatus> Down(Progress<Log> progress) => this.SimpleDeskAction("down");

        internal Task<ClientDeskStatus> Stop(Progress<Log> progress) => this.SimpleDeskAction("stop");

        internal Task<ClientDeskStatus> FrontLeftActuatorExtend(Progress<Log> progress) => this.SimpleDeskAction("FrontLeftActuatorExtend");

        internal Task<ClientDeskStatus> FrontLeftActuatorRetract(Progress<Log> progress) => this.SimpleDeskAction("FrontLeftActuatorRetract");

        internal Task<ClientDeskStatus> BackLeftActuatorExtend(Progress<Log> progress) => this.SimpleDeskAction("BackLeftActuatorExtend");

        internal Task<ClientDeskStatus> BackLeftActuatorRetract(Progress<Log> progress) => this.SimpleDeskAction("BackLeftActuatorRetract");

        internal Task<ClientDeskStatus> FrontRightActuatorExtend(Progress<Log> progress) => this.SimpleDeskAction("FrontRightActuatorExtend");

        internal Task<ClientDeskStatus> FrontRightActuatorRetract(Progress<Log> progress) => this.SimpleDeskAction("FrontRightActuatorRetract");

        internal Task<ClientDeskStatus> BackRightActuatorExtend(Progress<Log> progress) => this.SimpleDeskAction("BackRightActuatorExtend");

        internal Task<ClientDeskStatus> BackRightActuatorRetract(Progress<Log> progress) => this.SimpleDeskAction("BackRightActuatorRetract");

        internal Task<ClientDeskStatus> ExecuteCommand(string commandText, Progress<Log> progress) => this.SimpleDeskAction($"ExecuteCommand?commandText=\"{commandText}\"");

        public async Task<ClientDeskStatus> SimpleDeskAction(string action)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"http://192.168.1.10:9999/desk/{action}");
            request.Headers.Add("User-Agent", "QASDWin.DeskClient");

            var response = await this.httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                //var deskStatus = await JsonSerializer.DeserializeAsync
                //    <DeskStatus>(responseStream);

                return new ClientDeskStatus
                {
                    //DeskStatus = deskStatus
                };
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                return new ClientDeskStatus
                {
                    HttpFault = true,
                    HttpErrorMessage = content
                };
            }
        }
    }

    public class ClientDeskStatus
    {
        public DeskStatus DeskStatus { get; set; }

        public bool HttpFault { get; set; }

        public string HttpErrorMessage { get; set; }
    }
}
