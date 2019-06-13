using BattleTech;

namespace AIToolkit.Selectors.Combat
{
    public class IsInterleaved : Selector<CombatGameState>
    {
        public override bool Select(string selectString, CombatGameState combat)
        {
            if (selectString == "true")
                return combat.TurnDirector.IsInterleaved;

            return !combat.TurnDirector.IsInterleaved;
        }
    }
}
