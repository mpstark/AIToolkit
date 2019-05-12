using BattleTech;

namespace EnhancedAI.Selectors
{
    public interface ISelector
    {
        bool Select(string selector, AbstractActor unit);
    }
}
