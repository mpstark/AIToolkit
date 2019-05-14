# EnhancedAI

## Current Features

Loads `AIOverrideDef` JSONs (a ModTek CustomResourceType) that override behavior trees/variables, these can be provided by dropping them into the `EnhancedAI/AIOverrideDefs/` folder or providing them in another mod (with EnhancedAI as a dependancy). As a custom resource type, mods can also merge onto already provided defs.

When `AIOverrideDef` are loaded, they are matched one-to-one to unit's AI by their `Selector` and `SelectorType` (multiple selectors will be added in the future). If multiple `AIOverrideDef`s match a single unit, the one with the highest priority is chosen, if there is a tie on priority, then the first loaded wins. 
* e.g. `SelectorType`: "TeamName" `Selector`: "Player 2" will override all AI units controlled by the team named "Player 2"
* Current selector types are `TreeSelector`, `TeamNameSelector`, `RoleSelector`, and `CustomSelector`
  * `Custom` selector allows to call custom function with signature `public static bool MySelectorName(AbstractActor unit)`
    * e.g. `Selector`: "MyNamespace.MySelectorName"

Each `AIOverrideDef` can change the AI by:
* Replacing the behavior tree itself
  * a JSON representation of the tree is provided in `NewBehaviorTreeRoot`
* Overriding BehaviorVariables for the unit
  * Simple overrides in `BehaviorVariableOverrides`
    * *CAUTION* it's a Dictionary<BehaviorVariableName, BehaviorVariableValue>
  * Path to a directory of `BehaviorVariableScope` JSON files in `BehaviorScopesDirectory`

The mod also provides the following functionality:

* Behavior Tree Dump
  * Vanilla Behavior Trees can be dumped as JSON or a text-based format
    * Text based format does not contain all parameters based to BehaviorNodes
* AI Hot Reload
  * Behavior Variables load from their location on disk (in versionManifest/modtek)
  * AIOverrideDefs are reloaded and all referenced files are reloaded
  * All overrideDef functionality is re-applied
* AI Pause (Setting in mod.json)
  * AI will show moves before taking them
  * Left CTRL + Right Arrow advances AI in steps
  * If an influence map is used, a visualization of that map is shown
    * This visualization is a drastic simplification (facing/movement types)

## Limitations of Current Implementation

* AIOverrideDefs are only applied on HotReload and BehaviorTree initialization
* AIOverrideDefs do not allow for tier'd retrivial behaviorVariables
* Reflection-based BehaviorNode construction searches fields for matching types
  * Thus, BehaviorNode's can only have single of each type and have to have a matching field of that type
* HotReload only reloads default behavior variable scopes if AI is not paused

## Planned Features

* Add/remove influence map calculations
* Multiple selectors on `AIOverrideDef`
* Additional `SelectorTypes` based on feedback
* Additional BehaviorNode types to provide some base mod functionality
* Base AIOverrideDefs to merge onto for at least CoreBT_AI tree

## Potential Features

* Wild idea: support rewinding AI turns without reloading
* Wild idea: multiple trees/variables on a unit and visualize results
