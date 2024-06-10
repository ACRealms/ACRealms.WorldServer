DELETE FROM `weenie` WHERE `class_Id` = 4200022;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200022, '4200022', 38, '2019-08-11 06:52:23') /* Gem */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200022,   1,       2048) /* ItemType - Gem */
     , (4200022,   3,         82) /* PaletteTemplate - PinkPurple */
     , (4200022,   5,         10) /* EncumbranceVal */
     , (4200022,   8,         10) /* Mass */
     , (4200022,  11,         25) /* MaxStackSize */
     , (4200022,  12,          1) /* StackSize */
     , (4200022,  13,         10) /* StackUnitEncumbrance */
     , (4200022,  14,         10) /* StackUnitMass */
     , (4200022,  15,       1) /* StackUnitValue */
     , (4200022,  16,          8) /* ItemUseable - Contained */
     , (4200022,  18,          1) /* UiEffects - Magical */
     , (4200022,  19,       1) /* Value */
     , (4200022,  53,        101) /* PlacementPosition - Resting */
     , (4200022,  93,       3092) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity, LightingOn */
     , (4200022,  94,         16) /* TargetType - Creature */
     , (4200022, 106,        210) /* ItemSpellcraft */
     , (4200022, 107,         50) /* ItemCurMana */
     , (4200022, 108,         50) /* ItemMaxMana */
     , (4200022, 109,          0) /* ItemDifficulty */
     , (4200022, 110,          0) /* ItemAllegianceRankLimit */
     , (4200022, 150,        103) /* HookPlacement - Hook */
     , (4200022, 151,          2) /* HookType - Wall */
     , (4200022, 280,       1000) /* SharedCooldown */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200022,  11, True ) /* IgnoreCollisions */
     , (4200022,  13, True ) /* Ethereal */
     , (4200022,  14, True ) /* GravityStatus */
     , (4200022,  15, True ) /* LightsStatus */
     , (4200022,  19, True ) /* Attackable */
     , (4200022,  23, True ) /* DestroyOnSell */
     , (4200022,  42002, True);

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200022, 167,      15) /* CooldownDuration */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200022,   1, 'Creepy Canyons') /* Name */
     , (4200022,  15, 'This portal summoning gem works best if used outside in a relatively flat area.') /* ShortDesc */
     , (4200022,  16, 'This portal summoning gem works best if used outside in a relatively flat area.') /* LongDesc */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200022,   1,   33556769) /* Setup */
     , (4200022,   3,  536870932) /* SoundTable */
     , (4200022,   6,   67111919) /* PaletteBase */
     , (4200022,   7,  268435723) /* ClothingBase */
     , (4200022,   8,  100674869) /* Icon */
     , (4200022,  22,  872415275) /* PhysicsEffectTable */
     , (4200022,  28,        157) /* Spell - Summon Primary Portal I */
     , (4200022,  31,    4200021) /* LinkedPortalOne - Deep Mukkir Nest */
     , (4200022,  36,  234881046) /* MutateFilter */;
