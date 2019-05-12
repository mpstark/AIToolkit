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
        private static readonly Dictionary<string, ISelector> Selectors = new Dictionary<string, ISelector>
        {
            { "Custom", new CustomSelector() },
            { "TeamName", new TeamNameSelector() },
            { "Role", new RoleSelector() },
            { "Tree", new TreeSelector() }
        };

        public string Name;
        public string SelectorType;
        public string Selector;
        public int Priority = 0;
        public BehaviorNodeJSONRepresentation RootReplacement;
        public string BehaviorScopeDirectory;
        public Dictionary<BehaviorVariableName, BehaviorVariableValue> CustomScope;

        [JsonIgnore]
        public BehaviorVariableScopeManagerWrapper ScopeWrapper;


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

        public static AIOverrideDef SelectFrom(List<AIOverrideDef> overrides, AbstractActor unit)
        {
            var matching = overrides.FindAll(o => Selectors[o.SelectorType].Select(o.Selector, unit));

            if (matching.Count == 0)
                return null;

            if (matching.Count == 1)
                return matching[0];

            // find the one with the highest priority, and then just choose the first
            // one with that priority
            var maxPriority = matching.Max(o => o.Priority);
            return matching.Find(o => o.Priority == maxPriority);
        }
    }
}
