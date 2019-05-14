using System.Collections.Generic;
using BattleTech;
using Harmony;
using UnityEngine;

// ReSharper disable AccessToStaticMemberViaDerivedType

namespace EnhancedAI.Features
{
    public static class AIDebugPause
    {
        public static bool IsPaused { get; private set; }

        private static AITeam _currentAITeam;
        private static InvocationMessage _interceptedInvocationMessage;


        public static void Reset()
        {
            if (!IsPaused)
                return;

            if (_currentAITeam != null)
            {
                var currentUnit = Traverse.Create(_currentAITeam).Field("currentUnit").GetValue<AbstractActor>();
                currentUnit.BehaviorTree.Reset();
                currentUnit.BehaviorTree.influenceMapEvaluator.ResetWorkspace();
                currentUnit.BehaviorTree.influenceMapEvaluator.Reset();
            }

            _interceptedInvocationMessage = null;
            OnUnpause();
        }

        public static bool OnAIThink(AITeam team)
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
                OnPause(team);

            return true;
        }

        public static bool OnAIInvocation(AITeam aiTeam, InvocationMessage invocation)
        {
            if (!Main.Settings.ShouldPauseAI)
                return false;

            // TODO: provide an option for what invocations to pause on
            if (invocation is ReserveActorInvocation)
                return false;

            Main.HBSLog?.Log($"AIDebugPause: Intercepted an AI invocation: {invocation.InvocationID} ({invocation.MessageType})");
            _interceptedInvocationMessage = invocation;

            InvocationVisualization.ShowFor(aiTeam.Combat, invocation);
            return true;
        }

        public static InvocationMessage TryGetMessageInject()
        {
            if (_interceptedInvocationMessage == null)
                return null;

            Main.HBSLog?.Log($"AIDebugPause: Injecting an AI invocation: {_interceptedInvocationMessage.InvocationID} ({_interceptedInvocationMessage.MessageType})");

            var message = _interceptedInvocationMessage;
            _interceptedInvocationMessage = null;

            return message;
        }


        private static void OnPause(AITeam team)
        {
            Main.HBSLog?.Log("AIDebugPause -- Paused");

            InfluenceMapVisualization.Show();

            IsPaused = true;
            _currentAITeam = team;
        }

        private static void OnUnpause()
        {
            Main.HBSLog?.Log("AIDebugPause -- Unpaused");

            IsPaused = false;
            _currentAITeam = null;

           InfluenceMapVisualization.Hide();
           InvocationVisualization.Hide();
        }
    }
}
