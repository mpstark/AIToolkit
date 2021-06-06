// AIToolkit.BehaviorNodes.TestFilterNode

using System.Collections.Generic;
using BattleTech;
using UnityEngine;

public class TestFilterNode : LeafBehaviorNode
{
	public TestFilterNode(string name, BehaviorTree tree, AbstractActor unit)
		: base(name, tree, unit)
	{
	}

	private static bool HasLOS(AbstractActor attacker, ICombatant target, Vector3 position, List<AbstractActor> allies)
	{
		foreach (var ally in allies)
		{
			if (ally.VisibilityCache.VisibilityToTarget(target).VisibilityLevel == VisibilityLevel.LOSFull)
			{
				return true;
			}
		}
		return attacker.Combat.LOS.GetVisibilityToTargetWithPositionsAndRotations(attacker, position, target) == VisibilityLevel.LOSFull;
	}

	private static bool HasAttack(AbstractActor attacker, ICombatant target, Vector3 position)
	{
		var lineOfFire = attacker.Combat.LOS.GetLineOfFire(attacker, position, target, target.CurrentPosition, target.CurrentRotation, out _);
		if (attacker.GetLongestRangeWeapon(enabledWeaponsOnly: false, indirectFireOnly: true) == null || lineOfFire <= LineOfFireLevel.LOFBlocked)
		{
			return false;
		}
		var num = Vector3.Distance(position, target.CurrentPosition);
		var maxRange = attacker.GetLongestRangeWeapon(enabledWeaponsOnly: false).MaxRange;
		return num <= maxRange;
	}

	public override BehaviorTreeResults Tick()
	{
		var allAlliesOf = unit.Combat.GetAllAlliesOf(unit);
		var list = new List<MoveDestination>();
		foreach (var movementCandidateLocation in tree.movementCandidateLocations)
		{
			var flag = false;
			foreach (var enemyUnit in tree.enemyUnits)
			{
				if (!HasLOS(unit, enemyUnit, movementCandidateLocation.PathNode.Position, allAlliesOf))
				{
					break;
				}
				flag = HasAttack(unit, enemyUnit, movementCandidateLocation.PathNode.Position);
				if (flag)
				{
					break;
				}
			}
			if (flag)
			{
				list.Add(movementCandidateLocation);
			}
		}
		tree.movementCandidateLocations = list;
		return new BehaviorTreeResults(BehaviorNodeState.Success);
	}
}
