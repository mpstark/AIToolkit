using System.Linq;
using AIToolkit.BehaviorNodes.Orders;
using BattleTech;

namespace AIToolkit.BehaviorNodes
{
    public class UsePilotAbilityOnSelfNode : LeafBehaviorNode
    {
        private string _abilityID;

        public UsePilotAbilityOnSelfNode(string name, BehaviorTree tree, AbstractActor unit, string abilityID) : base(name, tree, unit)
        {
            _abilityID = abilityID;
        }

        public override BehaviorTreeResults Tick()
        {
            var pilot = unit.GetPilot();
            if (pilot == null)
                return new BehaviorTreeResults(BehaviorNodeState.Failure);

            var ability = pilot.ActiveAbilities.LastOrDefault(a => a.Def.Id == _abilityID);
            if (ability == null || !ability.IsAvailable)
                return new BehaviorTreeResults(BehaviorNodeState.Failure);

            return new BehaviorTreeResults(BehaviorNodeState.Success)
            {
                orderInfo = new PilotAbilityOrderInfo(unit, unit, _abilityID)
            };
        }
    }
}
