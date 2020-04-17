using System.Device.Gpio;

namespace QuadActuatorStandupDesk
{
    public class BackRightActuator : Actuator
    {
        public BackRightActuator(GpioController pigpiodIf) : base(pigpiodIf, nameof(BackRightActuator), 20, 12)
        {
        }
    }
}
