using System;
using System.Collections.Generic;
using System.Text;

namespace QuadActuatorStandupDesk
{
    public class DeskStatus
    {
        public string Message { get; set; }

        public float Height { get; set; }

        public ActuatorStatus FrontLeftActuatorState { get; set; }

        public ActuatorStatus BackLeftActuatorState { get; set; }

        public ActuatorStatus BackRightActuatorState { get; set; }

        public ActuatorStatus FrontRightActuatorState { get; set; }
    }

    public class ActuatorStatus
    {
        public ActuatorState ActuatorState { get; set; }

        public float Height { get; set; }
    }
}
