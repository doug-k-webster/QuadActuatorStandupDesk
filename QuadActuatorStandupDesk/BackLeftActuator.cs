namespace QuadActuatorStandupDesk
{
    public class BackLeftActuator : Actuator
    {
        public BackLeftActuator(PigpiodIf pigpiodIf) : base(pigpiodIf, nameof(BackLeftActuator), 26, 13)
        {
        }
    }
}
