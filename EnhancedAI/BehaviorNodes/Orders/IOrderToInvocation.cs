using BattleTech;

namespace EnhancedAI.BehaviorNodes.Orders
{
    public interface IOrderToInvocation
    {
        InvocationMessage GetInvocation(AbstractActor unit);
    }
}
