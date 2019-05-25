using BattleTech;
using EnhancedAI.Features;
using EnhancedAI.Features.Overrides;
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
            return !AIPause.OnAIThink(__instance);
        }
    }

    /// <summary>
    /// Hook to potentially override units ai each time it comes up, as well as
    /// override the order in which units are selected
    /// </summary>
    [HarmonyPatch(typeof(AITeam), "selectCurrentUnit")]
    public static class AITeam_selectCurrentUnit_patch
    {
        // ReSharper disable once RedundantAssignment
        public static bool Prefix(AITeam __instance, ref AbstractActor __result)
        {
            __result = TurnOrderOverride.GetHighestPriorityUnitFrom(__instance.GetUnusedUnitsForCurrentPhase());
            Main.TryOverrideAI(__result);
            return false;
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
            // if shouldPauseAI is on at all, we should never fail to get invocation because of time
            if (Main.Settings.ShouldPauseAI)
                Traverse.Create(__instance).Field("planningStartTime").SetValue(__instance.Combat.BattleTechGame.Time);

            var inject = AIPause.TryGetMessageInject();
            if (inject == null)
                return true;

            // abort the call and force the return to be the previously skipped
            // invocation message that we got in the postfix
            __result = inject;
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
            var skipThisInvocation = AIPause.OnAIInvocation(__instance, __result);
            if (skipThisInvocation)
                __result = null;
        }
    }
}
