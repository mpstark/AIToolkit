using System.Collections.Generic;
using BattleTech;
using EnhancedAI.Util;
using Newtonsoft.Json;

// ReSharper disable CollectionNeverUpdated.Global

namespace EnhancedAI.Resources
{
    public class UnitAIOverrideDef : AIOverrideDef<AbstractActor>
    {
        public SerializableBehaviorNode NewBehaviorTreeRoot;
        public string BehaviorScopesDirectory;
        public Dictionary<string, BehaviorVariableValue> BehaviorVariableOverrides
            = new Dictionary<string, BehaviorVariableValue>();
        public List<string> AddInfluenceFactors = new List<string>();
        public List<string> RemoveInfluenceFactors = new List<string>();

        [JsonIgnore]
        public BVScopeManagerWrapper ScopeWrapper;
    }
}
