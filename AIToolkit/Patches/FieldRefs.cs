using System.Collections.Generic;
using BattleTech;
using GraphCoroutines;
using static Harmony.AccessTools;

namespace AIToolkit.Patches
{
    public class FieldRefs
    {
        internal static readonly FieldRef<InfluenceMapEvaluator, GraphCoroutine> CoroutineRef = 
            FieldRefAccess<InfluenceMapEvaluator, GraphCoroutine>("evaluationCoroutine");

        internal static readonly FieldRef<InfluenceMapEvaluator, bool> EvaluationCompleteRef =
            FieldRefAccess<InfluenceMapEvaluator, bool>("evaluationComplete");

        internal static readonly FieldRef<InfluenceMapEvaluator, AbstractActor> UnitRef =
            FieldRefAccess<InfluenceMapEvaluator, AbstractActor>("unit");

        internal static readonly FieldRef<InfluenceMapEvaluator, InfluenceMapAllyFactor[]> AllyFactorsRef =
            FieldRefAccess<InfluenceMapEvaluator, InfluenceMapAllyFactor[]>("allyFactors");
        
        internal static readonly FieldRef<InfluenceMapEvaluator, InfluenceMapHostileFactor[]> HostileFactorsRef =
            FieldRefAccess<InfluenceMapEvaluator, InfluenceMapHostileFactor[]>("hostileFactors");
        
        internal static readonly FieldRef<InfluenceMapEvaluator, InfluenceMapPositionFactor[]> PositionalFactorsRef =
            FieldRefAccess<InfluenceMapEvaluator, InfluenceMapPositionFactor[]>("positionalFactors");
        
        internal static readonly FieldRef<BehaviorNode, string> BehaviorNodeNameRef =
            FieldRefAccess<BehaviorNode, string>("name");
        
        internal static readonly FieldRef<BehaviorNode, BehaviorVariableName> BehaviorVariableNameRef =
            FieldRefAccess<BehaviorNode, BehaviorVariableName>("bvName");
        
        internal static readonly FieldRef<Team, BehaviorVariableScope> BehaviorVariableRef =
            FieldRefAccess<Team, BehaviorVariableScope>("BehaviorVariables");
        
        internal static readonly FieldRef<BehaviorTree, BehaviorTreeIDEnum> BehaviorTreeIDEnumRef =
            FieldRefAccess<BehaviorTree, BehaviorTreeIDEnum>("behaviorTreeIDEnum");
        
        internal static readonly FieldRef<AITeam, AbstractActor> CurrentUnitRef =
            FieldRefAccess<AITeam, AbstractActor>("currentUnit");
        
        internal static readonly FieldRef<AITeam, List<InvocationMessage>> PendingInvocationsRef =
            FieldRefAccess<AITeam, List<InvocationMessage>>("pendingInvocations");
        
        
    }
}
