using System.Collections.Generic;
using System.Linq;
using BattleTech;
using EnhancedAI.Resources;
using EnhancedAI.TurnOrderFactors;
using EnhancedAI.Util;
using Harmony;
using UnityEngine;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace EnhancedAI.Features.Overrides
{
    public static class TurnOrderOverride
    {
        private static readonly Dictionary<string, ITurnOrderFactor> AvailableFactors
            = new Dictionary<string, ITurnOrderFactor>();


        public static AbstractActor TryOverrideCurrentUnit(List<AbstractActor> units,
            TeamAIOverrideDef teamAIOverride)
        {
            if (units == null || units.Count == 0)
                return null;

            if (units.Count == 1)
                return units[0];

            var unitToTotal = new Dictionary<AbstractActor, float>();
            var unitToFactors = new Dictionary<AbstractActor, Dictionary<string, float>>();

            // initialize totals for units
            foreach (var unit in units)
            {
                unitToTotal.Add(unit, 0f);
                unitToFactors.Add(unit, new Dictionary<string, float>());
            }

            // calculate weighted/normalized factor values for each unit
            foreach (var factorName in teamAIOverride.TurnOrderFactorWeights.Keys)
            {
                var weight = teamAIOverride.TurnOrderFactorWeights[factorName];
                if (weight == 0f)
                    continue;

                if (!AvailableFactors.ContainsKey(factorName))
                {
                    var type = TypeUtil.GetTypeByName(factorName, typeof(ITurnOrderFactor));
                    if (type == null)
                    {
                        Main.HBSLog?.LogWarning($"Could not find ITurnOrderFactor: {factorName}");
                        continue;
                    }

                    var factor = AccessTools.Constructor(type)?.Invoke(null) as ITurnOrderFactor;
                    if (factor == null)
                    {
                        Main.HBSLog?.LogWarning($"Could not construct ITurnOrderFactor: {factorName}");
                        continue;
                    }

                    AvailableFactors.Add(factorName, factor);
                }

                var max = float.MinValue;
                var min = float.MaxValue;

                // evaluate factor for all units
                foreach (var unit in units)
                {
                    var factor = AvailableFactors[factorName];
                    var factorValue = factor.EvaluateUnit(unit);

                    if (factorValue == float.MinValue || factorValue == float.MaxValue)
                    {
                        Main.HBSLog?.Log($"{factorName} evaluated to min/max for unit {unit.UnitName}, skipping");
                        continue;
                    }

                    unitToFactors[unit][factorName] = factorValue;
                    max = Mathf.Max(max, factorValue);
                    min = Mathf.Min(min, factorValue);
                }

                if (min >= max)
                    continue;

                // normalize based on minValue/maxValue and weight
                foreach (var unit in units)
                {
                    if (!unitToFactors[unit].ContainsKey(factorName))
                        continue;

                    var raw = unitToFactors[unit][factorName];
                    var norm = (raw - min) / (max - min);

                    //if (min > 0)
                    //    norm = raw / max;
                    //else if (max < 0)
                    //    norm = max / raw;

                    var weighted = norm * weight;
                    unitToFactors[unit][factorName] = weighted;
                    unitToTotal[unit] += weighted;
                }
            }

            Main.HBSLog?.Log("TurnOrderOverride:");

            var maxUnitValue = unitToTotal.Max(x => x.Value);

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            var selectedUnit = unitToTotal.First(x => x.Value == maxUnitValue).Key;
            Main.HBSLog?.Log($"Selecting {selectedUnit.UnitName}");

            AIPause.PausePopup.AppendText("Turn Order:");

            foreach (var unit in units)
            {
                var selected = unit == selectedUnit ? "*" : "";
                Main.HBSLog?.Log($"{unit.UnitName}: {unitToTotal[unit]}");
                AIPause.PausePopup.AppendText($"  {unit.UnitName}: {unitToTotal[unit]:0.00}{selected}");
            }

            AIPause.PausePopup.AppendText("");

            return selectedUnit;
        }
    }
}
