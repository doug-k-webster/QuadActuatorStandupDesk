namespace QuadActuatorStandupDesk
{
    using System;
    using System.Collections.Generic;
    using System.Device.Gpio;
    using System.Linq;

    public class Desk
    {
        public const float LoweredHeightInches = 29.5f;

        public const float RaisedHeightInches = 47f;

        public const float MaxAcuatorDeviationAllowed = 0.5f;

        private static Desk instance = null;

        private readonly GpioController controller;

        private readonly Dictionary<Type, Actuator> actuators;

        bool initialized = false;

        private Desk()
        {
            this.controller = new GpioController();
            this.actuators = new Dictionary<Type, Actuator>
            {
                { typeof(BackLeftActuator), new BackLeftActuator(this.controller) },
                { typeof(FrontLeftActuator), new FrontLeftActuator(this.controller) },
                { typeof(FrontRightActuator), new FrontRightActuator(this.controller) },
                { typeof(BackRightActuator), new BackRightActuator(this.controller) }
            };
        }

        public Actuator BackLeftActuator => this.actuators[typeof(BackLeftActuator)];

        public Actuator FrontLeftActuator => this.actuators[typeof(FrontLeftActuator)];

        public Actuator FrontRightActuator => this.actuators[typeof(FrontRightActuator)];

        public Actuator BackRightActuator => this.actuators[typeof(BackRightActuator)];

        public float Height => LoweredHeightInches + AverageActuatorExtension;

        public float AverageActuatorExtension => this.actuators.Average(a => a.Value.CurrentExtensionInches);

        public DeskState DeskState { get; private set; }

        public void Initialize(IProgress<Log> progress)
        {
            if(this.initialized)
            {
                return;
            }

            this.initialized = true;
            this.BackRightActuator.Initialize(progress);
            this.BackLeftActuator.Initialize(progress);
            this.FrontLeftActuator.Initialize(progress);
            this.FrontRightActuator.Initialize(progress);
            this.DeskState = DeskState.Stopped;
        }

        internal void CorrectDeviatingActuators(IProgress<Log> progress)
        {
            if (this.DeskState == DeskState.Raising)
            {
                foreach(var actuator in this.actuators.Select(kvp=>kvp.Value))
                {
                    if (this.GetActuatorDeviation(actuator) > MaxAcuatorDeviationAllowed  && actuator.ActuatorState == ActuatorState.Extending)
                    {
                        progress?.Report(Log.Warn($"{actuator.Name} actuator is deviating up, correcting..."));
                        actuator.Stop(progress);
                    }

                    if (this.GetActuatorDeviation(actuator) < 0 && actuator.ActuatorState == ActuatorState.Stopped)
                    {
                        actuator.Extend(progress);
                    }
                }

                if (this.Height > RaisedHeightInches)
                {
                    progress?.Report(Log.Info($"desk fully raised. stopping..."));
                    this.Stop(progress);
                }
            }

            if (this.DeskState == DeskState.Lowering)
            {
                foreach (var actuator in this.actuators.Select(kvp => kvp.Value))
                {
                    if (this.GetActuatorDeviation(actuator) < -MaxAcuatorDeviationAllowed && actuator.ActuatorState == ActuatorState.Retracting)
                    {
                        progress?.Report(Log.Warn($"{actuator.Name} actuator is deviating down, correcting..."));
                        actuator.Stop(progress);
                    }

                    if (this.GetActuatorDeviation(actuator) > 0 && actuator.ActuatorState == ActuatorState.Stopped)
                    {
                        actuator.Retract(progress);
                    }
                }

                if (this.Height < LoweredHeightInches)
                {
                    progress?.Report(Log.Info($"desk fully lowered. stopping..."));
                    this.Stop(progress);
                }
            }
        }

        internal float GetActuatorDeviation(Actuator actuator) => actuator.CurrentExtensionInches + LoweredHeightInches - this.Height;

        public void Up(IProgress<Log> progress)
        {
            progress?.Report(Log.Debug("asking desk to go up..."));
            this.BackRightActuator.Extend(progress);
            this.BackLeftActuator.Extend(progress);
            this.FrontLeftActuator.Extend(progress);
            this.FrontRightActuator.Extend(progress);
            this.DeskState = DeskState.Raising;
            progress?.Report(Log.Debug("desk going up"));
        }

        internal DeskStatus GetStatus() => new DeskStatus
        {
            FrontLeftActuatorState = this.FrontLeftActuator.GetStatus(),
            BackLeftActuatorState = this.BackLeftActuator.GetStatus(),
            BackRightActuatorState = this.BackRightActuator.GetStatus(),
            FrontRightActuatorState = this.FrontRightActuator.GetStatus(),
            Height = this.Height
        };

        public void Down(IProgress<Log> progress)
        {
            progress?.Report(Log.Debug("asking desk to go down..."));
            this.BackRightActuator.Retract(progress);
            this.BackLeftActuator.Retract(progress);
            this.FrontLeftActuator.Retract(progress);
            this.FrontRightActuator.Retract(progress);
            this.DeskState = DeskState.Lowering;
            progress?.Report(Log.Debug("desk going down"));
        }

        public void Stop(IProgress<Log> progress)
        {
            progress?.Report(Log.Debug("stopping desk..."));
            this.BackLeftActuator.Stop(progress);
            this.FrontLeftActuator.Stop(progress);
            this.FrontRightActuator.Stop(progress);
            this.BackRightActuator.Stop(progress);
            this.DeskState = DeskState.Stopped;
            progress?.Report(Log.Debug("desk stopped"));
        }

        public void ExecuteCommand(string commandText, IProgress<Log> progress)
        {
            // just a simple command interpreter here
            if (commandText == "up")
            {
                this.Up(progress);
            }
            else if (commandText == "down")
            {
                this.Down(progress);
            }
            else if (commandText == "stop")
            {
                this.Stop(progress);
            }
            else
            {
                progress.Report(Log.Warn($"Unknown command: {commandText}"));
            }
        }

        public static Desk Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Desk();
                }

                return instance;
            }
        }
    }
}