using System.Collections.Generic;
using BattleTech;
using AIToolkit.Features.UI;
using Harmony;
using UnityEngine;

// ReSharper disable AccessToStaticMemberViaDerivedType

namespace AIToolkit.Features
{
    public static class AIPause
    {
        public static AITeam CurrentAITeam { get; private set; }

        public static bool IsPaused { get; private set; }

        public static TextPopup PausePopup =>
            _pausePopup ?? (_pausePopup = new TextPopup("AIToolkitPausePopup", false));
        public static InfluenceMapVisualization InfluenceMapVisual =>
            _influenceMapVisual ?? (_influenceMapVisual = new InfluenceMapVisualization("AIToolkitPauseInfluenceMapVisual"));
        public static InvocationVisualization InvocationVisual =>
            _invocationVisual ?? (_invocationVisual = new InvocationVisualization("AIToolkitPauseInvocationVisual"));

        private static InvocationMessage _interceptedInvocationMessage;
        private static TextPopup _pausePopup;
        private static InfluenceMapVisualization _influenceMapVisual;
        private static InvocationVisualization _invocationVisual;


        public static void DestroyUI()
        {
            if (PausePopup != null)
                GameObject.Destroy(PausePopup.ParentObject);

            if (InfluenceMapVisual != null)
                GameObject.Destroy(InfluenceMapVisual.ParentObject);

            if (InvocationVisual != null)
                GameObject.Destroy(InvocationVisual.ParentObject);

            _pausePopup = null;
            _influenceMapVisual = null;
            _invocationVisual = null;

            Reset();
        }

        public static void Reset()
        {
            if (!IsPaused)
                return;

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
            var pendingInvocations =
                Traverse.Create(team).Field("pendingInvocations").GetValue<List<InvocationMessage>>();
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

            if (invocation is EjectInvocation)
                return false;

            if (invocation is InspireActorInvocation)
                return false;

            _interceptedInvocationMessage = invocation;
            return true;
        }

        public static InvocationMessage TryGetMessageInject()
        {
            if (_interceptedInvocationMessage == null)
                return null;

            var message = _interceptedInvocationMessage;
            _interceptedInvocationMessage = null;

            return message;
        }


        private static void OnPause(AITeam team)
        {
            Main.HBSLog?.Log($"AIDebugPause -- Paused on {_interceptedInvocationMessage.GetType().Name}");
            IsPaused = true;

            CurrentAITeam = team;

            InvocationVisual.ShowFor(team.Combat, _interceptedInvocationMessage);
            InfluenceMapVisual.Show();
        }

        private static void OnUnpause()
        {
            Main.HBSLog?.Log("AIDebugPause -- Unpaused");
            IsPaused = false;

            InvocationVisual.Hide();
            InfluenceMapVisual.Hide();
            PausePopup.Hide();
        }
    }
}
