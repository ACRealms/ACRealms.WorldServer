DELETE FROM `weenie` WHERE `class_Id` = 4200008;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200008, '4200008', 38, '2019-08-11 06:52:23') /* Gem */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200008,   1,       2048) /* ItemType - Gem */
     , (4200008,   3,         82) /* PaletteTemplate - PinkPurple */
     , (4200008,   5,         10) /* EncumbranceVal */
     , (4200008,   8,         10) /* Mass */
     , (4200008,  11,         25) /* MaxStackSize */
     , (4200008,  12,          1) /* StackSize */
     , (4200008,  13,         10) /* StackUnitEncumbrance */
     , (4200008,  14,         10) /* StackUnitMass */
     , (4200008,  15,       1) /* StackUnitValue */
     , (4200008,  16,          8) /* ItemUseable - Contained */
     , (4200008,  18,          1) /* UiEffects - Magical */
     , (4200008,  19,       1) /* Value */
     , (4200008,  53,        101) /* PlacementPosition - Resting */
     , (4200008,  93,       3092) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity, LightingOn */
     , (4200008,  94,         16) /* TargetType - Creature */
     , (4200008, 106,        210) /* ItemSpellcraft */
     , (4200008, 107,         50) /* ItemCurMana */
     , (4200008, 108,         50) /* ItemMaxMana */
     , (4200008, 109,          0) /* ItemDifficulty */
     , (4200008, 110,          0) /* ItemAllegianceRankLimit */
     , (4200008, 150,        103) /* HookPlacement - Hook */
     , (4200008, 151,          2) /* HookType - Wall */
     , (4200008, 280,       1000) /* SharedCooldown */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200008,  11, True ) /* IgnoreCollisions */
     , (4200008,  13, True ) /* Ethereal */
     , (4200008,  14, True ) /* GravityStatus */
     , (4200008,  15, True ) /* LightsStatus */
     , (4200008,  19, True ) /* Attackable */
     , (4200008,  23, True ) /* DestroyOnSell */
     , (4200008,  42002, True);

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200008, 167,      15) /* CooldownDuration */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200008,   1, 'Deep Mukkir Nest') /* Name */
     , (4200008,  15, 'This portal summoning gem works best if used outside in a relatively flat area.') /* ShortDesc */
     , (4200008,  16, 'This portal summoning gem works best if used outside in a relatively flat area.') /* LongDesc */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200008,   1,   33556769) /* Setup */
     , (4200008,   3,  536870932) /* SoundTable */
     , (4200008,   6,   67111919) /* PaletteBase */
     , (4200008,   7,  268435723) /* ClothingBase */
     , (4200008,   8,  100674869) /* Icon */
     , (4200008,  22,  872415275) /* PhysicsEffectTable */
     , (4200008,  28,        157) /* Spell - Summon Primary Portal I */
     , (4200008,  31,    4200007) /* LinkedPortalOne - Deep Mukkir Nest */
     , (4200008,  36,  234881046) /* MutateFilter */;
