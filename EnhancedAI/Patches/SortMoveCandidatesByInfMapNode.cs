using System.Reflection;
using BattleTech;
using EnhancedAI.Features;
using Harmony;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace EnhancedAI.Patches
{
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

    [HarmonyPatch]
    public static class SortMoveCandidatesByInfMapNode_Tick_Patch
    {
        public static MethodBase TargetMethod()
        {
            var type = AccessTools.TypeByName("SortMoveCandidatesByInfMapNode");
            return AccessTools.Method(type, "Tick");
        }

        public static void Postfix(object __instance, ref BehaviorTreeResults __result)
        {
            if (__result.nodeState != BehaviorNodeState.Success)
                return;

            if (!Main.Settings.ShouldPauseAI)
                return;

            var unit = Traverse.Create(__instance).Field("unit").GetValue<AbstractActor>();
            AIPause.InfluenceMapVisual.OnInfluenceMapSort(unit);
        }
    }
}
