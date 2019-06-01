using BattleTech;
using EnhancedAI.ActiveAbility;

namespace EnhancedAI.Features.Overrides
{
    public static class InvocationFromOrderOverride
    {
        public static InvocationMessage TryCreateInvocation(AbstractActor unit, OrderInfo order)
        {
            if (order is PilotAbilityOrderInfo abilityOrder)
                return new PilotAbilityInvocation(abilityOrder.Source, abilityOrder.Target, abilityOrder.AbilityID);

            return null;
        }
    }
}
