// AIToolkit.BehaviorNodes.ProperlyFilterOutNonLOFMovesNode
using System.Collections.Generic;
using BattleTech;
using UnityEngine;

public class ProperlyFilterOutNonLOFMovesNode : LeafBehaviorNode
{
	public ProperlyFilterOutNonLOFMovesNode(string name, BehaviorTree tree, AbstractActor unit)
		: base(name, tree, unit)
	{
	}

	protected override BehaviorTreeResults Tick()
	{
		var list = new List<MoveDestination>();
		for (var i = 0; i < tree.movementCandidateLocations.Count; i++)
		{
			var moveDestination = tree.movementCandidateLocations[i];
			var flag = false;
			for (var j = 0; j < unit.Weapons.Count; j++)
			{
				var weapon = unit.Weapons[j];
				for (var k = 0; k < unit.BehaviorTree.enemyUnits.Count; k++)
				{
					var combatant = unit.BehaviorTree.enemyUnits[k];
					Vector3 collisionWorldPos;
					var lineOfFire = unit.Combat.LOS.GetLineOfFire(unit, moveDestination.PathNode.Position, combatant, combatant.CurrentPosition, combatant.CurrentRotation, out collisionWorldPos);
					var quaternion = Quaternion.Euler(0f, PathingUtil.FloatAngleFrom8Angle(moveDestination.PathNode.Angle), 0f);
					if ((weapon.WillFireAtTargetFromPosition(combatant, moveDestination.PathNode.Position, quaternion) && unit.HasLOFToTargetUnitAtTargetPosition(combatant, weapon.MaxRange, moveDestination.PathNode.Position, quaternion, combatant.CurrentPosition, combatant.CurrentRotation, isIndirectFireCapable: false) && lineOfFire > LineOfFireLevel.LOFBlocked) || (weapon.IndirectFireCapable && weapon.WillFireAtTargetFromPosition(combatant, moveDestination.PathNode.Position, quaternion) && unit.HasIndirectLOFToTargetUnit(moveDestination.PathNode.Position, quaternion, combatant, enabledWeaponsOnly: true)))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
			if (flag)
			{
				list.Add(moveDestination);
			}
		}
		tree.movementCandidateLocations = list;
		return BehaviorTreeResults.BehaviorTreeResultsFromBoolean(success: true);
	}
}
