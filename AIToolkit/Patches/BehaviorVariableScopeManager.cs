using AIToolkit.Util;
using BattleTech;
using Harmony;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace AIToolkit.Patches
{
    /// <summary>
    /// Skip ScopeLoaded calls that we want to ignore
    /// </summary>
    [HarmonyPatch(typeof(BehaviorVariableScopeManager), "OnBehaviorVariableScopeLoaded")]
    public static class BehaviorVariableScopeManager_OnBehaviorVariableScopeLoaded_Patch
    {
        public static bool Prefix(BehaviorVariableScopeManager __instance)
        {
            return !BVScopeManagerWrapper.IgnoreScopeLoadedCalls
                && !BVScopeManagerWrapper.IgnoreScopeLoadedCallsFrom.Contains(__instance);
        }
    }
}
