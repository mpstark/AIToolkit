using BattleTech;

namespace AIToolkit.TurnOrderFactors
{
    public class WalkSpeed : ITurnOrderFactor
    {
        public float EvaluateUnit(AbstractActor unit)
        {
            return unit.MovementCaps.MaxWalkDistance;
        }
    }
}
