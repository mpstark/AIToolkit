using System.Collections.Generic;
using BattleTech;
using UnityEngine;

namespace AIToolkit.BehaviorNodes
{
    public class FilterNonLOSOrLOFMovesNode : LeafBehaviorNode
    {
        public FilterNonLOSOrLOFMovesNode(string name, BehaviorTree tree, AbstractActor unit) : base(name, tree, unit)
        {
        }

        // stolen from FiringPreviewManager -- not under license
        private static bool HasLOS(AbstractActor attacker, ICombatant target, Vector3 position, List<AbstractActor> allies)
        {
            foreach (var ally in allies)
            {
                if (ally.VisibilityCache.VisibilityToTarget(target).VisibilityLevel == VisibilityLevel.LOSFull)
                    return true;
            }
            var visibility = attacker.Combat.LOS.GetVisibilityToTargetWithPositionsAndRotations(attacker, position, target);
            return visibility == VisibilityLevel.LOSFull;
        }

        private static bool HasAttack(AbstractActor attacker, ICombatant target, Vector3 position)
        {
            var lof = attacker.Combat.LOS.GetLineOfFire(attacker, position, target,
                target.CurrentPosition, target.CurrentRotation, out _);

            // it's hard to get if a MoveDestination has enough turn left
            // this will be a little "incorrect" because it won't check rotation
            // but that's better than other options

            var longestIndirect = attacker.GetLongestRangeWeapon(false, true);
            if (longestIndirect == null && lof <= LineOfFireLevel.LOFBlocked)
                return false;

            var distance = Vector3.Distance(position, target.CurrentPosition);
            var maxRange = attacker.GetLongestRangeWeapon(false).MaxRange;
            if (distance > maxRange)
                return false;

            return true;
        }

        public override BehaviorTreeResults Tick()
        {
            // this is mostly taken from decompiled HBS code, not under license
            var allies = unit.Combat.GetAllAlliesOf(unit);

            var filteredMoves = new List<MoveDestination>();
            foreach (var move in tree.movementCandidateLocations)
            {
                var moveHasAttack = false;
                foreach (var enemy in tree.enemyUnits)
                {
                    if (!HasLOS(unit, enemy, move.PathNode.Position, allies))
                        break;

                    moveHasAttack = HasAttack(unit, enemy, move.PathNode.Position);
                    if (moveHasAttack)
                        break;
                }

                if (moveHasAttack)
                    filteredMoves.Add(move);
            }

            tree.movementCandidateLocations = filteredMoves;
            return new BehaviorTreeResults(BehaviorNodeState.Success);
        }
    }
}
