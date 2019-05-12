using System.Collections.Generic;
using BattleTech;
using Harmony;
using UnityEngine;

namespace EnhancedAI.Features
{
    public static class AIDebugPause
    {
        private static AbstractActor _lastFocusedUnit;
        private static InvocationMessage _interceptedInvocationMessage;

        private static void MaybeFocusUnit(AbstractActor unit)
        {
            if (_lastFocusedUnit == unit)
                return;

            CameraControl.Instance.ForceMovingToGroundPos(unit.CurrentPosition);
            _lastFocusedUnit = unit;
        }

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
                return false;

            if (Main.Settings.FocusOnPause)
                MaybeFocusUnit(currentUnit);

            return true;
        }

        public static bool OnAIInvocation(InvocationMessage invocation)
        {
            if (!Main.Settings.ShouldPauseAI)
                return false;

            Main.HBSLog?.Log($"AIDebugPause: Intercepted an AI invocation: {invocation.InvocationID} ({invocation.MessageType})");
            _interceptedInvocationMessage = invocation;
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
    }
}
