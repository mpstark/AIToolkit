using BattleTech;

namespace AIToolkit.Selectors.Combat
{
    public class IsStoryContract : Selector<CombatGameState>
    {
        public override bool Select(string selectString, CombatGameState combat)
        {
            if (combat.ActiveContract == null)
                return false;

            var isTrue = combat.ActiveContract.IsPriorityContract;
            switch (selectString)
            {
                case "true":
                    return isTrue;
                case "false":
                    return !isTrue;
                default:
                    return false;
            }
        }
    }
}
