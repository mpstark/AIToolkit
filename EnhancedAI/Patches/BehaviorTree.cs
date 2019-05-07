using System;
using System.Collections.Generic;
using System.IO;
using EnhancedAI.Util;
using Harmony;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace EnhancedAI.Patches
{
    [HarmonyPatch(typeof(BehaviorTree), "InitRootNode")]
    public static class BehaviorTree_InitRootNode_Patch
    {
        private static readonly List<BehaviorTreeIDEnum> hasPrinted = new List<BehaviorTreeIDEnum>();

        public static void Postfix(BehaviorTree __instance)
        {
            var id = Traverse.Create(__instance).Field("behaviorTreeIDEnum").GetValue<BehaviorTreeIDEnum>();

            if (hasPrinted.Contains(id))
                return;

            __instance.DumpTree(Path.Combine(Main.Directory, $"{Enum.GetName(typeof(BehaviorTreeIDEnum), id)}_dump.txt"));
            hasPrinted.Add(id);
        }
    }
}
