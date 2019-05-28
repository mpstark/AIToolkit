using BattleTech;
using UnityEngine;

namespace EnhancedAI.TurnOrderFactors
{
    public class DistanceToClosestVulnerableEnemy : ITurnOrderFactor
    {
        public float EvaluateUnit(AbstractActor unit)
        {
            var minDistance = float.MaxValue;
            foreach (var combatant in unit.BehaviorTree.enemyUnits)
            {
                var enemy = combatant as AbstractActor;
                if (enemy == null || enemy.IsDead || !enemy.IsVulnerableToCalledShots())
                    continue;

                var distance = Vector3.Distance(unit.CurrentPosition, enemy.CurrentPosition);
                minDistance = Mathf.Min(minDistance, distance);
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (minDistance == float.MaxValue)
                return float.MinValue;

            return 1f / minDistance;
        }
    }
}
