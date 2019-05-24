using BattleTech;
using EnhancedAI.Features;
using Harmony;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace EnhancedAI.Patches
{
    /// <summary>
    /// Skip non-essential ValidateCoverage method, as it spams log if it detects
    /// that one of the enum values isn't hooked up
    /// </summary>
    [HarmonyPatch(typeof(InfluenceMapEvaluator), "ValidateCoverage")]
    public static class InfluenceMapEvaluator_ValidateCoverage_Patch
    {
        public static bool Prefix()
        {
            return false;
        }
    }

    /// <summary>
    /// Replace RunEvaluationForSeconds to use our own functions for evaluation
    /// </summary>
    [HarmonyPatch(typeof(InfluenceMapEvaluator), "RunEvaluationForSeconds")]
    public static class InfluenceMapEvaluator_RunEvaluationForSeconds_Patch
    {
        // ReSharper disable once RedundantAssignment
        public static bool Prefix(InfluenceMapEvaluator __instance, float seconds, ref bool __result)
        {
            __result = InfluenceMapModdedEvaluator.RunEvaluationForSeconds(__instance, seconds);
            return false;
        }
    }
}
