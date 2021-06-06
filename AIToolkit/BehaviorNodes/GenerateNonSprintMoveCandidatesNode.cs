using BattleTech;

namespace AIToolkit.BehaviorNodes
{
    public class GenerateNonSprintMoveCandidatesNode : LeafBehaviorNode
    {
        public GenerateNonSprintMoveCandidatesNode(string name, BehaviorTree tree, AbstractActor unit) : base(name, tree, unit)
        {
        }

        public override BehaviorTreeResults Tick()
        {
            var moveTypes = new[] { MoveType.Backward, MoveType.Walking };
            foreach (var moveType in moveTypes)
            {
                var pathNodes = unit.Pathing.getGrid(moveType).GetSampledPathNodes();
                foreach (var pathNode in pathNodes)
                {
                    if (pathNode == null)
                        continue;

                    tree.movementCandidateLocations.Add(new MoveDestination(pathNode, moveType));
                }
            }

            if (!(unit is Mech mech) || mech.WorkingJumpjets == 0)
                return new BehaviorTreeResults(BehaviorNodeState.Success);

            var jumpPathNodes = mech.JumpPathing.GetSampledPathNodes();
            foreach (var jumpPathNode in jumpPathNodes)
            {
                if (jumpPathNode == null)
                    continue;

                tree.movementCandidateLocations.Add(new MoveDestination(jumpPathNode, MoveType.Jumping));
            }

            return new BehaviorTreeResults(BehaviorNodeState.Success);
        }
    }
}
