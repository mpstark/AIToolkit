# Notes

These are based on BattleTech 1.5x -- more work will be done after BattleTech 1.6 is out.

## Some Thoughts About Current AI Flaws

* AI doesn't seem to consider evasion from movement almost at all
  * Evasion pips are vital to light/medium mech survival
  * Maybe add an influence map factor similar to that for guarded
* AI is very disconnected between Movement and Attack
  * Can decide to move to locations that allow for limited/no attacks
  * Perhaps change the logic in tree to decide if will attack, and maybe if will attack, before doing movement
* Sensor Locking when it doens't strip any evasion and has LOS
  * Logic for sensor lock seems very complicated and not effective
  * To me, sensor lock decision is..
    * Gain los if hiding or cannot gain LOS this turn
    * Strip 2x evasion pips if 1x strip + the damage from attacks is outweighed, or if that mech is hiding or cannot engage this turn
* Lance level decisions are effectively nil, this is likely intentional
  * Target coordination would go a long way -- this is all implicit right now
  * Even just changing the order that mechs go in would be a large benefit
* "Aggressive" moves are straight toward the player's unit
  * Only "defensive" moves use the influence map
  * Does not consider anything other than a straight move
  * Should consider a couple different locations using influence map
* AI will never wait in positions of advantage
  * Unsure if this should be included, the purpose of the AI is to provide fun
* Handing of heat is very strange, with "acceptable heat"
  * Simplification of this behavior, maybe acceptable damage/heat for different levels?
* Coolent Vent logic is only before an attack that would go above "acceptable heat"
  * Most players will use coolent vent on cooldown when the extra heat capacity wouldn't be wasted
* Any other active abilities are not possible within the current framework of the game

## BehaviorNodes that return order infos:

* BraceNode
* MechStartUpNode
* StandNode
* SensorLockRecordedSensorLockTargetNode
* MeleeWithHighestPriorityEnemyNode
* ExecuteStationaryAttackNode
* ShootAtHighestPriorityEnemyNode
* MoveAlongRouteNode
* MoveTowardHighestPriorityMoveCandidateNode
* MoveInsideEncounterBoundsNode
* MoveToDestinationNode
* MoveToStayInsideRegionNode
* MoveTowardsHighestPriorityEnemyLastSeenLocationNode
* MoveTowardsHighestPriorityEnemyNode
* MoveTowardsHighestPriorityMoveCandidateNode
* ClaimInspirationNode
* ShootTrainingWeaponsAtTargetNode
