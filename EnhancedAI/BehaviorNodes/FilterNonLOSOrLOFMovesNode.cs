using System.Collections.Generic;
using BattleTech;
using UnityEngine;

namespace EnhancedAI.BehaviorNodes
{
    public class FilterNonLOSOrLOFMovesNode : LeafBehaviorNode
    {
        public FilterNonLOSOrLOFMovesNode(string name, BehaviorTree tree, AbstractActor unit) : base(name, tree, unit)
        {
        }

        // stolen from FiringPreviewManager -- not under license
        private bool HasLOS(AbstractActor attacker, ICombatant target, Vector3 position, List<AbstractActor> allies)
        {
            foreach (var ally in allies)
            {
                if (ally.VisibilityCache.VisibilityToTarget(target).VisibilityLevel == VisibilityLevel.LOSFull)
                    return true;
            }
            var visibility = attacker.Combat.LOS.GetVisibilityToTargetWithPositionsAndRotations(attacker, position, target);
            return visibility == VisibilityLevel.LOSFull;
        }

        protected override BehaviorTreeResults Tick()
        {
            // this is mostly taken from decompiled HBS code, not under license
            var allies = unit.Combat.GetAllAlliesOf(unit);

            var filteredMoves = new List<MoveDestination>();
            foreach (var move in tree.movementCandidateLocations)
            {
                var moveHasAttack = false;
                foreach (var enemy in tree.enemyUnits)
                {
                    var canSeeEnemy = HasLOS(unit, enemy, move.PathNode.Position, allies);
                    if (!canSeeEnemy)
                        break;

                    foreach (var weapon in unit.Weapons)
                    {
                        moveHasAttack = weapon.WillFireAtTargetFromPosition(enemy, move.PathNode.Position);
                        if (moveHasAttack)
                            break;
                    }

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
