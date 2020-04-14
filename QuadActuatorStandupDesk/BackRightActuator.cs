namespace QuadActuatorStandupDesk
{
    public class BackRightActuator : Actuator
    {
        public BackRightActuator(PigpiodIf pigpiodIf) : base(pigpiodIf, nameof(BackRightActuator), 20, 12)
        {
        }
    }
}
