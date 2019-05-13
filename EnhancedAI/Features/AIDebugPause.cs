using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Rendering.UI;
using BattleTech.UI;
using Harmony;
using UnityEngine;
using UnityEngine.Rendering;

// ReSharper disable AccessToStaticMemberViaDerivedType

namespace EnhancedAI.Features
{
    public static class AIDebugPause
    {
        public static bool IsPaused { get; private set; }

        private static InvocationMessage _interceptedInvocationMessage;

        private static GameObject _mechMovementVisualization;
        private static GameObject _veeMovementVisualization;

        private static LineRenderer _attackLine;
        private static LineRenderer _movementLine;
        private static Vector3 _movementLineGroundOffset = new Vector3(0f, 1f, 0f);


        public static bool ShouldSkipAIThink(AITeam team)
        {
            if (!Main.Settings.ShouldPauseAI)
                return false;

            // don't pause if the current unit has already activated this round
            var currentUnit = Traverse.Create(team).Field("currentUnit").GetValue<AbstractActor>();
            if (currentUnit.HasActivatedThisRound)
                return false;

            // don't pause if pending invocations (we want to only pause before new invocation)
            var pendingInvocations = Traverse.Create(team).Field("pendingInvocations").GetValue<List<InvocationMessage>>();
            if (pendingInvocations != null && pendingInvocations.Count > 0)
                return false;

            // don't pause if we don't have an intercepted message
            if (_interceptedInvocationMessage == null)
                return false;

            // unpause when press the keys
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.RightArrow))
            {
                OnUnpause();
                return false;
            }

            if (!IsPaused)
                OnPause();

            return true;
        }

        public static bool OnAIInvocation(AITeam aiTeam, InvocationMessage invocation)
        {
            if (!Main.Settings.ShouldPauseAI)
                return false;

            Main.HBSLog?.Log($"AIDebugPause: Intercepted an AI invocation: {invocation.InvocationID} ({invocation.MessageType})");
            _interceptedInvocationMessage = invocation;

            ParseInvocationMessage(aiTeam.Combat, invocation);

            return true;
        }

        public static InvocationMessage TryGetInjectInvocationMessage()
        {
            if (_interceptedInvocationMessage == null)
                return null;

            Main.HBSLog?.Log($"AIDebugPause: Injecting an AI invocation: {_interceptedInvocationMessage.InvocationID} ({_interceptedInvocationMessage.MessageType})");

            var message = _interceptedInvocationMessage;
            _interceptedInvocationMessage = null;

            return message;
        }


        private static void OnPause()
        {
            IsPaused = true;

            //if (Main.Settings.FocusOnPause)
            //    MaybeFocusUnit(currentUnit);
        }

        private static void OnUnpause()
        {
            IsPaused = false;

            if (_movementLine != null)
                _movementLine.gameObject.SetActive(false);

            if (_attackLine != null)
                _attackLine.gameObject.SetActive(false);

            if (_mechMovementVisualization != null)
                _mechMovementVisualization.SetActive(false);

            if (_veeMovementVisualization != null)
                _veeMovementVisualization.SetActive(false);
        }


        private static Vector3[] GetPointsForWaypoints(List<WayPoint> waypoints)
        {
            if (waypoints == null || waypoints.Count <= 1)
                return null;

            return waypoints.Select(p => p.Position + _movementLineGroundOffset).ToArray();
        }

        private static GameObject GetMovementVisualization(AbstractActor unit)
        {
            switch (unit)
            {
                case Mech mech:
                {
                    if (_mechMovementVisualization == null)
                        _mechMovementVisualization = GameObject.Instantiate(mech.GameRep.BlipObjectIdentified, null, true);

                    return _mechMovementVisualization;
                }
                case Vehicle vee:
                {
                    if (_veeMovementVisualization == null)
                        _veeMovementVisualization = GameObject.Instantiate(vee.GameRep.BlipObjectIdentified, null, true);

                    return _veeMovementVisualization;
                }
            }

            return null;
        }


        private static LineRenderer InitLineObject(string name, float width, Color color)
        {
            var gameObject = new GameObject(name);
            var line = gameObject.AddComponent<LineRenderer>();

            var existingLineRenderer = CombatMovementReticle.Instance.pathManager.badPathTutorialLine.line;
            line.sharedMaterial = existingLineRenderer.sharedMaterial;

            line.startWidth = width;
            line.endWidth = width;
            line.receiveShadows = false;
            line.shadowCastingMode = ShadowCastingMode.Off;

            line.startColor = color;
            line.endColor = color;

            gameObject.AddComponent<UISweep>();

            return line;
        }

        private static void DrawMovementLine(Vector3[] linePoints)
        {
            if (linePoints == null || linePoints.Length == 0)
                return;

            if (_movementLine == null)
                _movementLine = InitLineObject("AIDebugMovementLine", 0.3f, Color.white);

            _movementLine.gameObject.SetActive(true);
            _movementLine.positionCount = linePoints.Length;
            _movementLine.SetPositions(linePoints);
        }

        private static void DrawAttackLine(Vector3 from, Vector3 to, bool isIndirect)
        {
            if (_attackLine == null)
                _attackLine = InitLineObject("AIDebugAttackLine", 0.5f, Color.red);

            _attackLine.gameObject.SetActive(true);

            if (isIndirect)
            {
                var pointsForArc = WeaponRangeIndicators.GetPointsForArc(18, 30f, from, to);
                _attackLine.positionCount = 18;
                _attackLine.SetPositions(pointsForArc);
                return;
            }

            _attackLine.positionCount = 2;
            _attackLine.SetPositions(new []{from, to});
        }


        private static void ParseInvocationMessage(CombatGameState combat, InvocationMessage message)
        {
            switch (message)
            {
                case AbstractActorMovementInvocation move:
                {
                    var unit = combat.FindActorByGUID(move.ActorGUID);
                    var finalPosition = unit.CurrentPosition;
                    var rotation = Quaternion.LookRotation(move.FinalOrientation, Vector3.up);

                    if (move.Waypoints != null && move.Waypoints.Count > 0)
                        finalPosition = move.Waypoints.Last().Position;

                    var linePoints = GetPointsForWaypoints(move.Waypoints);
                    VisualizeMovement(unit, finalPosition, rotation, linePoints);
                    break;
                }

                case MechJumpInvocation jump:
                {
                    var unit = combat.FindActorByGUID(jump.MechGUID);
                    var finalPosition = jump.FinalDestination;
                    var minArcHeight = Mathf.Max(Mathf.Abs(finalPosition.y - unit.CurrentPosition.y) + 16f, 32f);
                    var linePoints = WeaponRangeIndicators.GetPointsForArc(18, minArcHeight, unit.CurrentPosition + _movementLineGroundOffset, finalPosition + _movementLineGroundOffset);

                    VisualizeMovement(unit, finalPosition, jump.FinalRotation, linePoints);
                    break;
                }

                case AttackInvocation attack:
                {
                    var unit = combat.FindActorByGUID(attack.SourceGUID);
                    var subAttack = attack.subAttackInvocations[0];
                    var target = combat.FindActorByGUID(subAttack.targetGUID);

                    VisualizeAttack(unit, target, false, !unit.HasLOFToTargetUnit(target, float.MaxValue, false));
                    break;
                }
            }
        }

        private static void VisualizeMovement(AbstractActor unit, Vector3 finalPosition, Quaternion rotation, Vector3[] linePoints)
        {
            // draw end point visualization
            var visualization = GetMovementVisualization(unit);
            visualization.transform.position = finalPosition;
            visualization.transform.rotation = rotation;
            visualization.SetActive(true);

            DrawMovementLine(linePoints);

            if (Main.Settings.FocusOnPause)
                CameraControl.Instance.ForceMovingToGroundPos(finalPosition);
        }

        private static void VisualizeAttack(AbstractActor unit, AbstractActor target, bool isMeleeAttack, bool isIndirect)
        {
            var targetPosition = target.CurrentPosition;
            if (!isMeleeAttack)
                targetPosition = target.TargetPosition;

            DrawAttackLine(unit.CurrentPosition + unit.HighestLOSPosition, targetPosition, isIndirect);

            if (Main.Settings.FocusOnPause)
                CameraControl.Instance.ForceMovingToGroundPos(target.CurrentPosition);
        }
    }
}
