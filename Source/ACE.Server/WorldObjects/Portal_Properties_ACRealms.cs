using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;

namespace ACE.Server.WorldObjects
{
    // This should be for AC Realms properties from 42000-42999 only. Create/Use a Portal_Properties_Custom.cs for custom portal properties
    partial class Portal
    {
        public int? CrossRealmPortalRealmId
        {
            get => GetProperty(PropertyInt.CrossRealmPortalRealmID);
            set { if (value == null) RemoveProperty(PropertyInt.CrossRealmPortalRealmID); else SetProperty(PropertyInt.CrossRealmPortalRealmID, value.Value); }
        }

        public bool IsEphemeralRealmPortal
        {
            get => GetProperty(PropertyBool.IsEphemeralRealmPortal) ?? false;
            set { if (!value) RemoveProperty(PropertyBool.IsEphemeralRealmPortal); else SetProperty(PropertyBool.IsEphemeralRealmPortal, value); }
        }

        public uint? EphemeralRealmPortalInstanceID
        {
            get => (uint?)GetProperty(PropertyInstanceId.EphemeralRealmPortalInstanceID);
            set { if (value == null) RemoveProperty(PropertyInstanceId.EphemeralRealmPortalInstanceID); else SetProperty(PropertyInstanceId.EphemeralRealmPortalInstanceID, value.Value); }
        }
    }
}
