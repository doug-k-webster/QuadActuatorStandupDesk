using System.Device.Gpio;

namespace QuadActuatorStandupDesk
{
    public class FrontRightActuator : Actuator
    {
        public FrontRightActuator(GpioController pigpiodIf) : base(pigpiodIf, nameof(FrontRightActuator), 22, 19)
        {
        }
    }
}
