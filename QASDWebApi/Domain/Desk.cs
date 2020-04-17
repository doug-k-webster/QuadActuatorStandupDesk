namespace QuadActuatorStandupDesk
{
    using System;
    using System.Device.Gpio;
    
    public class Desk
    {
        public float LoweredHeightInches = 29.5f;

        private static Desk instance = null;

        private readonly GpioController controller;

        bool initialized = false;

        private Desk()
        {
            this.controller = new GpioController();
            this.BackLeftActuator = new BackLeftActuator(this.controller);
            this.FrontLeftActuator = new FrontLeftActuator(this.controller);
            this.FrontRightActuator = new FrontRightActuator(this.controller);
            this.BackRightActuator = new BackRightActuator(this.controller);
        }

        public BackLeftActuator BackLeftActuator { get; }

        public FrontLeftActuator FrontLeftActuator { get; }

        public FrontRightActuator FrontRightActuator { get; }

        public BackRightActuator BackRightActuator { get; }

        public float Height => LoweredHeightInches + (this.FrontLeftActuator.CurrentExtensionInches + this.BackLeftActuator.CurrentExtensionInches + this.BackRightActuator.CurrentExtensionInches + this.FrontRightActuator.CurrentExtensionInches) / 4;

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

        }

        public void Up(IProgress<Log> progress)
        {
            progress?.Report(Log.Debug("asking desk to go up..."));
            this.BackRightActuator.Extend(progress);
            this.BackLeftActuator.Extend(progress);
            this.FrontLeftActuator.Extend(progress);
            this.FrontRightActuator.Extend(progress);
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
            progress?.Report(Log.Debug("desk going down"));
        }

        public void Stop(IProgress<Log> progress)
        {
            progress?.Report(Log.Debug("stopping desk..."));
            this.BackLeftActuator.Stop(progress);
            this.FrontLeftActuator.Stop(progress);
            this.FrontRightActuator.Stop(progress);
            this.BackRightActuator.Stop(progress);
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