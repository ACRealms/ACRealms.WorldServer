DELETE FROM `weenie` WHERE `class_Id` = 4200018;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200018, '4200018', 38, '2019-08-11 06:52:23') /* Gem */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200018,   1,       2048) /* ItemType - Gem */
     , (4200018,   3,         82) /* PaletteTemplate - PinkPurple */
     , (4200018,   5,         10) /* EncumbranceVal */
     , (4200018,   8,         10) /* Mass */
     , (4200018,  11,         25) /* MaxStackSize */
     , (4200018,  12,          1) /* StackSize */
     , (4200018,  13,         10) /* StackUnitEncumbrance */
     , (4200018,  14,         10) /* StackUnitMass */
     , (4200018,  15,       1) /* StackUnitValue */
     , (4200018,  16,          8) /* ItemUseable - Contained */
     , (4200018,  18,          1) /* UiEffects - Magical */
     , (4200018,  19,       1) /* Value */
     , (4200018,  53,        101) /* PlacementPosition - Resting */
     , (4200018,  93,       3092) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity, LightingOn */
     , (4200018,  94,         16) /* TargetType - Creature */
     , (4200018, 106,        210) /* ItemSpellcraft */
     , (4200018, 107,         50) /* ItemCurMana */
     , (4200018, 108,         50) /* ItemMaxMana */
     , (4200018, 109,          0) /* ItemDifficulty */
     , (4200018, 110,          0) /* ItemAllegianceRankLimit */
     , (4200018, 150,        103) /* HookPlacement - Hook */
     , (4200018, 151,          2) /* HookType - Wall */
     , (4200018, 280,       1000) /* SharedCooldown */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200018,  11, True ) /* IgnoreCollisions */
     , (4200018,  13, True ) /* Ethereal */
     , (4200018,  14, True ) /* GravityStatus */
     , (4200018,  15, True ) /* LightsStatus */
     , (4200018,  19, True ) /* Attackable */
     , (4200018,  23, True ) /* DestroyOnSell */
     , (4200018,  42002, True);

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200018, 167,      15) /* CooldownDuration */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200018,   1, 'Abayar''s Laboratory') /* Name */
     , (4200018,  15, 'This portal summoning gem works best if used outside in a relatively flat area.') /* ShortDesc */
     , (4200018,  16, 'This portal summoning gem works best if used outside in a relatively flat area.') /* LongDesc */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200018,   1,   33556769) /* Setup */
     , (4200018,   3,  536870932) /* SoundTable */
     , (4200018,   6,   67111919) /* PaletteBase */
     , (4200018,   7,  268435723) /* ClothingBase */
     , (4200018,   8,  100674869) /* Icon */
     , (4200018,  22,  872415275) /* PhysicsEffectTable */
     , (4200018,  28,        157) /* Spell - Summon Primary Portal I */
     , (4200018,  31,    4200017) /* LinkedPortalOne - Deep Mukkir Nest */
     , (4200018,  36,  234881046) /* MutateFilter */;
