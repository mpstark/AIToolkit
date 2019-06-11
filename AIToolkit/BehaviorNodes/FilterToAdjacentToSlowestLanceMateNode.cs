using System.Collections.Generic;
using System.Linq;
using BattleTech;
using UnityEngine;

namespace AIToolkit.BehaviorNodes
{
    public class FilterToAdjacentToSlowestLanceMateNode : LeafBehaviorNode
    {
        private float _distance;

        public FilterToAdjacentToSlowestLanceMateNode(string name, BehaviorTree tree, AbstractActor unit, float distance) : base(name, tree, unit)
        {
            _distance = distance;
        }

        protected override BehaviorTreeResults Tick()
        {
            // don't have lance
            if (unit.lance == null)
                return new BehaviorTreeResults(BehaviorNodeState.Failure);

            var lanceUnits = unit.lance.unitGuids
                .Select(guid => unit.Combat.FindActorByGUID(guid))
                .Where(u => !u.IsDead)
                .ToArray();

            // solo in lance
            if (lanceUnits.Length <= 1)
                return new BehaviorTreeResults(BehaviorNodeState.Failure);

            var minSpeed = lanceUnits.Min(u => u.MovementCaps.MaxWalkDistance);
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            var slowestUnit = lanceUnits.First(u => minSpeed == u.MovementCaps.MaxWalkDistance);
            var slowPosition = slowestUnit.CurrentPosition;

            // filter to positions not around the slowest mech
            var filteredMoves = new List<MoveDestination>();
            foreach (var move in tree.movementCandidateLocations)
            {
                if (Vector3.Distance(move.PathNode.Position, slowPosition) > _distance)
                    continue;

                filteredMoves.Add(move);
            }

            tree.movementCandidateLocations = filteredMoves;
            return new BehaviorTreeResults(BehaviorNodeState.Success);
        }
    }
}
