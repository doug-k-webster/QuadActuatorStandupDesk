using System;
using System.Collections.Generic;
using System.Text;

namespace QuadActuatorStandupDesk
{
    public class DeskStatus
    {
        public string Message { get; set; }

        public float Height { get; set; }

        public DeskState DeskState { get; set; }

        public ActuatorStatus FrontLeftActuatorState { get; set; }

        public ActuatorStatus BackLeftActuatorState { get; set; }

        public ActuatorStatus BackRightActuatorState { get; set; }

        public ActuatorStatus FrontRightActuatorState { get; set; }
    }
}
