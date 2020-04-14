namespace QuadActuatorStandupDesk
{
    public class FrontLeftActuator : Actuator
    {
        public FrontLeftActuator(PigpiodIf pigpiodIf) : base(pigpiodIf, nameof(FrontLeftActuator), 21, 27)
        {
        }
    }
}
