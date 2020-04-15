namespace QuadActuatorStandupDesk
{
    using System;
    using System.Threading;

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
            this.BackRightActuator.Extend(progress);
            Thread.Sleep(5); 
            this.BackLeftActuator.Extend(progress);
            Thread.Sleep(5); 
            this.FrontLeftActuator.Extend(progress);
            Thread.Sleep(5); 
            this.FrontRightActuator.Extend(progress);
            progress?.Report(Log.Debug("desk going up"));
        }

        public void Down(IProgress<Log> progress)
        {
            progress?.Report(Log.Debug("asking desk to go down..."));
            this.BackRightActuator.Retract(progress);
            Thread.Sleep(5);
            this.BackLeftActuator.Retract(progress);
            Thread.Sleep(5); 
            this.FrontLeftActuator.Retract(progress);
            Thread.Sleep(5); 
            this.FrontRightActuator.Retract(progress);
            progress?.Report(Log.Debug("desk going down"));
        }

        public void Stop(IProgress<Log> progress)
        {
            progress?.Report(Log.Debug("stopping desk..."));
            this.BackLeftActuator.Stop(progress);
            Thread.Sleep(5); 
            this.FrontLeftActuator.Stop(progress);
            Thread.Sleep(5); 
            this.FrontRightActuator.Stop(progress);
            Thread.Sleep(5); 
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