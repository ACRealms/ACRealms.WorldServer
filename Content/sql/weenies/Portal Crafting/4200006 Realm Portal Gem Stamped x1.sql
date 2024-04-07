DELETE FROM `weenie` WHERE `class_Id` = 4200006;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200006, 'realm-portal-gem-stamped', 38, '2019-08-11 06:52:23') /* Gem */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200006,   1,       2048) /* ItemType - Gem */
     , (4200006,   3,         82) /* PaletteTemplate - PinkPurple */
     , (4200006,   5,         10) /* EncumbranceVal */
     , (4200006,   8,         10) /* Mass */
     , (4200006,  11,         1) /* MaxStackSize */
     , (4200006,  12,          1) /* StackSize */
     , (4200006,  13,         10) /* StackUnitEncumbrance */
     , (4200006,  14,         10) /* StackUnitMass */
     , (4200006,  15,       1) /* StackUnitValue */
     , (4200006,  16,          8) /* ItemUseable - Contained */
     , (4200006,  18,          1) /* UiEffects - Magical */
     , (4200006,  19,       1) /* Value */
     , (4200006,  53,        101) /* PlacementPosition - Resting */
     , (4200006,  93,       3092) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity, LightingOn */
     , (4200006,  94,         16) /* TargetType - Creature */
     , (4200006, 106,        210) /* ItemSpellcraft */
     , (4200006, 107,         50) /* ItemCurMana */
     , (4200006, 108,         50) /* ItemMaxMana */
     , (4200006, 109,          0) /* ItemDifficulty */
     , (4200006, 110,          0) /* ItemAllegianceRankLimit */
     , (4200006, 150,        103) /* HookPlacement - Hook */
     , (4200006, 151,          2) /* HookType - Wall */
     , (4200006, 280,       1000) /* SharedCooldown */
     , (4200006, 42001,          3) /* SummonTargetRealm */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200006,  11, True ) /* IgnoreCollisions */
     , (4200006,  13, True ) /* Ethereal */
     , (4200006,  14, True ) /* GravityStatus */
     , (4200006,  15, True ) /* LightsStatus */
     , (4200006,  19, True ) /* Attackable */
     , (4200006,  23, True ) /* DestroyOnSell */
     , (4200006,  42002, True);

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200006, 167,      15) /* CooldownDuration */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200006,   1, 'Realm Portal Gem - Stamped') /* Name */
     , (4200006,  15, 'This portal summoning gem works best if used outside in a relatively flat area.') /* ShortDesc */
     , (4200006,  16, 'This portal summoning gem works best if used outside in a relatively flat area.') /* LongDesc */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200006,   1,   33556769) /* Setup */
     , (4200006,   3,  536870932) /* SoundTable */
     , (4200006,   6,   67111919) /* PaletteBase */
     , (4200006,   7,  268435723) /* ClothingBase */
     , (4200006,   8,  100674869) /* Icon */
     , (4200006,  22,  872415275) /* PhysicsEffectTable */
     , (4200006,  28,        157) /* Spell - Summon Primary Portal I */
     , (4200006,  31,       7291) /* LinkedPortalOne - Halls of Metos */
     , (4200006,  36,  234881046) /* MutateFilter */
     , (4200006,  50,  0x06006C20) /* IconOverlay */;
