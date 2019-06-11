using System.Collections.Generic;
using System.Linq;
using BattleTech;
using UnityEngine;

namespace AIToolkit.BehaviorNodes
{
    public class FilterToAdjacentSlowestMoveNode : LeafBehaviorNode
    {
        private float _distance;

        public FilterToAdjacentSlowestMoveNode(string name, BehaviorTree tree, AbstractActor unit, float distance) : base(name, tree, unit)
        {
            _distance = distance;
        }

        private static float GetMinWeaponRange(AbstractActor actor)
        {
            var minRange = float.MaxValue;
            foreach (var weapon in actor.Weapons)
            {
                if (!weapon.CanFire)
                    continue;

                minRange = Mathf.Min(minRange, weapon.LongRange);
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (minRange == float.MaxValue)
                minRange = 24f;

            return minRange;
        }

        private static Vector3? GetEstimatedPath(AbstractActor actor, out List<Vector3> path)
        {
            PathNode closestNode = null;
            var closestNodeDist = float.MaxValue;
            var destRadius = GetMinWeaponRange(actor);
            path = null;

            var enemies = actor.Combat.GetAllEnemiesOf(actor);
            if (enemies.Count == 0)
                Main.HBSLog?.LogWarning("FilterToAdjacentSlowestMoveNode: no enemies");

            foreach (var enemy in enemies)
            {
                if (enemy.IsDead)
                    continue;

                var clippedEnemyPos = RegionUtil.MaybeClipMovementDestinationToStayInsideRegion(actor, enemy.CurrentPosition);

                var curPath = DynamicLongRangePathfinder.GetPathToDestination(clippedEnemyPos,
                    actor.MovementCaps.MaxWalkDistance, actor, false, destRadius);
                if (curPath == null || curPath.Count == 0)
                {
                    Main.HBSLog?.LogWarning("FilterToAdjacentSlowestMoveNode: didn't get a path to unit");
                    continue;
                }

                var nodeOnCurPath = AIUtil.GetPrunedClosestValidPathNode(actor, actor.Pathing.CurrentGrid,
                    actor.Combat, actor.MovementCaps.MaxWalkDistance, curPath);
                if (nodeOnCurPath == null)
                {
                    Main.HBSLog?.LogWarning("FilterToAdjacentSlowestMoveNode: didn't get a node on the current path");
                    continue;
                }

                var distanceFromEnemy = Vector3.Distance(nodeOnCurPath.Position, clippedEnemyPos);
                if (distanceFromEnemy >= closestNodeDist)
                    continue;

                path = curPath;
                closestNode = nodeOnCurPath;
                closestNodeDist = distanceFromEnemy;
            }

            return closestNode?.Position;
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

            var mapMetaData = unit.Combat.MapMetaData;
            var avoidHexes = new HashSet<MapTerrainDataCell>
            {
                mapMetaData.GetCellAt(slowestUnit.CurrentPosition)
            };

            if (slowestUnit.Pathing == null)
                return new BehaviorTreeResults(BehaviorNodeState.Failure);

            // run pathing on slowest unit
            slowestUnit.Pathing.SetWalking();
            if (!slowestUnit.Pathing.ArePathGridsComplete)
                return new BehaviorTreeResults(BehaviorNodeState.Running);

            var potentialPos = GetEstimatedPath(slowestUnit, out var slowestUnitPath);
            if (potentialPos == null)
                return new BehaviorTreeResults(BehaviorNodeState.Failure);

            if (!slowestUnit.HasMovedThisRound)
                slowPosition = potentialPos.Value;

            avoidHexes.Add(mapMetaData.GetCellAt(potentialPos.Value));
            foreach (var pathPos in slowestUnitPath)
                avoidHexes.Add(mapMetaData.GetCellAt(pathPos));

            // filter to positions not on the path
            var filteredMoves = new List<MoveDestination>();
            foreach (var move in tree.movementCandidateLocations)
            {
                var hex = mapMetaData.GetCellAt(move.PathNode.Position);
                if (avoidHexes.Contains(hex))
                    continue;

                if (Vector3.Distance(move.PathNode.Position, slowPosition) > _distance)
                    continue;

                filteredMoves.Add(move);
            }

            tree.movementCandidateLocations = filteredMoves;
            return new BehaviorTreeResults(BehaviorNodeState.Success);
        }
    }
}
