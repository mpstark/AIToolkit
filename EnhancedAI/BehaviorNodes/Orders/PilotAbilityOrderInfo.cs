using BattleTech;

namespace EnhancedAI.BehaviorNodes.Orders
{
    public class PilotAbilityOrderInfo : OrderInfo
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
    }
}
