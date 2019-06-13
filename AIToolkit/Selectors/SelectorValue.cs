using BattleTech;

namespace AIToolkit.Selectors
{
    public class SelectorValue
    {
        public string TypeName;
        public string SelectString;

        public bool Matches(object obj)
        {
            if (obj is AbstractActor unit)
            {
                var unitSelector = Selector<AbstractActor>.FindSelector(TypeName);
                if (unitSelector != null)
                    return unitSelector.Select(SelectString, unit);

                obj = unit.team;
            }

            if (obj is AITeam team)
            {
                var teamSelector = Selector<AITeam>.FindSelector(TypeName);
                if (teamSelector != null)
                    return teamSelector.Select(SelectString, team);

                obj = team.Combat;
            }

            if (obj is CombatGameState combat)
            {
                var combatSelector = Selector<CombatGameState>.FindSelector(TypeName);
                if (combatSelector != null)
                    return combatSelector.Select(SelectString, combat);
            }

            Main.HBSLog?.LogError($"Could not find a selector named {TypeName}, failing check");
            return false;
        }
    }
}
