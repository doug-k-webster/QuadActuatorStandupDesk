using Microsoft.Extensions.Logging;
using System;
using System.Device.Gpio;
using System.Diagnostics;

namespace QuadActuatorStandupDesk
{
    public abstract class Actuator
    {
        public const float MaximumExtensionInches = 18.0f;

        private readonly GpioController controller;

        private Stopwatch extensionStopwatch = new Stopwatch();

        private Stopwatch retractionStopwatch = new Stopwatch();

        private float startingExtension = 0;

        protected Actuator(GpioController controller,string name, int blackWirePin, int redWirePin)
        {
            this.controller = controller;
            this.Name = name;
            this.BlackWirePin = blackWirePin;
            this.RedWirePin = redWirePin;
        }

        public string Name { get; }

        public int BlackWirePin { get; }

        public int RedWirePin { get; }

        public ActuatorState ActuatorState { get; private set; }

        public abstract TimeSpan TimeToExtend { get; }

        public abstract TimeSpan TimeToRetract { get; }

        public float ExtensionSpeedInchesPerSecond => MaximumExtensionInches * 1000 / (float)this.TimeToExtend.TotalMilliseconds;

        public float RetractionSpeedInchesPerSecond => MaximumExtensionInches * 1000 / (float)this.TimeToRetract.TotalMilliseconds;

        public float CurrentExtensionInches => this.startingExtension + (this.extensionStopwatch.ElapsedMilliseconds * this.ExtensionSpeedInchesPerSecond / 1000)
            - (this.retractionStopwatch.ElapsedMilliseconds * this.RetractionSpeedInchesPerSecond / 1000);
        
        public void Extend(ILogger logger)
        {
            logger?.LogDebug($"asking {this.Name} actuator to extend...");
            controller.Write(this.RedWirePin, PinValue.High);
            controller.Write(this.BlackWirePin, PinValue.Low);
            this.extensionStopwatch.Start();
            this.retractionStopwatch.Stop();
            this.ActuatorState = ActuatorState.Extending;
            logger?.LogDebug($"{this.Name} actuator is extendenting.");
        }

        public void Retract(ILogger logger)
        {
            logger?.LogDebug($"asking {this.Name} actuator to retract...");
            controller.Write(this.RedWirePin, PinValue.Low);
            controller.Write(this.BlackWirePin, PinValue.High);
            this.extensionStopwatch.Stop();
            this.retractionStopwatch.Start();
            this.ActuatorState = ActuatorState.Retracting;
            logger?.LogDebug($"{this.Name} actuator is retracting.");
        }

        public void Stop(ILogger logger)
        {
            logger?.LogDebug($"asking {this.Name} actuator to stop...");
            controller.Write(this.RedWirePin, PinValue.High);
            controller.Write(this.BlackWirePin, PinValue.High);
            this.extensionStopwatch.Stop();
            this.retractionStopwatch.Stop();
            this.ActuatorState = ActuatorState.Stopped;
            logger?.LogDebug($"{this.Name} actuator is stopped.");
        }

        public void Initialize(ILogger logger)
        {
            logger?.LogDebug($"initializing {this.Name} actuator...");
            this.controller.OpenPin(this.RedWirePin, PinMode.Output);
            this.controller.OpenPin(this.BlackWirePin, PinMode.Output);
            this.ActuatorState = ActuatorState.Stopped;
        }

        internal ActuatorStatus GetStatus(float averageActuatorExtension) => new ActuatorStatus
        {
            ActuatorState = this.ActuatorState,
            Height = this.CurrentExtensionInches,
            DeviationFromAverage = this.CurrentExtensionInches - averageActuatorExtension
        };

        internal void SetExtension(ILogger progress, float extension)
        {
            this.Stop(progress);
            this.startingExtension = extension;
            this.extensionStopwatch.Reset();
            this.retractionStopwatch.Reset();
        }
    }
}
