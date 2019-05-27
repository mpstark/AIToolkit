using BattleTech;

namespace EnhancedAI.TurnOrderFactors
{
    public class IsUnstable : ITurnOrderFactor
    {
        public float EvaluateUnit(AbstractActor unit)
        {
            return unit.IsUnsteady ? 1f : 0f;
        }
    }
}
