using System.Linq;
using BattleTech;

namespace AIToolkit.Selectors.Combat
{
    public class ContractType : Selector<CombatGameState>
    {
        public override bool Select(string selectString, CombatGameState combat)
        {
            if (combat.ActiveContract == null)
                return false;

            var contactTypes = selectString.Split(' ');
            return contactTypes.Contains(combat.ActiveContract.GetContractTypeString(combat.BattleTechGame.Simulation));
        }
    }
}
