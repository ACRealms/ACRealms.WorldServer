namespace ACE.Common.ACRealms
{
    // See Config.realms.js.example for full descriptions
    public class ACRealmsMasterConfiguration
    {
        // IMPORTANT:
        // If true, the server is opting out of instancing or realms. This feature is experimental and not fully implemented yet.
        // If this is enabled, it may be difficult to disable until this is better supported, so I recommend leaving it 'false' for now
        public bool OptOutOfRealms { get; set; } = false; // Not really implemented, just a placeholder
        public string DefaultRealm { get; set; } = "default";

        public bool AllowUndefinedDefaultRealm { get; set; } = false;

        public CharacterCreationOptions CharacterCreationOptions { get; set; } = new CharacterCreationOptions();

        public CharacterMigrationOptions CharacterMigrationOptions { get; set; } = new CharacterMigrationOptions();
    }
}
