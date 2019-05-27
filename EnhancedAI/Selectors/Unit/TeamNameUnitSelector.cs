using BattleTech;

namespace EnhancedAI.Selectors.Unit
{
    public class TeamNameUnitSelector : ISelector<AbstractActor>
    {
        public bool Select(string selectString, AbstractActor unit)
        {
            if (unit?.team == null)
                return false;

            return unit.team.Name == selectString;
        }
    }
}
