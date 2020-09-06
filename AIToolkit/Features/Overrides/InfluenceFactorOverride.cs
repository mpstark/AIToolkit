using System.Collections.Generic;
using System.Linq;
using BattleTech;
using AIToolkit.Resources;
using AIToolkit.Util;
using Harmony;
using static AIToolkit.Patches.FieldRefs;

namespace AIToolkit.Features.Overrides
{
    public static class InfluenceFactorOverride
    {
        public static void TryOverrideInfluenceFactors(BehaviorTree tree, UnitAIOverrideDef o)
        {
            // why o? line length getting too long!
            if (o.AddInfluenceFactors.Count == 0 && o.RemoveInfluenceFactors.Count == 0)
                return;

            var trav = Traverse.Create(tree.influenceMapEvaluator);
            Main.HBSLog?.Log($"Overriding Influence Factors for {tree.unit.UnitName}");

            var allyFactors = AllyFactorsRef(tree.influenceMapEvaluator);
            var hostileFactors = HostileFactorsRef(tree.influenceMapEvaluator);
            var positionalFactors = PositionalFactorsRef(tree.influenceMapEvaluator);

            var newAllyFactors = new List<InfluenceMapAllyFactor>();
            var newHostileFactors = new List<InfluenceMapHostileFactor>();
            var newPositionalFactors = new List<InfluenceMapPositionFactor>();

            // add old influence factors if we're not supposed to remove them
            newAllyFactors.AddRange(allyFactors
                .Where(f => !o.RemoveInfluenceFactors.Contains(f.GetType().Name)));
            newHostileFactors.AddRange(hostileFactors
                .Where(f => !o.RemoveInfluenceFactors.Contains(f.GetType().Name)));
            newPositionalFactors.AddRange(positionalFactors
                .Where(f => !o.RemoveInfluenceFactors.Contains(f.GetType().Name)));

            // add new factors
            if (o.AddInfluenceFactors.Count != 0)
            {
                newAllyFactors.AddRange(ConstructNewFactors<InfluenceMapAllyFactor>(o.AddInfluenceFactors));
                newHostileFactors.AddRange(ConstructNewFactors<InfluenceMapHostileFactor>(o.AddInfluenceFactors));
                newPositionalFactors.AddRange(ConstructNewFactors<InfluenceMapPositionFactor>(o.AddInfluenceFactors));
            }

            Main.HBSLog?.Log($"  Had this number of factors -- ally: {allyFactors.Length} hostile: {hostileFactors.Length} positional: {positionalFactors.Length}");
            Main.HBSLog?.Log($"  Now -- ally: {newAllyFactors.Count} hostile: {newHostileFactors.Count} positional: {newPositionalFactors.Count}");
        }

        private static List<T> ConstructNewFactors<T>(List<string> typeNames)
        {
            var factors = new List<T>();

            foreach (var typeName in typeNames)
            {
                var type = TypeUtil.GetTypeByName(typeName, typeof(WeightedFactor));
                if (type == null || !type.IsSubclassOf(typeof(T)))
                    continue;

                // does not support factors that contain parameters (none of the vanilla ones do)
                var factor = (T) AccessTools.Constructor(type)?.Invoke(null);
                if (factor != null)
                    factors.Add(factor);
            }

            return factors;
        }
    }
}
