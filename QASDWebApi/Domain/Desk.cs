namespace QuadActuatorStandupDesk
{
    using Microsoft.Extensions.Logging;
    using QASDCommon;
    using System;
    using System.Collections.Generic;
    using System.Device.Gpio;
    using System.Linq;

    public class Desk
    {
        public const float LoweredHeightInches = 29.5f;

        public const float RaisedHeightInches = 47f;

        public const float MaxAcuatorDeviationAllowed = 0.25f;

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

        public void Initialize(ILogger logger)
        {
            if(this.initialized)
            {
                return;
            }

            this.initialized = true;
            this.BackRightActuator.Initialize(logger);
            this.BackLeftActuator.Initialize(logger);
            this.FrontLeftActuator.Initialize(logger);
            this.FrontRightActuator.Initialize(logger);
            this.DeskState = DeskState.Stopped;
        }

        internal void CorrectDeviatingActuators(ILogger logger)
        {
            if (this.DeskState == DeskState.Raising)
            {
                foreach(var actuator in this.actuators.Select(kvp=>kvp.Value))
                {
                    if (this.GetActuatorDeviation(actuator) > MaxAcuatorDeviationAllowed  && actuator.ActuatorState == ActuatorState.Extending)
                    {
                        logger?.LogInformation($"{actuator.Name} actuator is deviating up, correcting...");
                        actuator.Stop(logger);
                    }

                    if (this.GetActuatorDeviation(actuator) < 0 && actuator.ActuatorState == ActuatorState.Stopped)
                    {
                        logger?.LogInformation($"{actuator.Name} actuator has resumed extending...");
                        actuator.Extend(logger);
                    }
                }

                if (this.Height > RaisedHeightInches + 1)
                {
                    logger?.LogInformation($"desk fully raised. stopping...");
                    this.Stop(logger);
                    this.SetHeight(logger, RaisedHeightInches);
                }
            }

            if (this.DeskState == DeskState.Lowering)
            {
                foreach (var actuator in this.actuators.Select(kvp => kvp.Value))
                {
                    if (this.GetActuatorDeviation(actuator) < -MaxAcuatorDeviationAllowed && actuator.ActuatorState == ActuatorState.Retracting)
                    {
                        logger?.LogInformation($"{actuator.Name} actuator is deviating down, correcting...");
                        actuator.Stop(logger);
                    }

                    if (this.GetActuatorDeviation(actuator) > 0 && actuator.ActuatorState == ActuatorState.Stopped)
                    {
                        logger?.LogInformation($"{actuator.Name} actuator has resumed retracting...");
                        actuator.Retract(logger);
                    }
                }

                if (this.Height < LoweredHeightInches - 1)
                {
                    logger?.LogInformation($"desk fully lowered. stopping...");
                    this.Stop(logger);
                    this.SetHeight(logger, LoweredHeightInches);
                }
            }
        }

        internal float GetActuatorDeviation(Actuator actuator) => actuator.CurrentExtensionInches + LoweredHeightInches - this.Height;

        public void Up(ILogger logger)
        {
            logger?.LogDebug("asking desk to go up...");
            this.BackRightActuator.Extend(logger);
            this.BackLeftActuator.Extend(logger);
            this.FrontLeftActuator.Extend(logger);
            this.FrontRightActuator.Extend(logger);
            this.DeskState = DeskState.Raising;
            logger?.LogDebug("desk going up");
        }

        internal DeskStatus GetStatus() => new DeskStatus
        {
            FrontLeftActuatorState = this.FrontLeftActuator.GetStatus(this.AverageActuatorExtension),
            BackLeftActuatorState = this.BackLeftActuator.GetStatus(this.AverageActuatorExtension),
            BackRightActuatorState = this.BackRightActuator.GetStatus(this.AverageActuatorExtension),
            FrontRightActuatorState = this.FrontRightActuator.GetStatus(this.AverageActuatorExtension),
            Height = this.Height,
            DeskState = this.DeskState
        };

        public void Down(ILogger logger)
        {
            logger?.LogDebug("asking desk to go down...");
            this.BackRightActuator.Retract(logger);
            this.BackLeftActuator.Retract(logger);
            this.FrontLeftActuator.Retract(logger);
            this.FrontRightActuator.Retract(logger);
            this.DeskState = DeskState.Lowering;
            logger?.LogDebug("desk going down");
        }

        public void Stop(ILogger logger)
        {
            logger?.LogDebug("stopping desk...");
            this.BackLeftActuator.Stop(logger);
            this.FrontLeftActuator.Stop(logger);
            this.FrontRightActuator.Stop(logger);
            this.BackRightActuator.Stop(logger);
            this.DeskState = DeskState.Stopped;
            logger?.LogDebug("desk stopped");
        }

        public ExecuteCommandResult ExecuteCommand(string commandText, ILogger logger)
        {
            var result = new ExecuteCommandResult();

            // just a simple command interpreter here
            if (commandText.ToUpperInvariant() == "UP")
            {
                this.Up(logger);
            }
            else if (commandText.ToUpperInvariant() == "DOWN")
            {
                this.Down(logger);
            }
            else if (commandText.ToUpperInvariant() == "STOP")
            {
                this.Stop(logger);
            }
            else if (commandText.ToUpperInvariant().StartsWith("SETHEIGHT "))
            {
                var commandParts = commandText.Split(" ");

                if (commandParts.Length != 2)
                {
                    result.IsError = true;
                    result.Message = "The setheight command should have a single parameter between 29.5 and 47 inclusive";
                    logger.LogWarning(result.Message);
                    return result;
                }

                var heightPart = commandParts[1];

                if (!float.TryParse(heightPart, out var height))
                {
                    result.IsError = true;
                    result.Message = $"Could not parse the height value {heightPart} as a float";
                    logger.LogWarning(result.Message);
                    return result;
                }

                if (height < 29.5f || height > 47f)
                {
                    result.IsError = true;
                    result.Message = $"The height value {height} is outside the acceptable range of 29.5 to 47";
                    logger.LogWarning(result.Message);
                    return result;
                }

                this.SetHeight(logger, height);
                result.Message = $"Height changed to {height}";
                logger.LogWarning(result.Message);
                return result;
            }
            else
            {
                result.IsError = true;
                result.Message = $"Unknown command: {commandText}";
                logger.LogWarning(result.Message);
            }

            return result;
        }

        private void SetHeight(ILogger progress, float height)
        {
            foreach(var actuator in this.actuators)
            {
                actuator.Value.SetExtension(progress, height - LoweredHeightInches);
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