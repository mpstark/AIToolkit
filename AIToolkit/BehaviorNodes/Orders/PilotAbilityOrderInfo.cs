using BattleTech;

namespace AIToolkit.BehaviorNodes.Orders
{
    public class PilotAbilityOrderInfo : OrderInfo, IOrderToInvocation
    {
        public AbstractActor Source;
        public ICombatant Target;
        public string AbilityID;

        public PilotAbilityOrderInfo(AbstractActor source, ICombatant target, string abilityID)
            : base(OrderType.ActiveAbility)
        {
            Source = source;
            Target = target;
            AbilityID = abilityID;
        }

        public InvocationMessage GetInvocation(AbstractActor unit)
        {
            return new PilotAbilityInvocation(Source, Target, AbilityID);
        }
    }
}
