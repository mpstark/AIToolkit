using System.Collections.Generic;
using System.Linq;
using BattleTech;
using EnhancedAI.Resources;
using EnhancedAI.Util;
using Harmony;

namespace EnhancedAI.Features.Overrides
{
    public static class InfluenceFactorOverride
    {
        public static void TryOverrideInfluenceFactors(BehaviorTree tree, UnitAIOverride o)
        {
            // why o? line length getting too long!
            if (o.NewAllyInfluenceFactors.Count + o.NewHostileInfluenceFactors.Count
                + o.NewPositionInfluenceFactors.Count + o.RemoveInfluenceFactors.Count == 0)
                return;

            Main.HBSLog?.Log($"Overriding Influence Factors for {tree.unit.UnitName}");

            var trav = Traverse.Create(tree.influenceMapEvaluator);

            var allyFactors = trav.Field("allyFactors").GetValue<InfluenceMapAllyFactor[]>();
            var hostileFactors = trav.Field("hostileFactors").GetValue<InfluenceMapHostileFactor[]>();
            var positionalFactors = trav.Field("positionalFactors").GetValue<InfluenceMapPositionFactor[]>();

            Main.HBSLog?.Log($"Had this number of factors -- ally: {allyFactors.Length} hostile: {hostileFactors.Length} positional: {positionalFactors.Length}");

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
            newAllyFactors.AddRange(ConstructNewFactors<InfluenceMapAllyFactor>(o.NewAllyInfluenceFactors));
            newHostileFactors.AddRange(ConstructNewFactors<InfluenceMapHostileFactor>(o.NewHostileInfluenceFactors));
            newPositionalFactors.AddRange(ConstructNewFactors<InfluenceMapPositionFactor>(o.NewPositionInfluenceFactors));

            // set the factors in the influence evaluator
            trav.Field("allyFactors").SetValue(newAllyFactors.ToArray());
            trav.Field("hostileFactors").SetValue(newHostileFactors.ToArray());
            trav.Field("positionalFactors").SetValue(newPositionalFactors.ToArray());

            Main.HBSLog?.Log($"Now -- ally: {newAllyFactors.Count} hostile: {newHostileFactors.Count} positional: {newPositionalFactors.Count}");
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
