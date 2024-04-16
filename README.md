# ACRealms V2

This is a fork of https://github.com/ACEmulator/ACE/. I'm considering recreating the repository as a fork. For now, if you want to see a full list of code changes (diff), you can git clone the project, and add the upstream ACE project as a second git remote, and use your preferred git client to do a diff between ACE tag `v1.58.4480` and `master`.

Focus areas:
 - Instanced Landblocks (multiple logical dungeons in same 'physical' landblock space)
 - Ruleset Composition (Realms are composed of ruleset definitions chained together recursively)
 - Ephemeral Realms (Temporary landblocks that may be assigned additional rulesets, including those defined by the player through crafting, inspired by the map device in Path of Exile)

 This is a work in progress, expect bugs and rough edges (but the project is rapidly improving as of v2). Contributions and feedback greatly appreciated.


## Servers using AC Realms

This project has been used in three servers that I am aware of.
1. ACRealms Alpha (late 2020)
2. Escape From Dereth (mid 2023)
3. Pourtide 3 (April 2024)

## Content Structure (differences from ACE)

#### Realms.jsonc
realms.jsonc contains a list of realm and ruleset names mapped to realm ids. These ids can be changed to anything between 1 and 0x7FFE (32766), and may not be changed to new values after the first run of the server.  
New realms can still be added to the list as long as they are not changed after the next time the server is started.
The realm name must match the name specified in the realm file (not the filename).
If a realm file exists, it must have a corresponding entry in this file, and vice versa. It's not the most user-friendly process but there is room for improvement.

#### Realm and Ruleset JSON

A realm file exists under `Content/json/realms/realm/xxx.jsonc`, and a ruleset file exists under `Content/json/realms/ruleset/xxx.jsonc`. They have the same basic structure.
The key difference between a realm and a ruleset is that a realm may exist as a permanent home location for a player. A ruleset does not. Rulesets are intended to be composed on top of realms, to produce "ephemeral realms" (temporary rulesets).
Realm definitions may also be composed in a similar manner, but the result is a permanent world

Example:
```json
{
  "name": "Modern Realm",
  "type": "Realm",
  "properties": {
    "Description": "The Customized Realm with the latest and greatest features.",
    "CanBeHomeworld": true,
    "CanInteractWithNeutralZone": true,
    "HideoutEnabled": true
  }
}
```

Valid keys:
- `name` (required): The name of the realm. Must be unique and must match an entry in `realms.jsonc`
- `type` (required): "Realm" for realm, "Ruleset" for a ruleset
- `parent` (optional): The `name` of the realm or ruleset from which to inherit properties from. A realm may only inherit from an entry with `type` = Realm. 
- `apply_rulesets` (optional): An array of ruleset names to compose on top of this one (the properties in the current ruleset file are first applied from the result of the parent ruleset, and then the rulesets defined in `apply_rulesets` are applied afterward)
- `apply_rulesets_random` (optional): An array where the elements may be either a hash or an array.
  If a hash: The key is the ruleset name, and the value is either a floating point number between 0 and 1, for the probability, or "auto" to automatically assign probabilities equally.
  If an array: The element of the array contains one or more hashes described like above, where one of the elements is chosen at random.
  See [random-test.jsonc](https://github.com/ACRealms/ACRealms.WorldServer/blob/master/Content/json/realms/ruleset/random-test.jsonc) for an example
- `properties` (required): A hash of properties, where the key is equal to an enum entry defined in ACE.Entity.Enum.Properties.RealmPropertyXXX.cs, and the value is either a Hash of type 'property entry' described below, or a constant scalar value deserializable to the corresponding type.
  The realm properties in the Enum definition itself (.cs file) will also have absolute minimums and maximums. If the value is outside this range (after composition and randomization), it will be adjusted to fit the range. For example, if the range is from 0 to 50, and the composed value is 60, the effective value will be 50. No errors or warnings will be emitted, by design.
- `properties_random_count`: An integer to specify that instead of all properties being applied, this many properties from the ruleset will be selected at random (excluding the description). This occurs during landblock load as part of the ruleset activation pipeline.

Property entry hash valid keys:
- `value`: A default value, matching the type of the corresponding property. May not be present if `low` or `high` is present. This takes precedence over the default value defined in the enum type.
- `low`: The minimum value when rerolled. If present, `high` must also be defined. The absolute minimum defined in the enum type will take precedence if there is a conflict. 
- `high`: The maximum value when rerolled. If present, `low` must also be defined. The absolute maximum defined in the enum type will take precedence if there is a conflict.
- `reroll`: One of:
  - `landblock` (default): Reroll once during landblock load
  - `always`: Reroll each time the property is accessed by game logic
  - `never`: Use the default value
- `compose`: One of:
  - `add`: Add the result of the two rulesets together
  - `multiply`: Multiply the result of the two rulesets together
  - `replace`: Discard the previous value and replace it with the result from this ruleset.
- `locked`: Rulesets that inherit from this ruleset will ignore any properties that are locked here instead of composing them. If applying a ruleset with `apply_rulesets` or `apply_rulesets_random`, that value will become locked if specified.
- `probability`: (Not implemented yet): A floating-point number between 0 and 1 representing the probability of this property taking effect.


## Known Issues

- Housing is working, but not tested in full yet. Purchasing houses, abandoning houses, villa portals, villa storage have been tested. I haven't tried mansions, apartments, allegiance housing, booting, or permissions yet. I'm sure not everything works yet but it is just a matter of fixing minor things. The hard technical problems related to housing have already been solved, however.
- Ruleset and realm files were originally intended to be updatable without a restart of the server, and a very early version of this project allowed it, but there were issues with caching. I still want to fix that because restarting the server is not convenient when experimenting with ideas for new rulesets.
- The ruleset specification is complex and not covered by any unit tests. If you notice any unexpected behavior with rulesets, please report it!
- landblock content files are global and cannot be defined on a realm by realm basis yet. This is something I've wanted to address for a very long time now but it hasn't been done yet because of priorities.

## Developer notes

Property IDs (ACE.Entity.Enum.Properties.PropertyXXX) from 42000-42999 are reserved by AC Realms Core. 

Realm Property IDs (ACE.Entity.Enum.Properties.RealmPropertyXXX) from 0-9999 are reserved by AC Realms Core in a similar manner.

If using this project in your own server, please do not add new properties with IDs in this range.

## Contact

`russellfannin0@gmail.com`


## License

AGPL v3

## Server Operator Guidelines

We have the same guidelines as https://github.com/ACEmulator/ACE/, which ACRealms is forked from.

> We have a NO financial solicitation and donation policy. If you solicit or accept donations or gifts in direct relation to ACRealms, your perks will be revoked and discord tags removed. If you come back into compliance with this policy, your perks and access will be restored.
> If you are in violation of our ACRealms AGPL-3.0 license, you will be removed from our discord server. We may also petition to have your servers removed from public servers lists

## Credits

- AC Emulator Team
- gmriggs - Wrote the first proof of concept for instanced landblocks, which was the original inspiration for the project
- Vodka (https://github.com/darktidelegend) - Early adopter, brought this project back from the dead in 2023
- OptimShi - Brought attention to the static GUID limitations that ended up helping fix some obscure but critical bugs
