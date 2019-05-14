using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BattleTech;
using EnhancedAI.Selectors;
using Newtonsoft.Json;

namespace EnhancedAI.Resources
{
    public class AIOverrideDef
    {
        public string Name;
        public List<SelectorValue> Selectors;
        public int Priority = 0;

        public BehaviorNodeJSONRepresentation NewBehaviorTreeRoot;
        public string BehaviorScopesDirectory;
        public Dictionary<BehaviorVariableName, BehaviorVariableValue> BehaviorVariableOverrides;
        public List<string> RemoveInfluenceFactors = new List<string>();
        public List<string> NewAllyInfluenceFactors = new List<string>();
        public List<string> NewHostileInfluenceFactors = new List<string>();
        public List<string> NewPositionInfluenceFactors = new List<string>();


        [JsonIgnore]
        public BehaviorVariableScopeManagerWrapper ScopeWrapper;


        public bool MatchesUnit(AbstractActor unit)
        {
            if (Selectors == null || Selectors.Count == 0)
                return false;

            return Selectors.All(selector => selector.MatchesUnit(unit));
        }


        public static AIOverrideDef FromJSON(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<AIOverrideDef>(json);
            }
            catch (Exception e)
            {
                Main.HBSLog?.LogError("AIOverrideDef.FromJSON tossed exception");
                Main.HBSLog?.LogException(e);
                return null;
            }
        }

        public static AIOverrideDef FromPath(string path)
        {
            if (!File.Exists(path))
            {
                Main.HBSLog?.LogWarning($"Could not find file at: {path}");
                return null;
            }

            return FromJSON(File.ReadAllText(path));
        }

        public static AIOverrideDef MatchToUnitFrom(IEnumerable<AIOverrideDef> overrides, AbstractActor unit)
        {
            var matching = overrides.Where(o => o.MatchesUnit(unit)).ToArray();

            if (matching.Length == 0)
                return null;

            if (matching.Length == 1)
                return matching[0];

            // find the one with the highest priority, and then just choose the first
            // one with that priority
            var maxPriority = matching.Max(o => o.Priority);
            return matching.First(o => o.Priority == maxPriority);
        }
    }
}
