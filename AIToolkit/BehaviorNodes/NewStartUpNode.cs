using BattleTech;

namespace AIToolkit.BehaviorNodes
{
    public class NewStartUpNode : LeafBehaviorNode
    {
        public NewStartUpNode(string name, BehaviorTree tree, AbstractActor unit) : base(name, tree, unit)
        {
        }

        protected override BehaviorTreeResults Tick()
        {
            if (!(unit is Mech) || !unit.IsShutDown)
                return new BehaviorTreeResults(BehaviorNodeState.Failure);

            return new BehaviorTreeResults(BehaviorNodeState.Success)
            {
                orderInfo = new OrderInfo(OrderType.StartUp)
            };
        }
    }
}
