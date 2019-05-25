using BattleTech;

namespace EnhancedAI.UnitSelectors
{
    public interface IUnitSelector
    {
        bool Select(string selectString, AbstractActor unit);
    }
}
