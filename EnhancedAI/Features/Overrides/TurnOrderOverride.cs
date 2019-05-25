using System.Collections.Generic;
using BattleTech;

namespace EnhancedAI.Features.Overrides
{
    public static class TurnOrderOverride
    {
        public static AbstractActor GetHighestPriorityUnitFrom(List<AbstractActor> units)
        {
            if (units == null || units.Count == 0)
                return null;

            //var unitToFactors = new Dictionary<AbstractActor, Dictionary<string, float>>();

            // return the key of the highest value in the dictionary
            //return unitsToPriority.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

            return units[0];
        }
    }
}
