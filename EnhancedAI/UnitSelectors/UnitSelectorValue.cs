using System.Collections.Generic;
using BattleTech;

namespace EnhancedAI.UnitSelectors
{
    public class UnitSelectorValue
    {
        private static readonly Dictionary<string, IUnitSelector> AvailableSelectors = new Dictionary<string, IUnitSelector>
        {
            { "Custom", new CustomUnitSelector() },
            { "TeamName", new TeamNameUnitSelector() },
            { "Role", new RoleUnitSelector() },
            { "Tree", new TreeUnitSelector() }
        };

        public string TypeName;
        public string SelectString;

        public bool MatchesUnit(AbstractActor unit)
        {
            if (!AvailableSelectors.ContainsKey(TypeName))
                return false;

            return AvailableSelectors[TypeName].Select(SelectString, unit);
        }
    }
}
