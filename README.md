# EnhancedAI

## Current Features

Mods can create AIOverrideDef JSONs (a ModTek CustomResourceType) that overrides behavior trees/variables. As a custom resource, ModTek supports both regular json merges and advanced merges if mods want to mod each other. These are declared in mods manifest as if they were regular types supported by the game.

`AIOverrideDefs`:
* only a single def is applied to each AI controlled unit
* matches based on a `Selector` and `SelectorType`
  * Current selector types are `TreeSelector`, `TeamNameSelector`, `RoleSelector`, and `CustomSelector`
  * e.g. `SelectorType`: "TeamName" `Selector`: "Player 2" will override all AI units controlled by the team named "Player 2"
  * `Custom` selector allows to call custom function with signature `public static bool MySelectorName(AbstractActor unit)`
    * e.g. `Selector`: "MyNamespace.MySelectorName"
  * if multiple overrideDefs match a single unit, the one that has the highest `Priority` wins
  * if multiple have same priority, first to be declared (in ModTek load order) wins
* Support behavior tree replacement from a JSON representation of BehaviorTrees
* Can override BehaviorVariableScopes by providing a whole directory of BehaviorVariableScopes
* Allow for simple override of particular BehaviorVariables (which then are the highest priority for behaviorVariableValues)

The mod also provides the following functionality:

* Behavior Tree Dump
  * Vanilla Behavior Trees can be dumped as JSON or a text-based format
    * Text based format does not contain all parameters based to BehaviorNodes
* AI Hot Reload
  * Behavior Variables load from their location on disk (in versionManifest/modtek)
  * AIOverrideDefs are reloaded and all referenced files are reloaded
  * All overrideDef functionality is re-applied

## Limitations of Current Implementation

* AIOverrideDefs are only applied on HotReload and BehaviorTree initialization
* Reflection-based BehaviorNode construction searches fields for matching types
  * Thus, BehaviorNode's can only have single of each type and have to have a matching field of that type

## Planned Features

* Additional `SelectorTypes` based on feedback
* Additional BehaviorNode types to provide some base mod functionality
* Base AIOverrideDefs to merge onto for at least CoreBT_AI tree

## Potential Features

* AI pause after action until keypress
* Influence map visualization on terrian after AI generates it
* Some sort of in-game visualization of what node was chosen
* Wild idea: support rewinding AI turns without reloading
* Wild idea: multiple trees/variables on a unit and visualize results
