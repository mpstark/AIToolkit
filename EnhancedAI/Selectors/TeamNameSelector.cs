using BattleTech;

namespace EnhancedAI.Selectors
{
    public class TeamNameSelector : ISelector
    {
        public bool Select(string selector, AbstractActor unit)
        {
            if (unit?.team == null)
                return false;

            return unit.team.Name == selector;
        }
    }
}
