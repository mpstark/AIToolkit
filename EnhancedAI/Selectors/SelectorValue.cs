using System.Collections.Generic;
using BattleTech;

namespace EnhancedAI.Selectors
{
    public class SelectorValue
    {
        private static readonly Dictionary<string, ISelector> AvailableSelectors = new Dictionary<string, ISelector>
        {
            { "Custom", new CustomSelector() },
            { "TeamName", new TeamNameSelector() },
            { "Role", new RoleSelector() },
            { "Tree", new TreeSelector() }
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
