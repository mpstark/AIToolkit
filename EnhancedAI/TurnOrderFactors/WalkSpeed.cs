using BattleTech;

namespace EnhancedAI.TurnOrderFactors
{
    public class WalkSpeed : ITurnOrderFactor
    {
        public float EvaluateUnit(AbstractActor unit)
        {
            return unit.MovementCaps.MaxWalkDistance;
        }
    }
}
