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
    }
}
