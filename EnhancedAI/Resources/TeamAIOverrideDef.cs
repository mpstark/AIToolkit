using System.Collections.Generic;

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable CollectionNeverUpdated.Global

namespace EnhancedAI.Resources
{
    public class TeamAIOverrideDef : AIOverrideDef<AITeam>
    {
        public Dictionary<string, float> TurnOrderFactorWeights = new Dictionary<string, float>();
        public bool DesignateTargets = false;
    }
}
