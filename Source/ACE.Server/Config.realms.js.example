/* Copy this file and rename it to Config.realms.js
   The defaults from this file will automatically be used instead when not located in acrealms.config
   More configuration options may be added in the future
   Config file provided with server version: 3.0.0-alpha0 */

{
  /* This is the default realm that new characters will be assigned to, if UseRealmSelector is false.
     By default, this is the realm named "default", which is system-defined and has realm ID 0x7FFE (32766)
     This value must be changed to a user-defined realm name unless "AllowUndefinedDefaultRealm" is true
     See the README.md file for instructions on how to configure a realm file

     Even if UseRealmSelector is enabled, a default realm is still necessary to cover any edge cases */
    "DefaultRealm": "default",

  // It is strongly recommended to leave this false, and to define a realm using a realm file, and change "DefaultRealm" to that realm
    "AllowUndefinedDefaultRealm": false,

    "CharacterCreationOptions": {
      /* If true, players will start in a room with an npc offering the choice of starting realm
         Realms with "CanBeHomeworld": true will be offered in this list
         This room will use a separate instance ID per player account.

         *important*: You must copy the contents of "Content/extensions/Realm Selector" to your root content folder for this to work */
        "UseRealmSelector": false
    },

  // Options for porting characters that existed before ACRealms v2.1, or existed in ACE (not ACRealms) servers before migrating to ACRealms
    "CharacterMigrationOptions": {
      /* If home realm is missing, characters will not be permitted to log in unless these options are set to valid values
         This action is not reversible for characters auto-assigned in this manner, which is why it is off by default

         For servers previously on ACE:
           AutoAssignHomeRealm: true, AutoAssignToRealm: "(name of realm you wish to assign existing characters to)"

         For servers previously on ACRealms versions 1.0 to 2.1:
           AutoAssignHomeRealm: true, AutoAssignToRealm: "RealmSelector" */
        "AutoAssignHomeRealm": false,

        "AutoAssignToRealm": "NULL"
    }
}
