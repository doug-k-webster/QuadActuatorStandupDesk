namespace QuadActuatorStandupDesk
{
    using System;

    public class Desk
    {
        private readonly PigpiodIf pigpiodIf;
                
        public Desk()
        {
            this.pigpiodIf = new PigpiodIf();
            this.BackLeftActuator = new BackLeftActuator(this.pigpiodIf);
            this.FrontLeftActuator = new FrontLeftActuator(this.pigpiodIf);
            this.FrontRightActuator = new FrontRightActuator(this.pigpiodIf);
            this.BackRightActuator = new BackRightActuator(this.pigpiodIf);
        }

        public BackLeftActuator BackLeftActuator { get; }

        public FrontLeftActuator FrontLeftActuator { get; }

        public FrontRightActuator FrontRightActuator { get; }

        public BackRightActuator BackRightActuator { get; }

        public void Initialize(IProgress<Log> progress)
        {
            pigpiodIf.pigpio_start("192.168.1.10", "8888", progress);
        }

        public void Up(IProgress<Log> progress)
        {
            progress?.Report(Log.Debug("asking desk to go up..."));
            this.BackLeftActuator.Extend(progress);
            this.FrontLeftActuator.Extend(progress);
            this.FrontRightActuator.Extend(progress);
            this.BackRightActuator.Extend(progress);
            progress?.Report(Log.Debug("desk going up"));
        }

        public void Down(IProgress<Log> progress)
        {
            progress?.Report(Log.Debug("asking desk to go down..."));
            this.BackLeftActuator.Retract(progress);
            this.FrontLeftActuator.Retract(progress);
            this.FrontRightActuator.Retract(progress);
            this.BackRightActuator.Retract(progress);
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
    }
}