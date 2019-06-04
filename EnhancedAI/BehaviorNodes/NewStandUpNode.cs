using BattleTech;

namespace EnhancedAI.BehaviorNodes
{
    public class NewStandUpNode : LeafBehaviorNode
    {
        public NewStandUpNode(string name, BehaviorTree tree, AbstractActor unit) : base(name, tree, unit)
        {
        }

        protected override BehaviorTreeResults Tick()
        {
            if (!(unit is Mech) || !unit.IsOperational || !unit.IsProne || unit.HasMovedThisRound)
                return new BehaviorTreeResults(BehaviorNodeState.Failure);

            return new BehaviorTreeResults(BehaviorNodeState.Success)
            {
                orderInfo = new OrderInfo(OrderType.Stand)
            };
        }
    }
}
