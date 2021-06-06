using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using AIToolkit.Features;
using BattleTech;
using Harmony;
using Mono.Unix;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace AIToolkit.Patches
{
    /// <summary>
    /// Skip non-essential drawDebugLines call
    /// </summary>
    [HarmonyPatch]
    public static class SortMoveCandidatesByInfMapNode_drawDebugLines_Patch
    {
        public static MethodBase TargetMethod()
        {
            var type = AccessTools.TypeByName("SortMoveCandidatesByInfMapNode");
            return AccessTools.Method(type, "drawDebugLines");
        }

        public static bool Prefix()
        {
            return false;
        }
    }

    /// <summary>
    /// Generate influence map visualization if we're pausing
    /// </summary>
    [HarmonyPatch]
    public static class SortMoveCandidatesByInfMapNode_Tick_Patch
    {
        public static MethodBase TargetMethod()
        {
            var type = AccessTools.TypeByName("SortMoveCandidatesByInfMapNode");
            return AccessTools.Method(type, "Tick");
        }

        // stops the vanilla method from constantly writing CSV files
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            var start = codes.FindIndex(c =>
                ReferenceEquals(c?.operand, AccessTools.Method(typeof(InfluenceMapEvaluator), "ExportInfluenceMapToCSV")));
            start -= 4;
            for (var index = 0; index < 5; index++)
            {
                codes[start + index].opcode = OpCodes.Nop;
            }

            return codes.AsEnumerable();
        }

        public static void Postfix(object __instance, ref BehaviorTreeResults __result, AbstractActor ___unit)
        {
            if (__result.nodeState != BehaviorNodeState.Success || !Main.Settings.ShouldPauseAI)
                return;

            AIPause.InfluenceMapVisual.OnInfluenceMapSort(___unit);
        }
    }
}
