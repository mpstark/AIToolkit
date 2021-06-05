// AIToolkit.BehaviorNodes.PilotHasPassiveAbilityNode
using System.Linq;
using BattleTech;

public class PilotHasPassiveAbilityNode : LeafBehaviorNode
{
    private string _abilityID;

    public PilotHasPassiveAbilityNode(string name, BehaviorTree tree, AbstractActor unit, string abilityID)
        : base(name, tree, unit)
    {
        _abilityID = abilityID;
    }

    protected override BehaviorTreeResults Tick()
    {
        var pilot = unit.GetPilot();
        if (pilot == null)
        {
            return new BehaviorTreeResults(BehaviorNodeState.Failure);
        }
        if (pilot.PassiveAbilities.LastOrDefault((Ability a) => a.Def.Id == _abilityID) == null)
        {
            return new BehaviorTreeResults(BehaviorNodeState.Failure);
        }
        return new BehaviorTreeResults(BehaviorNodeState.Success);
    }
}
