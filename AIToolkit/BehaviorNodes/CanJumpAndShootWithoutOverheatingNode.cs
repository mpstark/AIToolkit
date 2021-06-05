using BattleTech;

namespace AIToolkit.BehaviorNodes
{
	public class CanJumpAndShootWithoutOverheatingNode : LeafBehaviorNode
	{
		public CanJumpAndShootWithoutOverheatingNode(string name, BehaviorTree tree, AbstractActor unit)
			: base(name, tree, unit)
		{
		}

		protected override BehaviorTreeResults Tick()
		{
			var mech = unit as Mech;
			if (mech == null || mech.WorkingJumpjets == 0)
			{
				return new BehaviorTreeResults(BehaviorNodeState.Failure);
			}
			for (var i = 0; i < unit.Weapons.Count; i++)
			{
				var heatGenerated = mech.Weapons[i].HeatGenerated;
				var jumpDistance = mech.JumpDistance;
				float num = mech.CalcJumpHeat(jumpDistance);
				if ((float)mech.CurrentHeat + heatGenerated + num < AIUtil.GetAcceptableHeatLevelForMech(mech))
				{
					return new BehaviorTreeResults(BehaviorNodeState.Success);
				}
			}
			return new BehaviorTreeResults(BehaviorNodeState.Failure);

		}
	}
}
