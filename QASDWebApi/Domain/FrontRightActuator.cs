using System;
using System.Device.Gpio;

namespace QuadActuatorStandupDesk
{
    public class FrontRightActuator : Actuator
    {
        public FrontRightActuator(GpioController controller) : base(controller, nameof(FrontRightActuator), 22, 19)
        {
        }

        public override TimeSpan TimeToExtend => TimeSpan.FromSeconds(93.3);

        public override TimeSpan TimeToRetract => TimeSpan.FromSeconds(90.9);
    }
}
