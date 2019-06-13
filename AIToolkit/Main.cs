using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BattleTech;
using AIToolkit.Features;
using AIToolkit.Features.Overrides;
using AIToolkit.Resources;
using AIToolkit.Util;
using Harmony;
using HBS.Logging;

// ReSharper disable UnusedMember.Global

namespace AIToolkit
{
    public static class Main
    {
        internal static ILog HBSLog;
        internal static ModSettings Settings;
        internal static string Directory;

        private static readonly List<string> UnitAIOverridePaths = new List<string>();
        private static readonly List<string> TeamAIOverridePaths = new List<string>();
        private static readonly List<string> BehaviorNodePaths = new List<string>();

        internal static List<UnitAIOverrideDef> UnitAIOverrides = new List<UnitAIOverrideDef>();
        internal static List<TeamAIOverrideDef> TeamAIOverrides = new List<TeamAIOverrideDef>();
        internal static List<BehaviorNodeDef> BehaviorNodeDefs = new List<BehaviorNodeDef>();

        internal static readonly Dictionary<AbstractActor, UnitAIOverrideDef> UnitToAIOverride
            = new Dictionary<AbstractActor, UnitAIOverrideDef>();
        internal static readonly Dictionary<AITeam, TeamAIOverrideDef> TeamToAIOverride
            = new Dictionary<AITeam, TeamAIOverrideDef>();


        public static void Init(string modDir, string settings)
        {
            var harmony = HarmonyInstance.Create("io.github.mpstark.AIToolkit");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            HBSLog = Logger.GetLogger("AIToolkit");

            Settings = ModSettings.Parse(settings);
            Directory = modDir;

            if (Settings.ShouldDump)
                BehaviorTreeDump.DumpTrees(Settings.DumpType);
        }

        public static void FinishedLoading(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
        {
            if (customResources.ContainsKey(nameof(UnitAIOverrideDef)))
            {
                UnitAIOverridePaths.AddRange(customResources[nameof(UnitAIOverrideDef)]
                    .Values.Select(entry => entry.FilePath));
            }

            if (customResources.ContainsKey(nameof(TeamAIOverrideDef)))
            {
                TeamAIOverridePaths.AddRange(customResources[nameof(TeamAIOverrideDef)]
                    .Values.Select(entry => entry.FilePath));
            }

            if (customResources.ContainsKey(nameof(BehaviorNodeDef)))
            {
                BehaviorNodePaths.AddRange(customResources[nameof(BehaviorNodeDef)]
                    .Values.Select(entry => entry.FilePath));
            }
        }


        internal static void OnCombatInit()
        {
            ReloadResources();
            AIPause.DestroyUI();
        }

        internal static void ReloadResources()
        {
            UnitToAIOverride.Clear();
            TeamToAIOverride.Clear();

            UnitAIOverrides = SerializeUtil.FromPaths<UnitAIOverrideDef>(UnitAIOverridePaths);
            TeamAIOverrides = SerializeUtil.FromPaths<TeamAIOverrideDef>(TeamAIOverridePaths);
            BehaviorNodeDefs = SerializeUtil.FromPaths<BehaviorNodeDef>(BehaviorNodePaths);
        }


        internal static UnitAIOverrideDef TryOverrideUnitAI(AbstractActor unit)
        {
            var aiOverride = UnitAIOverrideDef.SelectOverride(unit,
                UnitAIOverrides.Cast<AIOverrideDef<AbstractActor>>()) as UnitAIOverrideDef;

            if (aiOverride == null)
                return null;

            if (UnitToAIOverride.ContainsKey(unit))
            {
                if (UnitToAIOverride[unit] == aiOverride)
                    return UnitToAIOverride[unit];

                ResetUnitAI(unit);
            }

            HBSLog?.Log($"Overriding AI on unit {unit.UnitName} with {aiOverride.Name}");
            UnitToAIOverride[unit] = aiOverride;

            BehaviorTreeOverride.TryOverrideTree(unit.BehaviorTree, UnitToAIOverride[unit]);
            InfluenceFactorOverride.TryOverrideInfluenceFactors(unit.BehaviorTree, UnitToAIOverride[unit]);

            return UnitToAIOverride[unit];
        }

        internal static void ResetUnitAI(AbstractActor unit)
        {
            HBSLog?.Log($"Resetting AI for unit {unit.UnitName}");

            Traverse.Create(unit.BehaviorTree).Method("InitRootNode").GetValue();
            unit.BehaviorTree.influenceMapEvaluator = new InfluenceMapEvaluator();
            unit.BehaviorTree.Reset();
        }


        internal static TeamAIOverrideDef TryOverrideTeamAI(AITeam team)
        {
            var aiOverride = TeamAIOverrideDef.SelectOverride(team,
                TeamAIOverrides.Cast<AIOverrideDef<AITeam>>()) as TeamAIOverrideDef;

            if (aiOverride == null)
                return null;

            if (TeamToAIOverride.ContainsKey(team))
            {
                if (TeamToAIOverride[team] == aiOverride)
                    return TeamToAIOverride[team];

                ResetTeamAI(team);
            }

            HBSLog?.Log($"Overriding AI on team {team.Name} with {aiOverride.Name}");
            TeamToAIOverride[team] = aiOverride;

            return TeamToAIOverride[team];
        }

        internal static void ResetTeamAI(AITeam team)
        {
            HBSLog?.Log($"Resetting AI for team {team.Name}");
        }
    }
}
