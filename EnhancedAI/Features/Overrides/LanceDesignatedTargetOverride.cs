using System.Collections.Generic;
using System.Linq;
using BattleTech;
using UnityEngine;

namespace EnhancedAI.Features.Overrides
{
    public static class LanceDesignatedTargetOverride
    {
        // TODO: make this more moddable from TeamAIOverride
        public static AbstractActor TryGetDesignatedTarget(AITeam team, List<AbstractActor> lanceUnits,
            List<AbstractActor> hostileUnits)
        {
            if (!Main.TeamToAIOverride.ContainsKey(team) || !Main.TeamToAIOverride[team].DesignateTargets)
                return null;

            if (lanceUnits.Count <= 1 || hostileUnits.Count <= 1)
                return null;

            var hostileToNumLof = new Dictionary<ICombatant, int>();
            foreach (var hostile in hostileUnits)
            {
                var numLof = 0;
                foreach (var unit in lanceUnits)
                {
                    if (unit.HasLOSToTargetUnit(hostile)
                        && unit.HasLOFToTargetUnit(hostile, unit.GetLongestRangeWeapon(false)))
                    {
                        numLof++;
                    }
                }

                hostileToNumLof[hostile] = numLof;
            }

            // get the target that has the most LOF from this lance
            var maxLof = hostileToNumLof.Values.Max();
            var maxLofHostiles = hostileToNumLof.Where(h => h.Value == maxLof).Select(h => h.Key).ToArray();
            if (maxLofHostiles.Length == 1)
            {
                var target = maxLofHostiles[0] as AbstractActor;
                Main.HBSLog?.Log($"Selecting highest LOS/LOF ({maxLof}) target {target?.UnitName} with pilot {target?.GetPilot()?.Name}");

                return target;
            }

            // if there are multiple targets with the same LOF number, use the closest
            ICombatant closest = null;
            var closestDistance = float.MaxValue;
            foreach (var hostile in maxLofHostiles)
            {
                foreach (var unit in lanceUnits)
                {
                    var distance = Vector3.Distance(hostile.CurrentPosition, unit.CurrentPosition);
                    if (!(distance < closestDistance))
                        continue;

                    closest = hostile;
                    closestDistance = distance;
                }
            }

            var closestTarget = closest as AbstractActor;
            Main.HBSLog?.Log($"Selecting closest highest LOS/LOF ({maxLof}) target {closestTarget?.UnitName} with pilot {closestTarget?.GetPilot()?.Name}");

            return closest as AbstractActor;
        }
    }
}
