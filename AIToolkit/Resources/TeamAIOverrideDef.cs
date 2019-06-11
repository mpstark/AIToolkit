using System.Collections.Generic;
using AIToolkit.Util;
using Newtonsoft.Json;

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable CollectionNeverUpdated.Global

namespace AIToolkit.Resources
{
    public class TeamAIOverrideDef : AIOverrideDef<AITeam>
    {
        public string BehaviorScopesDirectory;
        public Dictionary<string, float> TurnOrderFactorWeights = new Dictionary<string, float>();
        public bool DesignateTargets = false;

        [JsonIgnore]
        public BVScopeManagerWrapper ScopeWrapper;
    }
}
