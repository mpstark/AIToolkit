// AIToolkit.BehaviorNodes.MeleeWithEnemyNode
using System.Collections.Generic;
using BattleTech;

public class MeleeWithEnemyNode : LeafBehaviorNode
{
	public MeleeWithEnemyNode(string name, BehaviorTree tree, AbstractActor unit)
		: base(name, tree, unit)
	{
	}

	private List<ICombatant> getAllMeleeTargetsForMech(Mech mech)
	{
		var allMiscCombatants = mech.Combat.GetAllMiscCombatants();
		var allActors = mech.Combat.AllActors;
		for (var i = 0; i < allActors.Count; i++)
		{
			var abstractActor = allActors[i];
			if (!abstractActor.IsDead && mech.CanEngageTarget(abstractActor))
			{
				allMiscCombatants.Add(abstractActor);
			}
		}
		return allMiscCombatants;
	}

	protected override BehaviorTreeResults Tick()
	{
		var mech = unit as Mech;
		if (mech == null || tree.enemyUnits.Count == 0)
		{
			return new BehaviorTreeResults(BehaviorNodeState.Failure);
		}
		var allMeleeTargetsForMech = getAllMeleeTargetsForMech(mech);
		for (var i = 0; i < tree.enemyUnits.Count; i++)
		{
			var combatant = tree.enemyUnits[i];
			if (!allMeleeTargetsForMech.Contains(combatant))
			{
				continue;
			}
			var abstractActor = combatant as AbstractActor;
			var behaviorTreeResults = new BehaviorTreeResults(BehaviorNodeState.Success);
			var attackOrderInfo = (AttackOrderInfo)(behaviorTreeResults.orderInfo = new AttackOrderInfo(combatant));
			attackOrderInfo.TargetUnit = combatant;
			for (var j = 0; j < unit.Weapons.Count; j++)
			{
				foreach (var weapon in mech.Weapons)
				{
					if (weapon.WeaponCategoryValue.CanUseInMelee && weapon.CanFire)
					{
						attackOrderInfo.AddWeapon(weapon);
					}
				}
			}
			attackOrderInfo.Weapons.Add(mech.MeleeWeapon);
			attackOrderInfo.IsMelee = true;
			var meleeDestsForTarget = mech.Pathing.GetMeleeDestsForTarget(abstractActor);
			if (meleeDestsForTarget.Count == 0)
			{
				return new BehaviorTreeResults(BehaviorNodeState.Failure);
			}
			attackOrderInfo.AttackFromLocation = mech.FindBestPositionToMeleeFrom(abstractActor, meleeDestsForTarget);
			behaviorTreeResults.debugOrderString = name + " melee attack at: " + combatant.DisplayName;
			return behaviorTreeResults;
		}
		return new BehaviorTreeResults(BehaviorNodeState.Failure);
	}
}
