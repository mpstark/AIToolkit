using BattleTech;

namespace EnhancedAI.UnitSelectors
{
    public class TeamNameUnitSelector : IUnitSelector
    {
        public bool Select(string selectString, AbstractActor unit)
        {
            if (unit?.team == null)
                return false;

            return unit.team.Name == selectString;
        }
    }
}
