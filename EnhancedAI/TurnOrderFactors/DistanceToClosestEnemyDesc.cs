using BattleTech;
using UnityEngine;

namespace EnhancedAI.TurnOrderFactors
{
    public class DistanceToClosestEnemyDesc : ITurnOrderFactor
    {
        public float EvaluateUnit(AbstractActor unit)
        {
            var hostiles = AIUtil.HostilesToUnit(unit);
            if (hostiles.Count == 0)
                return float.MinValue;

            var minDistance = float.MaxValue;
            foreach (var hostile in hostiles)
            {
                var distance = Vector3.Distance(unit.CurrentPosition, hostile.CurrentPosition);
                minDistance = Mathf.Min(minDistance, distance);
            }

            return minDistance;
        }
    }
}
