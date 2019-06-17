using BattleTech;

namespace AIToolkit.Selectors.Combat
{
    public class IsSkirmish : Selector<CombatGameState>
    {
        public override bool Select(string selectString, CombatGameState combat)
        {
            var isTrue = UnityGameInstance.BattleTechGame.Simulation == null;
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
