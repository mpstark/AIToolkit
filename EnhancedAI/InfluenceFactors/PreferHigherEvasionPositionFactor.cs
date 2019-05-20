using BattleTech;
using UnityEngine;

namespace EnhancedAI.InfluenceFactors
{
    public class PreferHigherEvasionPositionFactor : InfluenceMapPositionFactor
    {
        public override string Name { get; } = "preferHigherEvasion";

        public override BehaviorVariableName GetRegularMoveWeightBVName()
        {
            return BehaviorVariableName.Float_CenterTorsoArmorMultiplier;
        }

        public override BehaviorVariableName GetSprintMoveWeightBVName()
        {
            return BehaviorVariableName.Float_CenterTorsoArmorMultiplier;
        }

        public override float EvaluateInfluenceMapFactorAtPosition(AbstractActor unit, Vector3 position, float angle, MoveType moveType,
            PathNode pathNode)
        {
            if (moveType == MoveType.None)
                return 0f;

            var distance = 0f;
            var rotation = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            var newPathNode = unit.Pathing.UpdateAIPath(position, rotation, moveType);

            if (moveType != MoveType.Jumping && newPathNode != null)
            {
                // get the path distance to find out how many pips will be generated
                var curPosition = unit.CurrentPosition;
                foreach (var node in unit.Pathing.CurrentPath)
                {
                    distance += Vector3.Distance(curPosition, node.Position);
                    curPosition = node.Position;
                }
            }
            else
            {
                distance = Vector3.Distance(unit.CurrentPosition, position);
            }

            var pipsGenerated = unit.GetEvasivePipsResult(distance, moveType == MoveType.Jumping, moveType == MoveType.Sprinting,
                moveType == MoveType.Melee);

            return pipsGenerated;
        }
    }
}
