using BattleTech;
using AIToolkit.Util;
using Harmony;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace AIToolkit.Patches
{
    [HarmonyPatch(typeof(BehaviorVariableScopeManager), "OnBehaviorVariableScopeLoaded")]
    public static class BehaviorVariableScopeManager_OnBehaviorVariableScopeLoaded_Patch
    {
        public static bool Prefix(BehaviorVariableScopeManager __instance, string id)
        {
            if (BVScopeManagerWrapper.IgnoreScopeLoadedCalls ||
                BVScopeManagerWrapper.IgnoreScopeLoadedCallsFrom
                    .Contains(__instance))
            {
                return false;
            }

            return true;
        }
    }
}
