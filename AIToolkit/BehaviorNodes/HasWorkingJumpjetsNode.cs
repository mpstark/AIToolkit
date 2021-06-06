using BattleTech;

namespace AIToolkit.BehaviorNodes
{
    public class HasWorkingJumpjetsNode : LeafBehaviorNode
    {
        public HasWorkingJumpjetsNode(string name, BehaviorTree tree, AbstractActor unit)
            : base(name, tree, unit)
        {
        }

        public override BehaviorTreeResults Tick()
        {
            var mech = unit as Mech;
            if (mech == null || mech.WorkingJumpjets <= 1)
            {
                return new BehaviorTreeResults(BehaviorNodeState.Failure);
            }
            return new BehaviorTreeResults(BehaviorNodeState.Success);
        }
    }
}
