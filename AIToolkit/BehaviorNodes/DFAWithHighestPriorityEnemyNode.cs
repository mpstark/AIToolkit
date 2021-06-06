using System.Collections.Generic;
using BattleTech;

public class DFAWithHighestPriorityEnemyNode : LeafBehaviorNode
{
	public DFAWithHighestPriorityEnemyNode(string name, BehaviorTree tree, AbstractActor unit)
		: base(name, tree, unit)
	{
	}

	private List<ICombatant> getAllDFATargetsForMech(Mech mech)
	{
		var allMiscCombatants = mech.Combat.GetAllMiscCombatants();
		var allActors = mech.Combat.AllActors;
		for (var i = 0; i < allActors.Count; i++)
		{
			var abstractActor = allActors[i];
			if (!abstractActor.IsDead && mech.CanDFATargetFromPosition(abstractActor, mech.CurrentPosition))
			{
				allMiscCombatants.Add(abstractActor);
			}
		}
		return allMiscCombatants;
	}

	public override BehaviorTreeResults Tick()
	{
		var mech = unit as Mech;
		var num = 0f;
		var num2 = 0.9f;
		if (mech != null)
		{
			num = AttackEvaluator.LegDamageLevel(mech);
		}
		if (mech == null || mech.WorkingJumpjets == 0 || tree.enemyUnits.Count == 0 || num >= num2)
		{
			return new BehaviorTreeResults(BehaviorNodeState.Failure);
		}
		var allDFATargetsForMech = getAllDFATargetsForMech(mech);
		for (var i = 0; i < tree.enemyUnits.Count; i++)
		{
			var combatant = tree.enemyUnits[i];
			if (!allDFATargetsForMech.Contains(combatant))
			{
				continue;
			}
			var abstractActor = combatant as AbstractActor;
			var behaviorTreeResults = new BehaviorTreeResults(BehaviorNodeState.Success);
			var attackOrderInfo = (AttackOrderInfo)(behaviorTreeResults.orderInfo = new AttackOrderInfo(combatant));
			attackOrderInfo.TargetUnit = combatant;
			attackOrderInfo.IsDeathFromAbove = true;
			attackOrderInfo.IsMelee = false;
			var dFADestsForTarget = mech.JumpPathing.GetDFADestsForTarget(abstractActor);
			if (dFADestsForTarget.Count == 0)
			{
				return new BehaviorTreeResults(BehaviorNodeState.Failure);
			}
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
			attackOrderInfo.Weapons.Remove(mech.DFAWeapon);
			attackOrderInfo.AttackFromLocation = mech.FindBestPositionToMeleeFrom(abstractActor, dFADestsForTarget);
			behaviorTreeResults.debugOrderString = name + " DFA attack at: " + combatant.DisplayName;
			return behaviorTreeResults;
		}
		return new BehaviorTreeResults(BehaviorNodeState.Failure);
	}
}
