namespace QASDWebApi.Domain
{
    using System;
    using System.Device.Gpio;

    public class BackLeftActuator : Actuator
    {
        public BackLeftActuator(GpioController controller)
            : base(controller, nameof(BackLeftActuator), 26, 13)
        {
        }

        public override TimeSpan TimeToExtend => TimeSpan.FromSeconds(86.6);

        public override TimeSpan TimeToRetract => TimeSpan.FromSeconds(88.3);
    }
}