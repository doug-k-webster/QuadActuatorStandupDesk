using System.Device.Gpio;

namespace QuadActuatorStandupDesk
{
    public class FrontLeftActuator : Actuator
    {
        public FrontLeftActuator(GpioController pigpiodIf) : base(pigpiodIf, nameof(FrontLeftActuator), 21, 27)
        {
        }
    }
}
