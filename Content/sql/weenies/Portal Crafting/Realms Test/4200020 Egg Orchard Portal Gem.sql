DELETE FROM `weenie` WHERE `class_Id` = 4200020;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200020, '4200020', 38, '2019-08-11 06:52:23') /* Gem */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200020,   1,       2048) /* ItemType - Gem */
     , (4200020,   3,         82) /* PaletteTemplate - PinkPurple */
     , (4200020,   5,         10) /* EncumbranceVal */
     , (4200020,   8,         10) /* Mass */
     , (4200020,  11,         25) /* MaxStackSize */
     , (4200020,  12,          1) /* StackSize */
     , (4200020,  13,         10) /* StackUnitEncumbrance */
     , (4200020,  14,         10) /* StackUnitMass */
     , (4200020,  15,       1) /* StackUnitValue */
     , (4200020,  16,          8) /* ItemUseable - Contained */
     , (4200020,  18,          1) /* UiEffects - Magical */
     , (4200020,  19,       1) /* Value */
     , (4200020,  53,        101) /* PlacementPosition - Resting */
     , (4200020,  93,       3092) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity, LightingOn */
     , (4200020,  94,         16) /* TargetType - Creature */
     , (4200020, 106,        210) /* ItemSpellcraft */
     , (4200020, 107,         50) /* ItemCurMana */
     , (4200020, 108,         50) /* ItemMaxMana */
     , (4200020, 109,          0) /* ItemDifficulty */
     , (4200020, 110,          0) /* ItemAllegianceRankLimit */
     , (4200020, 150,        103) /* HookPlacement - Hook */
     , (4200020, 151,          2) /* HookType - Wall */
     , (4200020, 280,       1000) /* SharedCooldown */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200020,  11, True ) /* IgnoreCollisions */
     , (4200020,  13, True ) /* Ethereal */
     , (4200020,  14, True ) /* GravityStatus */
     , (4200020,  15, True ) /* LightsStatus */
     , (4200020,  19, True ) /* Attackable */
     , (4200020,  23, True ) /* DestroyOnSell */
     , (4200020,  42002, True);

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200020, 167,      15) /* CooldownDuration */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200020,   1, 'Egg Orchard') /* Name */
     , (4200020,  15, 'This portal summoning gem works best if used outside in a relatively flat area.') /* ShortDesc */
     , (4200020,  16, 'This portal summoning gem works best if used outside in a relatively flat area.') /* LongDesc */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200020,   1,   33556769) /* Setup */
     , (4200020,   3,  536870932) /* SoundTable */
     , (4200020,   6,   67111919) /* PaletteBase */
     , (4200020,   7,  268435723) /* ClothingBase */
     , (4200020,   8,  100674869) /* Icon */
     , (4200020,  22,  872415275) /* PhysicsEffectTable */
     , (4200020,  28,        157) /* Spell - Summon Primary Portal I */
     , (4200020,  31,    4200019) /* LinkedPortalOne - Deep Mukkir Nest */
     , (4200020,  36,  234881046) /* MutateFilter */;
