using System;
using System.Device.Gpio;

namespace QuadActuatorStandupDesk
{
    public abstract class Actuator
    {
        private readonly GpioController controller;

        protected Actuator(GpioController pigpiodIf,string name, int blackWirePin, int redWirePin)
        {
            this.controller = pigpiodIf;
            Name = name;
            this.BlackWirePin = blackWirePin;
            this.RedWirePin = redWirePin;
        }

        public string Name { get; }

        public int BlackWirePin { get; }

        public int RedWirePin { get; }

        public void Extend(IProgress<Log> progress)
        {
            progress?.Report(Log.Debug($"asking {this.Name} actuator to extend..."));
            controller.Write(this.RedWirePin, PinValue.High);
            controller.Write(this.BlackWirePin, PinValue.Low);
            progress?.Report(Log.Debug($"{this.Name} actuator is extendenting."));
        }

        public void Retract(IProgress<Log> progress)
        {
            progress?.Report(Log.Debug($"asking {this.Name} actuator to retract..."));
            controller.Write(this.RedWirePin, PinValue.Low);
            controller.Write(this.BlackWirePin, PinValue.High);
            progress?.Report(Log.Debug($"{this.Name} actuator is retracting."));
        }

        public void Stop(IProgress<Log> progress)
        {
            progress?.Report(Log.Debug($"asking {this.Name} actuator to stop..."));
            controller.Write(this.RedWirePin, PinValue.High);
            controller.Write(this.BlackWirePin, PinValue.High);
            progress?.Report(Log.Debug($"{this.Name} actuator is stopped."));
        }

        public void Initialize(IProgress<Log> progress)
        {
            progress?.Report(Log.Debug($"initializing {this.Name} actuator..."));
            this.controller.OpenPin(this.RedWirePin, PinMode.Output);
            this.controller.OpenPin(this.BlackWirePin, PinMode.Output);
        }
    }
}
