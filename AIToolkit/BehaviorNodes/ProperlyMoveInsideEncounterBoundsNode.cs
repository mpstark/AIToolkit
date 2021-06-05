// AIToolkit.BehaviorNodes.ProperlyMoveInsideEncounterBoundsNode
using System.Collections.Generic;
using BattleTech;
using BattleTech.Designed;
using UnityEngine;

public class ProperlyMoveInsideEncounterBoundsNode : LeafBehaviorNode
{
	public ProperlyMoveInsideEncounterBoundsNode(string name, BehaviorTree tree, AbstractActor unit)
		: base(name, tree, unit)
	{
	}

	protected override BehaviorTreeResults Tick()
	{
		var encounterBoundaryChunk = unit.Combat.EncounterLayerData.encounterBoundaryChunk;
		if (encounterBoundaryChunk.IsInEncounterBounds(unit.CurrentPosition))
		{
			return new BehaviorTreeResults(BehaviorNodeState.Success);
		}
		var zero = Vector3.zero;
		if (encounterBoundaryChunk.encounterBoundaryRectList.Count == 0)
		{
			return new BehaviorTreeResults(BehaviorNodeState.Failure);
		}
		unit.Pathing.UpdateAIPath(zero, zero, MoveType.Sprinting);
		var vector = unit.Pathing.ResultDestination;
		var num = float.MaxValue;
		var currentGrid = unit.Pathing.CurrentGrid;
		var lookAt = zero;
		if ((currentGrid.GetValidPathNodeAt(vector, num) == null || (vector - zero).magnitude > 1f) && unit.Combat.EncounterLayerData.inclineMeshData != null)
		{
			var lanceUnits = AIUtil.GetLanceUnits(unit.Combat, unit.LanceId);
			var dynamicPathToDestination = DynamicLongRangePathfinder.GetDynamicPathToDestination(vector, num, unit, shouldSprint: true, lanceUnits, unit.Pathing.CurrentGrid, 0f);
			if (dynamicPathToDestination != null && dynamicPathToDestination.Count > 0)
			{
				vector = dynamicPathToDestination[dynamicPathToDestination.Count - 1];
			}
		}
		var currentPosition = unit.CurrentPosition;
		AIUtil.LogAI($"issuing order from [{currentPosition.x} {currentPosition.y} {currentPosition.z}] to [{vector.x} {vector.y} {vector.z}] looking at [{lookAt.x} {lookAt.y} {lookAt.z}]");
		var behaviorTreeResults = new BehaviorTreeResults(BehaviorNodeState.Success);
		behaviorTreeResults.debugOrderString = string.Format(arg2: ((MovementOrderInfo)(behaviorTreeResults.orderInfo = new MovementOrderInfo(vector, lookAt)
		{
			IsSprinting = true
		})).IsSprinting, format: "{0}: dest:{1} sprint:{2}", arg0: name, arg1: zero);
		return behaviorTreeResults;
	}
}
