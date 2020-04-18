namespace QuadActuatorStandupDesk
{
    public class ActuatorStatus
    {
        public ActuatorState ActuatorState { get; set; }

        public float Height { get; set; }

        public float DeviationFromAverage { get; set; }
    }
}
