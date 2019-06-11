using BattleTech;

namespace AIToolkit.TurnOrderFactors
{
    public interface ITurnOrderFactor
    {
        float EvaluateUnit(AbstractActor unit);
    }
}
