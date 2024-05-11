# ACRealms V2

This is a fork of https://github.com/ACEmulator/ACE/. The repo was not originally set up a fork and therefore could not be turned into one, but there is a mirror repo here for visibility https://github.com/ACRealms/ACRealmsForkMirror.

It is recommended to fork from the mirror repo instead of this one. The mirror repo may eventually become the main one. 

# Mission Statement

**The mission of ACRealms is to be the recommended Asheron's Call server emulator for servers with heavy customization needs.**

Focus areas:
 - Instanced Landblocks (multiple logical dungeons in same 'physical' landblock space)
 - Ruleset Composition (Realms are composed of ruleset definitions chained together recursively)
 - Ephemeral Realms (Temporary landblocks that may be assigned additional rulesets, including those defined by the player through crafting, inspired by the map device in Path of Exile)
 - Automated testing - ACE traditionally lacked this capability. Due to the sheer amount and complexity of changes, and customizability of rulesets, automated tests are more important here.
 

## Contributing

Contributions (time and development, **not money**) and feedback greatly appreciated. Contributors and server operators will have more of a say in development direction and priorities.

The best way to start contributing is to join the discord (https://discord.gg/pN65pYqZeS) and introduce yourself. Or alternatively, clone the repo and review the implementation and start experimenting!
You'll likely find something that can be improved. There are still many features not implemented as a realm property. 

For complex changes, it is recommended to get in touch via Discord before attempting them.

If you don't use Discord and want to contribute, email me and let me know what mode of communication works best for you.


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

It is recommended to use visual studio code to edit these files, using the "Content" folder as the root. A json-schema folder exists and realm properties will be populated in this schema after successful build of the ACE.Server project.  
This allows for autocomplete and tooltip functionality to be integrated with the editor. 

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
- `probability`: A floating-point number between 0 and 1 representing the probability of this property taking effect.

#### RealmProperty Enums

These are shared by all realms, application-wide, and list the possible realm properties that can be used. If the property is in this list, it can be used in a realm definition.
```C#
    public enum RealmPropertyFloat : ushort
    {
        [RealmPropertyFloat(defaultValue: 0f, minValue: 0f, maxValue: 0f)]
        Undef                          = 0,

        // First param is the default, second is the min, third is the max.
        [RealmPropertyFloat(1f, 0.1f, 5f)]
        SpellCasting_MoveToState_UpdatePosition_Threshold = 1,

        // If defaultFromServerProperty is defined, the corresponding property from PropertyManager (/fetchdouble in this case) will used in place of the default.
        // If the server property with the given name is missing from the database, the defaultValue parameter will be used as a fallback. 
        [RealmPropertyFloat(defaultFromServerProperty: "spellcast_max_angle", 20f, 0f, 360f)]
        Spellcasting_Max_Angle = 2,
        ...
    }
```

## Known Issues

- Ruleset and realm files were originally intended to be updatable without a restart of the server, and a very early version of this project allowed it, but there were issues with caching. I still want to fix that because restarting the server is not convenient when experimenting with ideas for new rulesets.
- The ruleset specification is complex and not yet fully covered by unit tests, but progress is being made here. If you notice any unexpected behavior with rulesets, please report it!
- landblock content files are global and cannot be defined on a realm by realm basis yet. This is something I've wanted to address for a very long time now but it hasn't been done yet because of priorities. (UPDATE: This is now scheduled for the v2.3 milestone)

## Developer notes

Property IDs (ACE.Entity.Enum.Properties.PropertyXXX) and PositionTypes from 42000-42999 are reserved by AC Realms Core. 

Realm Property IDs (ACE.Entity.Enum.Properties.RealmPropertyXXX) from 0-9999 are reserved by AC Realms Core in a similar manner.

If using this project in your own server, please do not add new properties with IDs in this range.

## Contact

`russellfannin0@gmail.com`

Discord Link: https://discord.gg/pN65pYqZeS

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
