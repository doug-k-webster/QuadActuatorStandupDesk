namespace QuadActuatorStandupDesk
{
    public class FrontRightActuator : Actuator
    {
        public FrontRightActuator(PigpiodIf pigpiodIf) : base(pigpiodIf, nameof(FrontRightActuator), 22, 19)
        {
        }
    }
}
