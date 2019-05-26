using System.Collections.Generic;

namespace EnhancedAI.Resources
{
    public class TeamAIOverrideDef : AIOverrideDef<AITeam>
    {
        public Dictionary<string, float> TurnOrderFactorWeights = new Dictionary<string, float>();
    }
}
