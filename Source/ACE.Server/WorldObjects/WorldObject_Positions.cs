using ACE.Entity.Enum.Properties;
using ACE.Server.Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Server.WorldObjects
{
    partial class WorldObject
    {
        // ========================================
        //= ======== Position Properties ==========
        // ========================================
        public InstancedPosition Location
        {
            get => (InstancedPosition)GetPosition(PositionType.Location);
            set => SetPosition(PositionType.Location, value);
        }

        public LocalPosition Destination
        {
            get { return (LocalPosition)GetPosition(PositionType.Destination); }
            set { SetPosition(PositionType.Destination, value); }
        }

        public InstancedPosition Instantiation
        {
            get { return (InstancedPosition)GetPosition(PositionType.Instantiation); }
            set { SetPosition(PositionType.Instantiation, value); }
        }

        public LocalPosition Sanctuary
        {
            get { return (LocalPosition)GetPosition(PositionType.Sanctuary); }
            set { SetPosition(PositionType.Sanctuary, value); }
        }

        public InstancedPosition Home
        {
            get { return (InstancedPosition)GetPosition(PositionType.Home); }
            set { SetPosition(PositionType.Home, value); }
        }

        public LocalPosition LinkedPortalOne
        {
            get { return (LocalPosition)GetPosition(PositionType.LinkedPortalOne); }
            set { SetPosition(PositionType.LinkedPortalOne, value); }
        }

        public LocalPosition LastPortal
        {
            get { return (LocalPosition)GetPosition(PositionType.LastPortal); }
            set { SetPosition(PositionType.LastPortal, value); }
        }

        public LocalPosition PortalStorm
        {
            get { return (LocalPosition)GetPosition(PositionType.PortalStorm); }
            set { SetPosition(PositionType.PortalStorm, value); }
        }

        public LocalPosition PortalSummonLoc
        {
            get { return (LocalPosition)GetPosition(PositionType.PortalSummonLoc); }
            set { SetPosition(PositionType.PortalSummonLoc, value); }
        }

        public LocalPosition HouseBoot
        {
            get { return (LocalPosition)GetPosition(PositionType.HouseBoot); }
            set { SetPosition(PositionType.HouseBoot, value); }
        }

        public LocalPosition LastOutsideDeath
        {
            get { return (LocalPosition)GetPosition(PositionType.LastOutsideDeath); }
            set { SetPosition(PositionType.LastOutsideDeath, value); }
        }

        public LocalPosition LinkedLifestone
        {
            get { return (LocalPosition)GetPosition(PositionType.LinkedLifestone); }
            set { SetPosition(PositionType.LinkedLifestone, value); }
        }

        public LocalPosition LinkedPortalTwo
        {
            get { return (LocalPosition)GetPosition(PositionType.LinkedPortalTwo); }
            set { SetPosition(PositionType.LinkedPortalTwo, value); }
        }

        public LocalPosition Save1
        {
            get { return (LocalPosition)GetPosition(PositionType.Save1); }
            set { SetPosition(PositionType.Save1, value); }
        }

        public LocalPosition Save2
        {
            get { return (LocalPosition)GetPosition(PositionType.Save2); }
            set { SetPosition(PositionType.Save2, value); }
        }

        public LocalPosition Save3
        {
            get { return (LocalPosition)GetPosition(PositionType.Save3); }
            set { SetPosition(PositionType.Save3, value); }
        }

        public LocalPosition Save4
        {
            get { return (LocalPosition)GetPosition(PositionType.Save4); }
            set { SetPosition(PositionType.Save4, value); }
        }

        public LocalPosition Save5
        {
            get { return (LocalPosition)GetPosition(PositionType.Save5); }
            set { SetPosition(PositionType.Save5, value); }
        }

        public LocalPosition Save6
        {
            get { return (LocalPosition)GetPosition(PositionType.Save6); }
            set { SetPosition(PositionType.Save6, value); }
        }

        public LocalPosition Save7
        {
            get { return (LocalPosition)GetPosition(PositionType.Save7); }
            set { SetPosition(PositionType.Save7, value); }
        }

        public LocalPosition Save8
        {
            get { return (LocalPosition)GetPosition(PositionType.Save8); }
            set { SetPosition(PositionType.Save8, value); }
        }

        public LocalPosition Save9
        {
            get { return (LocalPosition)GetPosition(PositionType.Save9); }
            set { SetPosition(PositionType.Save9, value); }
        }

        public InstancedPosition RelativeDestination
        {
            get { return (InstancedPosition)GetPosition(PositionType.RelativeDestination); }
            set { SetPosition(PositionType.RelativeDestination, value); }
        }

        public InstancedPosition TeleportedCharacter
        {
            get { return (InstancedPosition)GetPosition(PositionType.TeleportedCharacter); }
            set { SetPosition(PositionType.TeleportedCharacter, value); }
        }

        public InstancedPosition EphemeralRealmExitTo
        {
            get { return (InstancedPosition)GetPosition(PositionType.EphemeralRealmExitTo); }
            set { SetPosition(PositionType.EphemeralRealmExitTo, value); }
        }

        public InstancedPosition EphemeralRealmLastEnteredDrop
        {
            get { return (InstancedPosition)GetPosition(PositionType.EphemeralRealmLastEnteredDrop); }
            set { SetPosition(PositionType.EphemeralRealmLastEnteredDrop, value); }
        }
    }
}
