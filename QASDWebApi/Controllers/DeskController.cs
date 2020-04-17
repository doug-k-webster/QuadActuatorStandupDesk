using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuadActuatorStandupDesk;

namespace QASDWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeskController : ControllerBase
    {
        private readonly ILogger<DeskController> logger;

        private readonly Progress<Log> progress;

        public DeskController(ILogger<DeskController> logger)
        {
            this.logger = logger;
            this.progress = new Progress<Log>();
            this.progress.ProgressChanged += Progress_ProgressChanged;
            Desk.Instance.Initialize(this.progress);
        }

        private void Progress_ProgressChanged(object sender, Log e)
        {
            switch(e.LogLevel)
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

        [HttpGet]
        [Route("down")]
        public string Down()
        {
            this.logger.LogInformation("Going down...");
            Desk.Instance.Down(this.progress);
            return "Ok";
        }

        [HttpGet]
        [Route("up")]
        public string Up()
        {
            this.logger.LogInformation("Going up...");
            Desk.Instance.Up(this.progress);
            return "Ok";
        }

        [HttpGet]
        [Route("stop")]
        public string Stop()
        {
            this.logger.LogInformation("Stopping...");
            Desk.Instance.Stop(this.progress);
            return "Ok";
        }

        [HttpGet]
        [Route("FrontLeftActuatorExtend")]

        public void FrontLeftActuatorExtend()
        {
            Desk.Instance.FrontLeftActuator.Extend(this.progress);
        }

        [HttpGet]
        [Route("FrontLeftActuatorRetract")]
        public void FrontLeftActuatorRetract()
        {
            Desk.Instance.FrontLeftActuator.Retract(this.progress);
        }

        [HttpGet]
        [Route("BackLeftActuatorExtend")]
        public void BackLeftActuatorExtend()
        {
            Desk.Instance.BackLeftActuator.Extend(this.progress);
        }

        [HttpGet]
        [Route("BackLeftActuatorRetract")]
        public void BackLeftActuatorRetract()
        {
            Desk.Instance.BackLeftActuator.Retract(this.progress);
        }

        [HttpGet]
        [Route("FrontRightActuatorExtend")]
        public void FrontRightActuatorExtend()
        {
            Desk.Instance.FrontRightActuator.Extend(this.progress);
        }

        [HttpGet]
        [Route("FrontRightActuatorRetract")]
        public void FrontRightActuatorRetract()
        {
            Desk.Instance.FrontRightActuator.Retract(this.progress);
        }

        [HttpGet]
        [Route("BackRightActuatorExtend")]
        public void BackRightActuatorExtend()
        {
            Desk.Instance.BackRightActuator.Extend(this.progress);
        }

        [HttpGet]
        [Route("BackRightActuatorRetract")]
        public void BackRightActuatorRetract()
        {
            Desk.Instance.BackRightActuator.Retract(this.progress);
        }

        [HttpGet]
        [Route("ExecuteCommand")]
        public void ExecuteCommand(string commandText)
        {
            Desk.Instance.ExecuteCommand(commandText, this.progress);
        }
    }
}
