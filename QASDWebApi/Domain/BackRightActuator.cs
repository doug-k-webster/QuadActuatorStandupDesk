namespace QASDWebApi.Domain
{
    using System;
    using System.Device.Gpio;

    public class BackRightActuator : Actuator
    {
        public BackRightActuator(GpioController controller)
            : base(controller, nameof(BackRightActuator), 20, 12)
        {
        }

        public override TimeSpan TimeToExtend => TimeSpan.FromSeconds(99.9);

        public override TimeSpan TimeToRetract => TimeSpan.FromSeconds(99.5);
    }
}