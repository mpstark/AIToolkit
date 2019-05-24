using System;
using System.Collections.Generic;
using BattleTech;
using GraphCoroutines;
using Harmony;
using UnityEngine;

namespace EnhancedAI.Features
{
    public static class InfluenceMapModdedEvaluator
    {
        public static bool RunEvaluationForSeconds(InfluenceMapEvaluator evaluator, float seconds)
        {
            // this is largely rewritten from HBS code, and not subject to license
            var startTime = Time.realtimeSinceStartup;
            var coroutineTraverse = Traverse.Create(evaluator).Field("evaluationCoroutine");
            var completeTraverse = Traverse.Create(evaluator).Field("evaluationComplete");
            var coroutine = coroutineTraverse.GetValue<GraphCoroutine>();

            if (coroutine == null)
            {
                coroutine = new GraphCoroutine(IncrementalEvaluate(evaluator));
                coroutineTraverse.SetValue(coroutine);
            }

            while (Time.realtimeSinceStartup - startTime <= seconds)
            {
                coroutine.Update();

                if (completeTraverse.GetValue<bool>())
                {
                    coroutineTraverse.SetValue(null);
                    break;
                }
            }

            return completeTraverse.GetValue<bool>();
        }

        private static IEnumerable<Instruction> IncrementalEvaluate(InfluenceMapEvaluator evaluator)
        {
            // this is largely rewritten from HBS code, and not subject to license
            var evalTrav = Traverse.Create(evaluator);

            evalTrav.Method("ProfileFrameBegin").GetValue();
            evalTrav.Method("ProfileBegin", ProfileSection.AllInfluenceMaps).GetValue();

            yield return ControlFlow.Call(evalTrav.Method("Eval_Initialize").GetValue<IEnumerable<Instruction>>());
            //yield return ControlFlow.Call(evalTrav.Method("Eval_PositionalFactors").GetValue<IEnumerable<Instruction>>());
            //yield return ControlFlow.Call(evalTrav.Method("Eval_HostileFactors").GetValue<IEnumerable<Instruction>>());
            //yield return ControlFlow.Call(evalTrav.Method("Eval_AllyFactors").GetValue<IEnumerable<Instruction>>());
            yield return ControlFlow.Call(EvalFactors(evaluator));
            yield return ControlFlow.Call(evalTrav.Method("Apply_SprintScaling").GetValue<IEnumerable<Instruction>>());

            evaluator.expectedDamageFactor.LogEvaluation();

            evalTrav.Field("evaluationComplete").SetValue(true);
            evalTrav.Method("ProfileEnd", ProfileSection.AllInfluenceMaps).GetValue();
            evalTrav.Method("ProfileFrameEnd").GetValue();

            yield return null;
        }

        private static IEnumerable<Instruction> EvalFactors(InfluenceMapEvaluator evaluator)
        {
            // this is largely rewritten from HBS code, and not subject to license
            var trav = Traverse.Create(evaluator);
            var unit = trav.Field("unit").GetValue<AbstractActor>();
            var treeTrav = Traverse.Create(unit.BehaviorTree);

            // clear all accumulators
            for (var i = 0; i < evaluator.firstFreeWorkspaceEvaluationEntryIndex; i++)
            {
                var evalEntry = evaluator.WorkspaceEvaluationEntries[i];
                evalEntry.RegularMoveAccumulator = 0f;
                evalEntry.SprintMoveAccumulator = 0f;
            }

            var factors = new List<WeightedFactor>();
            factors.AddRange(trav.Field("allyFactors").GetValue<InfluenceMapAllyFactor[]>());
            factors.AddRange(trav.Field("hostileFactors").GetValue<InfluenceMapHostileFactor[]>());
            factors.AddRange(trav.Field("positionalFactors").GetValue<InfluenceMapPositionFactor[]>());

            // ally setup
            var allyCount = treeTrav.Method("GetBehaviorVariableValue", BehaviorVariableName.Int_AllyInfluenceCount)
                .GetValue<BehaviorVariableValue>().IntVal;
            var allAllies = unit.BehaviorTree.GetAllyUnits().ConvertAll<ICombatant>(x => x);
            var allies = trav.Method("getNClosestCombatants", allAllies, allyCount).GetValue<List<ICombatant>>();

            // hostile setup
            unit.EvaluateExpectedArmor();
            foreach (var combatant in unit.BehaviorTree.enemyUnits)
                (combatant as AbstractActor)?.EvaluateExpectedArmor();

            var hostileCount = treeTrav.Method("GetBehaviorVariableValue", BehaviorVariableName.Int_HostileInfluenceCount)
                .GetValue<BehaviorVariableValue>().IntVal;
            var hostiles = trav.Method("getNClosestCombatants", unit.BehaviorTree.enemyUnits, hostileCount).GetValue<List<ICombatant>>();

            // potential next frame
            yield return null;

            // evaluate factors
            foreach (var factor in factors)
            {
                var regularMoveWeightName = factor.GetRegularMoveWeightBVName();
                var sprintMoveWeightName = factor.GetSprintMoveWeightBVName();

                // weights are gotten normally if not INVALID_UNSET
                // and if they are,the weight is looked for in the units BehaviorVariableOverrides
                var regularMoveWeight = 0f;
                var sprintMoveWeight = 0f;
                if (regularMoveWeightName != BehaviorVariableName.INVALID_UNSET)
                {
                    regularMoveWeight = treeTrav.Method("GetBehaviorVariableValue", regularMoveWeightName)
                        .GetValue<BehaviorVariableValue>().FloatVal;
                }
                else if (Main.UnitToAIOverride.ContainsKey(unit))
                {
                    var weightName = factor.GetType().Name + "Weight";
                    if (Main.UnitToAIOverride[unit].BehaviorVariableOverrides.ContainsKey(weightName))
                        regularMoveWeight = Main.UnitToAIOverride[unit].BehaviorVariableOverrides[weightName].FloatVal;
                }

                if (sprintMoveWeightName != BehaviorVariableName.INVALID_UNSET)
                {
                    sprintMoveWeight = treeTrav.Method("GetBehaviorVariableValue", sprintMoveWeightName)
                        .GetValue<BehaviorVariableValue>().FloatVal;
                }
                else if (Main.UnitToAIOverride.ContainsKey(unit))
                {
                    var weightName = factor.GetType().Name + "SprintWeight";
                    if (Main.UnitToAIOverride[unit].BehaviorVariableOverrides.ContainsKey(weightName))
                        sprintMoveWeight = Main.UnitToAIOverride[unit].BehaviorVariableOverrides[weightName].FloatVal;
                }

                if (Math.Abs(regularMoveWeight) < 0.001 && Math.Abs(sprintMoveWeight) < 0.001)
                    continue;

                var minValue = float.MaxValue;
                var maxValue = float.MinValue;
                factor.InitEvaluationForPhaseForUnit(unit);
                trav.Method("ProfileBegin", ProfileSection.AllInfluenceMaps, factor.Name).GetValue();

                for (var i = 0; i < evaluator.firstFreeWorkspaceEvaluationEntryIndex; i++)
                {
                    var evalEntry = evaluator.WorkspaceEvaluationEntries[i];
                    var moveType = (!evalEntry.HasSprintMove) ? MoveType.Walking : MoveType.Sprinting;

                    evalEntry.FactorValue = 0f;

                    switch (factor)
                    {
                        case InfluenceMapAllyFactor allyFactor:
                            foreach (var ally in allies)
                            {
                                evalEntry.FactorValue += allyFactor.EvaluateInfluenceMapFactorAtPositionWithAlly(unit,
                                    evalEntry.Position, evalEntry.Angle, ally);
                            }
                            break;
                        case InfluenceMapHostileFactor hostileFactor:
                            foreach (var hostile in hostiles)
                            {
                                evalEntry.FactorValue += hostileFactor.EvaluateInfluenceMapFactorAtPositionWithHostile(unit,
                                    evalEntry.Position, evalEntry.Angle, moveType, hostile);
                            }
                            break;
                        case InfluenceMapPositionFactor positionFactor:
                            PathNode node = null;
                            if (evalEntry.PathNodes.ContainsKey(moveType))
                            {
                                node = evalEntry.PathNodes[moveType];
                            }
                            else if (moveType == MoveType.Walking && evalEntry.PathNodes.ContainsKey(MoveType.Backward))
                            {
                                node = evalEntry.PathNodes[MoveType.Backward];
                            }
                            else
                            {
                                // this is the weirdest shit from decompiled code
                                // I could guess what this means, but just use the decompiled version
                                using (var enumerator = evalEntry.PathNodes.Keys.GetEnumerator())
                                {
                                    if (enumerator.MoveNext())
                                    {
                                        var key = enumerator.Current;
                                        node = evalEntry.PathNodes[key];
                                    }
                                }
                            }

                            evalEntry.FactorValue = positionFactor.EvaluateInfluenceMapFactorAtPosition(unit,
                                evalEntry.Position, evalEntry.Angle, moveType, node);
                            break;
                    }

                    minValue = Mathf.Min(minValue, evalEntry.FactorValue);
                    maxValue = Mathf.Max(maxValue, evalEntry.FactorValue);

                    // potential next frame every 16 entries
                    if (i % 16 == 0)
                        yield return null;
                }

                if (minValue >= maxValue)
                {
                    trav.Method("ProfileEnd", ProfileSection.AllInfluenceMaps, factor.Name).GetValue();
                    continue;
                }

                for (var i = 0; i < evaluator.firstFreeWorkspaceEvaluationEntryIndex; i++)
                {
                    var rawValue = evaluator.WorkspaceEvaluationEntries[i].FactorValue;
                    var normValue = (rawValue - minValue) / (maxValue - minValue);
                    var regularValue = normValue * regularMoveWeight;
                    var sprintValue = normValue * sprintMoveWeight;

                    evaluator.WorkspaceEvaluationEntries[i].RegularMoveAccumulator += regularValue;
                    evaluator.WorkspaceEvaluationEntries[i].SprintMoveAccumulator += sprintValue;
                    evaluator.WorkspaceEvaluationEntries[i].ValuesByFactorName[factor.GetType().Name]
                        = new EvaluationDebugLogRecord(rawValue, normValue, regularValue, regularMoveWeight, sprintValue, sprintMoveWeight);
                }

                trav.Method("ProfileEnd", ProfileSection.AllInfluenceMaps, factor.Name).GetValue();
            }
        }
    }
}
