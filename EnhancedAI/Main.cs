using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BattleTech;
using EnhancedAI.Features;
using EnhancedAI.Resources;
using Harmony;
using HBS.Logging;

// ReSharper disable UnusedMember.Global

namespace EnhancedAI
{
    public static class Main
    {
        internal static ILog HBSLog;
        internal static ModSettings Settings;
        internal static string Directory;

        internal static readonly List<AIOverrideDef> AIOverrideDefs
            = new List<AIOverrideDef>();

        internal static readonly Dictionary<AbstractActor, AIOverrideDef> UnitToAIOverride
            = new Dictionary<AbstractActor, AIOverrideDef>();

        private static readonly List<string> AIOverridePaths = new List<string>();


        public static void Init(string modDir, string settings)
        {
            var harmony = HarmonyInstance.Create("io.github.mpstark.EnhancedAI");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            HBSLog = Logger.GetLogger("EnhancedAI");
            Logger.SetLoggerLevel("EnhancedAI", LogLevel.Log);

            Settings = ModSettings.Parse(settings);
            Directory = modDir;

            if (Settings.ShouldDump)
                BehaviorTreeDump.DumpTrees(Settings.DumpType);
        }

        public static void FinishedLoading(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
        {
            if (!customResources.ContainsKey(nameof(AIOverrideDef)))
                return;

            AIOverridePaths.AddRange(customResources[nameof(AIOverrideDef)].Values.Select(entry => entry.FilePath));
            ReloadAIOverrides();
        }


        internal static void ReloadAIOverrides()
        {
            UnitToAIOverride.Clear();
            AIOverrideDefs.Clear();

            foreach (var path in AIOverridePaths)
            {
                var overrideDef = AIOverrideDef.FromPath(path);
                if (overrideDef == null)
                {
                    HBSLog?.LogError($"AIOverrideDef Resource did not parse at {path}");
                    break;
                }

                HBSLog?.Log($"Parsed {overrideDef.Name} at {path}");
                AIOverrideDefs.Add(overrideDef);
            }
        }

        internal static void TryOverrideAI(AbstractActor unit)
        {
            var aiOverride = AIOverrideDef.MatchToUnitFrom(AIOverrideDefs, unit);

            // unit has already been overriden and has the same override then we just got
            if (UnitToAIOverride.ContainsKey(unit) && UnitToAIOverride[unit] == aiOverride)
                return;

            // unit has already been overridden but has a different override
            if (UnitToAIOverride.ContainsKey(unit) && UnitToAIOverride[unit] != aiOverride)
                ResetAI(unit);

            if (aiOverride == null)
                return;

            HBSLog?.Log($"Overriding AI on {unit.UnitName} with {aiOverride.Name}");

            UnitToAIOverride[unit] = aiOverride;
            BehaviorTreeReplace.TryReplaceTree(unit.BehaviorTree, aiOverride);
            InfluenceFactorOverride.TryOverrideInfluenceFactors(unit.BehaviorTree, aiOverride);
        }

        internal static void ResetAI(AbstractActor unit)
        {
            HBSLog?.Log($"Resetting AI for {unit.UnitName}");

            Traverse.Create(unit.BehaviorTree).Method("InitRootNode").GetValue();
            unit.BehaviorTree.influenceMapEvaluator = new InfluenceMapEvaluator();
            unit.BehaviorTree.Reset();
        }
    }
}
