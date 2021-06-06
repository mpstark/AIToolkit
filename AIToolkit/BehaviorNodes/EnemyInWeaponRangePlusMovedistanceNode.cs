using BattleTech;
using UnityEngine;

public class EnemyInWeaponRangePlusMovedistanceNode : LeafBehaviorNode
{
    public EnemyInWeaponRangePlusMovedistanceNode(string name, BehaviorTree tree, AbstractActor unit)
        : base(name, tree, unit)
    {
    }

    public override BehaviorTreeResults Tick()
    {
        var maxWalkDistance = unit.MaxWalkDistance;
        var num = 0f;
        for (var i = 0; i < unit.Weapons.Count; i++)
        {
            var weapon = unit.Weapons[i];
            if (weapon.CanFire)
            {
                num = Mathf.Max(weapon.MaxRange, num);
            }
        }
        for (var j = 0; j < tree.enemyUnits.Count; j++)
        {
            var combatant = tree.enemyUnits[j];
            if (!combatant.IsDead && unit.team.VisibilityToTarget(combatant) != 0 && (combatant.CurrentPosition - unit.CurrentPosition).magnitude < num + maxWalkDistance)
            {
                return new BehaviorTreeResults(BehaviorNodeState.Success);
            }
        }
        return new BehaviorTreeResults(BehaviorNodeState.Failure);
    }
}
