using System.Collections.Generic;
using System.IO;
using System.Linq;
using BattleTech;
using Harmony;
using HBS.Util;

namespace EnhancedAI
{
    // TODO: this seems really out of place now?
    public class BehaviorVariableScopeManagerWrapper
    {
        private static readonly HashSet<string> ValidIDs = new HashSet<string>
        {
            "global", "global_def", "global_sensorlock", "global_ruthless",
            "role_brawler", "role_brawler_def", "role_sniper", "role_sniper_def",
            "role_scout", "role_scout_def", "role_lastmanstanding", "role_lastmanstanding_def",
            "role_meleeonly", "role_meleeonly_def", "role_noncombatant", "role_noncombatant_def",
            "role_turret", "role_turret_def", "role_vehicle", "role_vehicle_def",
            "faction_davion", "faction_davion_def", "faction_kurita", "faction_kurita_def",
            "faction_liao", "faction_liao_def", "faction_marik", "faction_marik_def",
            "faction_steiner", "faction_steiner_def", "personality_disciplined",
            "personality_disciplined_def", "personality_aggressive", "personality_aggressive_def",
            "personality_qapersonality", "personality_qapersonality_def", "skill_reckless",
            "skill_reckless_def"
        };

        public static bool IgnoreScopeLoadedCalls;
        public static readonly HashSet<BehaviorVariableScopeManager> IgnoreScopeLoadedCallsFrom
            = new HashSet<BehaviorVariableScopeManager>();

        public BehaviorVariableScopeManager ScopeManager { get; private set; }
        private string _dirPath;

        public BehaviorVariableScopeManagerWrapper(GameInstance game, string path)
        {
            _dirPath = Path.Combine(Path.Combine(Main.Directory, ".."), path);
            Load(game);
        }

        public void Load(GameInstance game)
        {
            Main.HBSLog?.Log($"Loading behavior variable scopes from {_dirPath}");

            // we're going to delete the instance of ScopeManager, clean it up
            // if we already have a ScopeManager
            if (IgnoreScopeLoadedCallsFrom.Contains(ScopeManager))
                IgnoreScopeLoadedCallsFrom.Remove(ScopeManager);

            // we want to ignore OnBehaviorVariableScopeLoaded calls
            // both during the constructor of BehaviorVariableScopeManager and
            // for calls that take place after the constructor
            IgnoreScopeLoadedCalls = true;
            ScopeManager = new BehaviorVariableScopeManager(game);
            IgnoreScopeLoadedCalls = false;

            // load all the behavior variable scopes from directory
            var validPaths = Directory.GetFiles(_dirPath, "*.json")
                .Where(filePath => ValidIDs.Contains(Path.GetFileNameWithoutExtension(filePath)));
            foreach (var validPath in validPaths)
            {
                var id = Path.GetFileNameWithoutExtension(validPath);
                var json = Traverse.Create(typeof(JSONSerializationUtility))
                    .Method("StripHBSCommentsFromJSON", File.ReadAllText(validPath)).GetValue<string>();

                Main.HBSLog?.Log($"Loading scope with id {id}");
                ScopeManager.OnBehaviorVariableScopeLoaded(id, json);
            }

            // ignore future calls from datamanager
            IgnoreScopeLoadedCallsFrom.Add(ScopeManager);
        }
    }
}
