using BattleTech;

namespace EnhancedAI.TurnOrderFactors
{
    public interface ITurnOrderFactor
    {
        float EvaluateUnit(AbstractActor unit);
    }
}
