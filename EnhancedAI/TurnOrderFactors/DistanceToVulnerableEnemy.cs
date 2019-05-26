using BattleTech;
using UnityEngine;

namespace EnhancedAI.TurnOrderFactors
{
    public class DistanceToVulnerableEnemy : ITurnOrderFactor
    {
        public float EvaluateUnit(AbstractActor unit)
        {
            var minDistance = float.MaxValue;
            foreach (var enemyCombatant in unit.BehaviorTree.enemyUnits)
            {
                if (enemyCombatant.IsDead)
                    continue;

                if (!(enemyCombatant is Mech mech))
                    continue;

                if (mech.IsVulnerableToCalledShots())
                    minDistance = Mathf.Min(minDistance, Vector3.Distance(unit.CurrentPosition, mech.CurrentPosition));
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (minDistance == float.MaxValue)
                return float.MinValue;

            return 1f / minDistance;
        }
    }
}
