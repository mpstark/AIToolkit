using BattleTech;
using UnityEngine;

namespace AIToolkit.BehaviorNodes.Orders
{
    public class PilotAbilityInvocation : InvocationMessage
    {
        private AbstractActor _source;
        private ICombatant _target;
        private string _abilityID;

        // there are no subscribers to this messageType, so it doesn't matter what it is
        public override MessageCenterMessageType MessageType => MessageCenterMessageType.SensorLockInvocation;

        public PilotAbilityInvocation(AbstractActor source, ICombatant target, string abilityID) : base(Random.Range(0, 99999))
        {
            _source = source;
            _target = target;
            _abilityID = abilityID;
        }

        public override bool Invoke(CombatGameState combatGameState)
        {
            if (string.IsNullOrEmpty(_abilityID) || _source == null)
                return false;

            var pilot = _source.GetPilot();
            if (pilot == null || !pilot.ActiveAbilities.Exists(a => a.Def.Id == _abilityID))
                return false;

            var targetGUID = _target != null ? _target.GUID : _source.GUID;
            pilot.ActivateAbility(_source, _abilityID, targetGUID);

            return true;
        }
    }
}
