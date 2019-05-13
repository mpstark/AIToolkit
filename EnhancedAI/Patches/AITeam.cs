using BattleTech;
using EnhancedAI.Features;
using Harmony;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace EnhancedAI.Patches
{
    /// <summary>
    /// Hook for pausing the AI
    /// </summary>
    [HarmonyPatch(typeof(AITeam), "think")]
    public static class AITeam_think_patch
    {
        public static bool Prefix(AITeam __instance)
        {
            return !AIDebugPause.ShouldSkipAIThink(__instance);
        }
    }

    /// <summary>
    /// Hook for getting the next invocation message for the AI for pausing
    /// </summary>
    [HarmonyPatch(typeof(AITeam), "getInvocationForCurrentUnit")]
    public static class AITeam_getInvocationForCurrentUnit_patch
    {
        private static bool _injectedThisCall;

        public static bool Prefix(AITeam __instance, ref InvocationMessage __result)
        {
            var injectInvocation = AIDebugPause.TryGetInjectInvocationMessage();
            if (injectInvocation == null)
                return true;

            // abort the call and force the return to be the previously skipped
            // invocation message that we got in the postfix
            __result = injectInvocation;
            _injectedThisCall = true;
            return false;
        }

        public static void Postfix(AITeam __instance, ref InvocationMessage __result)
        {
            if (__result == null || _injectedThisCall)
            {
                _injectedThisCall = false;
                return;
            }

            // the ai has decided to do something, if we're pausing the AI,
            // we want to skip this invocation and inject it on the next think
            // after we hit the key shortcut
            var skipThisInvocation = AIDebugPause.OnAIInvocation(__instance, __result);
            if (skipThisInvocation)
                __result = null;
        }
    }
}
