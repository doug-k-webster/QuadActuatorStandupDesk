using System;
using System.Collections.Generic;
using System.Text;

namespace QuadActuatorStandupDesk
{
    public abstract class Actuator
    {
        private readonly PigpiodIf pigpiodIf;

        protected Actuator(PigpiodIf pigpiodIf,string name, uint blackWirePin, uint redWirePin)
        {
            this.pigpiodIf = pigpiodIf;
            Name = name;
            this.BlackWirePin = blackWirePin;
            this.RedWirePin = redWirePin;
        }

        public string Name { get; }

        public uint BlackWirePin { get; }

        public uint RedWirePin { get; }

        public void Extend(IProgress<Log> progress)
        {
            progress?.Report(Log.Debug($"asking {this.Name} actuator to extend..."));
            pigpiodIf.gpio_write(this.RedWirePin, PigpiodIf.PI_HIGH);
            pigpiodIf.gpio_write(this.BlackWirePin, PigpiodIf.PI_LOW);
            progress?.Report(Log.Debug($"{this.Name} actuator is extendenting."));
        }

        public void Retract(IProgress<Log> progress)
        {
            progress?.Report(Log.Debug($"asking {this.Name} actuator to retract..."));
            pigpiodIf.gpio_write(this.RedWirePin, PigpiodIf.PI_LOW);
            pigpiodIf.gpio_write(this.BlackWirePin, PigpiodIf.PI_HIGH);
            progress?.Report(Log.Debug($"{this.Name} actuator is retracting."));
        }

        public void Stop(IProgress<Log> progress)
        {
            progress?.Report(Log.Debug($"asking {this.Name} actuator to stop..."));
            pigpiodIf.gpio_write(this.RedWirePin, PigpiodIf.PI_HIGH);
            pigpiodIf.gpio_write(this.BlackWirePin, PigpiodIf.PI_HIGH);
            progress?.Report(Log.Debug($"{this.Name} actuator is stopped."));
        }
    }
}
