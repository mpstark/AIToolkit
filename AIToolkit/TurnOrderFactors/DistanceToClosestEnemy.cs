using BattleTech;
using UnityEngine;

namespace AIToolkit.TurnOrderFactors
{
    public class DistanceToClosestEnemy : ITurnOrderFactor
    {
        public float EvaluateUnit(AbstractActor unit)
        {
            var hostiles = AIUtil.HostilesToUnit(unit);
            if (hostiles.Count == 0)
                return float.MinValue;

            var minDistance = float.MaxValue;
            foreach (var hostile in hostiles)
            {
                if (hostile.IsDead)
                    continue;

                var distance = Vector3.Distance(unit.CurrentPosition, hostile.CurrentPosition);
                minDistance = Mathf.Min(minDistance, distance);
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (minDistance == float.MaxValue)
                return float.MinValue;

            return 1f / minDistance;
        }
    }
}
