# EnhancedAI

Unprecidented moddability of the BattleTech AI.

## Current Features

Functionality is provided via `AIOverride`s that are applied to matching to either teams or units. `AIOverride`s are loaded from JSON def files though ModTek's CustomResourceType feature. Each `AIOverride` will be matched one-to-one to a corresponding unit or team. This matching is performed each activation that the AI takes. If multiple overrides match, then the one with the highest priority is chosen, if multiple have the same priority, then the first loaded wins.

Matching operations are performed based on provided `Selectors` in the `AIOverride`. Each `Selector` has a `TypeName`, from a list of provided `Selector` types and a `SelectString` that is provided to the type as a parameter. For example, an `AIOverride` with a single selector with `TypeName`: "TeamName" and `SelectString` "Player 2" will match to a unit that is on the team "Player 2" (if it is selecting a unit) or the team itself (if it is selecting a team).

A special selector type is provided named `Custom` that will allow for other mods to provide additional selector functionality. An example might be a mod that provides panic states, where a unit could be selected because the unit is panicking and then it's AI behavior could be completely changed (perhaps it runs away always). Another example would be a mod that provides additional roles for units with custom behavior based on that.

### UnitAIOverrideDef

This type of `AIOverride` can affect behavior trees, behavior variables, and movement influence factors. 

#### Behavior Tree

The behavior tree controls all actions that the unit AI takes. The behavior tree of individual units can be replaced by providing a `NewBehaviorTreeRoot` in the `UnitAIOverrideDef` that contains a JSON representation of the tree. An example tree is below. A way to dump existing trees to JSON is provided as well (described later).

```json
{
  "Name": "example_root",
  "TypeName": "SelectorNode",
  "Children": [
    {
      "Name": "if_shutdown__restart",
      "TypeName": "SequenceNode",
      "Children": [
        {
          "Name": "isShutdown0000",
          "TypeName": "IsShutDownNode"
        },
        {
          "Name": "mechStartUp0000",
          "TypeName": "MechStartUpNode"
        }
      ]
    },
    {
      "Name": "if_prone__stand_up",
      "TypeName": "SequenceNode",
      "Children": [
        {
          "Name": "movementAvailable0000",
          "TypeName": "IsMovementAvailableForUnitNode"
        },
        {
          "Name": "isProne0000",
          "TypeName": "IsProneNode"
        },
        {
          "Name": "stand0000",
          "TypeName": "StandNode"
        }
      ]
    },
  ]
}
```

#### Influence Factors

Movement influence factors are calculated by the behavior tree node `SortMoveCandidatesByInfMapNode` and then an order to move to that node is emitted by the node `MoveTowardsHighestPriorityMoveCandidateNode`.

Influence Factors come in three varieties:

* `InfluenceMapPositionFactor`
  * basic evaluation given a hex, facing, and move type
* `InfluenceMapAllyFactor`
  * evaluates for x closest allies
    * x = behavior variable `Int_AllyInfluenceCount` value
* `InfluenceMapEnemyFactor`
  * evaluates for x closest enemies
    * x = behavior variable `Int_HostileInfluenceCount` value

Influence factors are calculated for each generated move (which are hex/move type/facing, other nodes in behavior tree generate these moves) and then the calculated value is then normalized from 0 (the lowest evaluated) to 1 (the highest evaluated). Then, each factor's value is multipled by its weight, either a normal weight or a sprint weight, and these weights make up many of the behavior variables. For example, the positional factor `PreferLocationsThatGrantGuardPositionFactor` uses the weights `Float_PreferLocationsThatGrantGuardFactorWeight` and `Float_SprintPreferLocationsThatGrantGuardFactorWeight`.

A `UnitAIOverrideDef` can provide additional factors to evaluate in `AddInfluenceFactors` and remove existing vanilla factors `RemoveInfluenceFactors`. Factors are added/removed based on their C# type name.

Since behavor variable's generally use the `BehaviorVariableName` that cannot be modified at runtime, modded factors are given the weight of 1.0 or can provide their own weights in `BehaviorVariableOverrides` with the (typeName + "Weight") or (typeName + "SprintWeight").

#### Behavior Variables

Behavior variables act as parameters to both the behavior tree and assorted other AI code. For example, the `Float_FenceRadius` controls how big the unit could get from the mean position of the lance for the influence factor `PreferInsideFenceNegativeLogicPositionalFactor`. `Bool_AllowAttack` is used as parameter to the `IsBVTrueNode` in many places in the tree to control if the unit can attack.

You can provide overrides to the affected unit's behavior variables in two ways:

* Provide the path to a folder that contains `BehaviorVariableScope` files (such as `global.json`) in `BehaviorScopesDirectory`
* Provide simple overrides in `BehaviorVariableOverrides`

An example `BehaviorVariableOverrides` that contains both vanilla variables and variables for a modded influence factor:

```json
"BehaviorVariableOverrides": {
    "Bool_AllowAttack": {
        "type": "Bool",
        "boolVal": true
    },
    "Float_FenceRadius": {
        "type": "Float",
        "floatVal": 150
    },

    "PreferHigherEvasionPositionFactorWeight": {
        "type": "Float",
        "floatVal": 1.0
    },
    "PreferHigherEvasionPositionFactorSprintWeight": {
        "type": "Float",
        "floatVal": 0.0
    }
}
```

#### Current Avalable Selectors

* TeamName *("Player 2")*
* Tree *("CoreAITree")*
* Role *("Brawler")*
* Custom

### TeamAIOverrideDef

#### Unit Selection / Turn Order

When `TurnOrderFactorWeights` is provided, the AI unit selection process is replaced by a similar system to influence factors (normalized from 0 (lowest value) to 1 (highest value)) and then multiplied by the weight. Each factor is then summed to a total value and the unit with the highest total goes first.

##### Currently Provided TurnOrderFactors

* DistanceAlongPatrolRoute
* DistanceToClosestEnemy
* DistanceToClosestEnemyDesc
* DistanceToClosestVulnerableEnemy
* IsUnstable
* IsVulnerable

#### Current Available Selectors

* TeamName *("Player 2")*
* Custom

### Aditional Functionality

The mod also provides the following functionality:

* Behavior Tree Dump
  * Vanilla Behavior Trees can be dumped as JSON or a text-based format
    * Text based format does not contain all parameters based to BehaviorNodes
* AI Hot Reload
  * Behavior Variables load from their location on disk (in versionManifest/modtek)
    * Currently cannot be done during AI pause/too close to an AI `think()` call
  * UnitAIOverrideDefs are reloaded and all referenced files are reloaded
  * All overrideDef functionality is re-applied
* AI Pause (Setting in mod.json)
  * AI will show moves before taking them
  * Left CTRL + Right Arrow executes current action, advances AI
  * If an influence map is used, a visualization of that map is shown
    * This visualization is a drastic simplification (facing/movement types)
  * If a behaviorTree returns an `OrderInfo`, the debug trace string is shown

## Limitations of Current Implementation

* UnitAIOverrideDefs do not allow for tier'd retrivial of behaviorVariables
* Reflection-based BehaviorNode construction searches fields for matching types
  * Thus, BehaviorNode's can only have single of each type and have to have a matching field of that type
* Reflection-based influence factor construction only supports parameter-less constructors
* HotReload only reloads default behavior variable scopes if AI is not paused

## Planned Features

* Additional `SelectorTypes` based on feedback
* Additional `WeightedFactor`/`BehaviorNode` based on feedback to provide base mod functionality
* Base UnitAIOverrideDefs to merge onto for at least CoreBT_AI tree?
* Modding reservation rules

## Potential Features

* Adding a "blackboard" for each unit
* Wild idea: support rewinding AI turns without reloading
* Wild idea: multiple trees/variables on a unit and visualize results of all
