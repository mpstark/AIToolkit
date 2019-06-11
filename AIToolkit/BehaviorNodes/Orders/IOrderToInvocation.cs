using BattleTech;

namespace AIToolkit.BehaviorNodes.Orders
{
    public interface IOrderToInvocation
    {
        InvocationMessage GetInvocation(AbstractActor unit);
    }
}
