using System.Linq;
using BattleTech;
using UnityEngine;

namespace EnhancedAI.TurnOrderFactors
{
    public class DistanceToEnemy : ITurnOrderFactor
    {
        public float EvaluateUnit(AbstractActor unit)
        {
            var hostiles = AIUtil.HostilesToUnit(unit);
            if (hostiles.Count == 0)
                return float.MinValue;

            var minDistance = hostiles.Min(hostile =>
                Vector3.Distance(unit.CurrentPosition, hostile.CurrentPosition));

            return -1 * minDistance;
        }
    }
}
