using System.Linq;
using BattleTech;

namespace EnhancedAI.BehaviorNodes
{
    public class IsSlowestUnitInLanceNode : LeafBehaviorNode
    {
        public IsSlowestUnitInLanceNode(string name, BehaviorTree tree, AbstractActor unit) : base(name, tree, unit)
        {
        }

        protected override BehaviorTreeResults Tick()
        {
            // don't have lance
            if (unit.lance == null)
                return new BehaviorTreeResults(BehaviorNodeState.Success);

            var lanceUnits = unit.lance.unitGuids
                .Select(guid => unit.Combat.FindActorByGUID(guid))
                .Where(u => !u.IsDead)
                .ToArray();

            // solo in lance
            if (lanceUnits.Length <= 1)
                return new BehaviorTreeResults(BehaviorNodeState.Success);

            var minSpeed = lanceUnits.Min(u => u.MovementCaps.MaxWalkDistance);
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            var slowestUnit = lanceUnits.First(u => minSpeed == u.MovementCaps.MaxWalkDistance);

            return BehaviorTreeResults.BehaviorTreeResultsFromBoolean(unit == slowestUnit);
        }
    }
}
