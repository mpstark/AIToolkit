using AIToolkit.Util;
using BattleTech;

namespace AIToolkit.TurnOrderFactors
{
    public class DistanceAlongPatrolRoute : ITurnOrderFactor
    {
        public float EvaluateUnit(AbstractActor unit)
        {
            if (unit.IsDead)
                return float.MinValue;

            var routeGUID = unit.BehaviorTree .GetBVValue(BehaviorVariableName.String_RouteGUID).StringVal;
            if (string.IsNullOrEmpty(routeGUID))
                return float.MinValue;

            // this is taken from decompiled HBS code, and not subject to license
            var route = RoutingUtil.FindRouteByGUID(unit.BehaviorTree, routeGUID);
            var routeCompleted = unit.BehaviorTree .GetBVValue(BehaviorVariableName.Bool_RouteCompleted).BoolVal;
            if (routeCompleted)
                return route.routePointList.Length;

            var isForward = unit.BehaviorTree.GetBVValue(BehaviorVariableName.Bool_RouteFollowingForward).BoolVal;
            var nextRouteIndex = unit.BehaviorTree.GetBVValue(BehaviorVariableName.Int_RouteTargetPoint).IntVal;

            var num = 0f;
            if (!isForward)
            {
                num += route.routePointList.Length;
                num += route.routePointList.Length - nextRouteIndex;
            }
            else
            {
                num += nextRouteIndex;
            }

            var lastRouteIndex = nextRouteIndex + (!isForward ? 1 : -1);
            if (lastRouteIndex < 0)
                lastRouteIndex = 1;

            if (lastRouteIndex >= route.routePointList.Length)
                lastRouteIndex = route.routePointList.Length - 1;

            var magnitude = (route.routePointList[nextRouteIndex].transform.position - unit.CurrentPosition).magnitude;
            var magnitude2 = (route.routePointList[lastRouteIndex].transform.position - unit.CurrentPosition).magnitude;
            var num3 = magnitude + magnitude2;
            var num4 = num3 <= 0f ? 0f : magnitude2 / num3;
            return num + num4;

            // This was my untested attempt at it, I gave up and used the game's version
            //var lastRouteIndex = Mathf.Clamp(nextRouteIndex + (isForward ? -1 : 1), 0, route.routePointList.Length - 1);

            //var nextPoint = route.routePointList[nextRouteIndex].transform.position;
            //var lastPoint = route.routePointList[lastRouteIndex].transform.position;

            //var distanceBetween = Vector3.Distance(nextPoint, lastPoint);
            //var distanceAway = Vector3.Distance(lastPoint, unit.CurrentPosition);

            //if (distanceAway > distanceBetween)
            //    distanceAway = distanceBetween;

            //var percentTowards = distanceAway / distanceBetween;
            //return lastRouteIndex + percentTowards;
        }
    }
}
