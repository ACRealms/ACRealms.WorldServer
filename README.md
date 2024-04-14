# ACRealms V2

This is a fork of https://github.com/ACEmulator/ACE/. I'm considering recreating the repository as a fork.

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


## Known Issues

#### GUID Table

GUIDs in Asheron's Call are stored and used as 32-bit unsigned integers (normally displayed in hexidecimal).
These GUIDs are sometimes dynamically assigned, but sometimes statically assigned, with the landblock ID composing the first 16 bits, and only the remaining 16 bits used for a local component of this GUID.
Because ACRealms makes it possible to have multiple landblocks with the same 16-bit identifier, it is therefore possible to have multiple objects with the same statically assigned GUID.

There are only a few cases where this becomes a problem, such as collision detection and housing. We knew about the housing limitation from the start of the project (2020), but the collision detection issue was not discovered until very recently, April 2024.

The good news is that given these assumptions, the issue is fixable (it just hasn't been done yet):
1. GUID collisions will always occur between objects in different instances
2. A player can only be in one instance at a time
3. Objects with statically assigned GUIDs will never swap instances

ACE.Server.Physics.Managers.ServerObjectManager contains a global table with 32-bit GUID as a key. The idea is to identify statically assigned GUIDs, and translate them into a 64-bit version of the GUID, using the instance ID as the first 32 bits, and the original GUID as the last 32 bits.


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
