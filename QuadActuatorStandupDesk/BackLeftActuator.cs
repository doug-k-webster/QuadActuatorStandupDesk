using System.Device.Gpio;

namespace QuadActuatorStandupDesk
{
    public class BackLeftActuator : Actuator
    {
        public BackLeftActuator(GpioController controller) : base(controller, nameof(BackLeftActuator), 26, 13)
        {
        }
    }
}
