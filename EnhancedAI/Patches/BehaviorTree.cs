using EnhancedAI.Features;
using Harmony;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace EnhancedAI.Patches
{
    /// <summary>
    /// Hook BehaviorTree InitRootNode to potentially dump the tree
    /// </summary>
    [HarmonyPatch(typeof(BehaviorTree), "InitRootNode")]
    public static class BehaviorTree_InitRootNode_Patch
    {
        public static void Postfix(BehaviorTree __instance)
        {
            if (Main.Settings.ShouldDump)
                TreeDump.DumpTree(__instance, Main.Settings.DumpType);
        }
    }
}
