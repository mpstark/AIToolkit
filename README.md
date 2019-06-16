# AIToolkit

Unprecidented moddability of the BattleTech AI.

## Current Features

* Override nearly all facets of unit AI ([UnitAIOverrideDef](https://github.com/Mpstark/AIToolkit/wiki/Resource:-UnitAIOverrideDef))
  * [Behavior Trees](https://github.com/Mpstark/AIToolkit/wiki/Unit:-Behavior-Trees): The "brain" of the unit AI, determines what the unit does during its turn
  * [Behavior Variables](https://github.com/Mpstark/AIToolkit/wiki/Unit:-Behavior-Variables): Variables/parameters for the behavior tree or weights for the influence map
  * [Influence Map/Factors](https://github.com/Mpstark/AIToolkit/wiki/Unit:-Influence-Map-and-Factors): Where units want to move (if using influence map in tree)
* Override some of team/lance AI ([TeamAIOverrideDef](https://github.com/Mpstark/AIToolkit/wiki/Resource:-TeamAIOverrideDef))
  * [Target designation](https://github.com/Mpstark/AIToolkit/wiki/Team:-Designating-Targets)
  * [Turn order](https://github.com/Mpstark/AIToolkit/wiki/Team:-Turn-Order)

Functionality is provided via `AIOverride`s that are applied to matching to either teams or units. `AIOverride`s are loaded from JSON def files though ModTek's CustomResourceType feature. Each `AIOverride` will be matched one-to-one to a corresponding unit or team. This matching is performed each activation that the AI takes. If multiple overrides match, then the one with the highest priority is chosen, if multiple have the same priority, then the first loaded wins.

Matching operations are performed based on provided `Selectors` in the `AIOverride`. Each `Selector` has a `TypeName`, from a list of provided `Selector` types and a `SelectString` that is provided to the type as a parameter. For example, an `AIOverride` with a single selector with `TypeName`: "TeamName" and `SelectString` "Player 2" will match to a unit that is on the team "Player 2" (if it is selecting a unit) or the team itself (if it is selecting a team). A [more detailed-primer on selectors](https://github.com/Mpstark/AIToolkit/wiki/Selectors) is on the available on the wiki.

## Support Functionality

The mod also provides the following functionality to aid in AI mod development.

* Behavior Tree Dump: vanilla behavior trees can be dumped as JSON or a text-based format
  * Text based format does not contain all parameters based to `BehaviorNode`s
* HotReload: reload all overrides defs (along with vanilla AI files) and reapply at runtime
  * Key-combo: CTRL-SHIFT-A
* AI Pause: AI will show moves before taking them, along with additional information
  * Left CTRL + Right Arrow advances and executes current action
  * If an influence map is used, a visualization of that map is shown
    * This visualization is a simplification (without facing/movement types)
  * If a leaf behavior node returns an `OrderInfo`, that leaf will be shown
  * If turn order is overriden, turn order results are shown

## Limitations of Current Implementation

* Reflection-based `BehaviorNode` construction searches fields for matching types
  * Thus, `BehaviorNode`'s can only have single of each type and have to have a matching field of that type
* Reflection-based influence factor construction only supports parameter-less constructors
* HotReload only reloads default behavior variable scopes if AI is not paused

## Planned Features

* Many, many more `SelectorType`/`WeightedFactor`/`BehaviorNode` based on feedback to provide base functionality
* Base `UnitAIOverrideDef`/`BehaviorNodeDef` for CoreAI_BT units
* Base `TeamAIOverrideDef` for non-story missions
* Modding reservation rules

## Potential Features

* Adding a "blackboard" for each unit
* Wild idea: multiple trees/variables on a unit and visualize results of all
