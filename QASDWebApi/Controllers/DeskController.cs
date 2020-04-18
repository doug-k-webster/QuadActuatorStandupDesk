using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QASDCommon;
using QuadActuatorStandupDesk;

namespace QASDWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeskController : ControllerBase
    {
        private readonly ILogger<DeskController> logger;

        public DeskController(ILogger<DeskController> logger)
        {
            this.logger = logger;
                        Desk.Instance.Initialize(this.logger);
        }

        [HttpGet]
        [Route("down")]
        public string Down()
        {
            this.logger.LogInformation("Going down...");
            Desk.Instance.Down(this.logger);
            return "Ok";
        }

        [HttpGet]
        [Route("up")]
        public string Up()
        {
            this.logger.LogInformation("Going up...");
            Desk.Instance.Up(this.logger);
            return "Ok";
        }

        [HttpGet]
        [Route("stop")]
        public string Stop()
        {
            this.logger.LogInformation("Stopping...");
            Desk.Instance.Stop(this.logger);
            return "Ok";
        }

        [HttpGet]
        [Route("FrontLeftActuatorExtend")]

        public void FrontLeftActuatorExtend()
        {
            Desk.Instance.FrontLeftActuator.Extend(this.logger);
        }

        [HttpGet]
        [Route("FrontLeftActuatorRetract")]
        public void FrontLeftActuatorRetract()
        {
            Desk.Instance.FrontLeftActuator.Retract(this.logger);
        }

        [HttpGet]
        [Route("BackLeftActuatorExtend")]
        public void BackLeftActuatorExtend()
        {
            Desk.Instance.BackLeftActuator.Extend(this.logger);
        }

        [HttpGet]
        [Route("BackLeftActuatorRetract")]
        public void BackLeftActuatorRetract()
        {
            Desk.Instance.BackLeftActuator.Retract(this.logger);
        }

        [HttpGet]
        [Route("FrontRightActuatorExtend")]
        public void FrontRightActuatorExtend()
        {
            Desk.Instance.FrontRightActuator.Extend(this.logger);
        }

        [HttpGet]
        [Route("FrontRightActuatorRetract")]
        public void FrontRightActuatorRetract()
        {
            Desk.Instance.FrontRightActuator.Retract(this.logger);
        }

        [HttpGet]
        [Route("BackRightActuatorExtend")]
        public void BackRightActuatorExtend()
        {
            Desk.Instance.BackRightActuator.Extend(this.logger);
        }

        [HttpGet]
        [Route("BackRightActuatorRetract")]
        public void BackRightActuatorRetract()
        {
            Desk.Instance.BackRightActuator.Retract(this.logger);
        }

        [HttpGet]
        [Route("ExecuteCommand")]
        public ExecuteCommandResult ExecuteCommand(string commandText)
        {
            return Desk.Instance.ExecuteCommand(commandText, this.logger);
        }
    }
}
