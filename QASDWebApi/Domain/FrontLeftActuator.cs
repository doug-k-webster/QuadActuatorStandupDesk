using System;
using System.Device.Gpio;

namespace QuadActuatorStandupDesk
{
    public class FrontLeftActuator : Actuator
    {
        public FrontLeftActuator(GpioController controller) : base(controller, nameof(FrontLeftActuator), 21, 27)
        {
        }

        public override TimeSpan TimeToExtend => TimeSpan.FromSeconds(84.19);

        public override TimeSpan TimeToRetract => TimeSpan.FromSeconds(85.5);

    }
}
