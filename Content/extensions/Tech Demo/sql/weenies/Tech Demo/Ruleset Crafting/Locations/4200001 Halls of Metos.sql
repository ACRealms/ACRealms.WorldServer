DELETE FROM `weenie` WHERE `class_Id` = 4200001;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200001, '4200001', 38, '2019-08-11 06:52:23') /* Gem */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200001,   1,       2048) /* ItemType - Gem */
     , (4200001,   3,         82) /* PaletteTemplate - PinkPurple */
     , (4200001,   5,         10) /* EncumbranceVal */
     , (4200001,   8,         10) /* Mass */
     , (4200001,  11,         25) /* MaxStackSize */
     , (4200001,  12,          1) /* StackSize */
     , (4200001,  13,         10) /* StackUnitEncumbrance */
     , (4200001,  14,         10) /* StackUnitMass */
     , (4200001,  15,       1) /* StackUnitValue */
     , (4200001,  16,          8) /* ItemUseable - Contained */
     , (4200001,  18,          1) /* UiEffects - Magical */
     , (4200001,  19,       1) /* Value */
     , (4200001,  53,        101) /* PlacementPosition - Resting */
     , (4200001,  93,       3092) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity, LightingOn */
     , (4200001,  94,         16) /* TargetType - Creature */
     , (4200001, 106,        210) /* ItemSpellcraft */
     , (4200001, 107,         50) /* ItemCurMana */
     , (4200001, 108,         50) /* ItemMaxMana */
     , (4200001, 109,          0) /* ItemDifficulty */
     , (4200001, 110,          0) /* ItemAllegianceRankLimit */
     , (4200001, 150,        103) /* HookPlacement - Hook */
     , (4200001, 151,          2) /* HookType - Wall */
     , (4200001, 280,       1000) /* SharedCooldown */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200001,  11, True ) /* IgnoreCollisions */
     , (4200001,  13, True ) /* Ethereal */
     , (4200001,  14, True ) /* GravityStatus */
     , (4200001,  15, True ) /* LightsStatus */
     , (4200001,  19, True ) /* Attackable */
     , (4200001,  23, True ) /* DestroyOnSell */
     , (4200001,  42002, True);

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200001, 167,      15) /* CooldownDuration */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200001,   1, 'Halls of Metos') /* Name */
     , (4200001,  15, 'This portal summoning gem works best if used outside in a relatively flat area.') /* ShortDesc */
     , (4200001,  16, 'This portal summoning gem works best if used outside in a relatively flat area.') /* LongDesc */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200001,   1,   33556769) /* Setup */
     , (4200001,   3,  536870932) /* SoundTable */
     , (4200001,   6,   67111919) /* PaletteBase */
     , (4200001,   7,  268435723) /* ClothingBase */
     , (4200001,   8,  100674869) /* Icon */
     , (4200001,  22,  872415275) /* PhysicsEffectTable */
     , (4200001,  28,        157) /* Spell - Summon Primary Portal I */
     , (4200001,  31,       7291) /* LinkedPortalOne - Halls of Metos */
     , (4200001,  36,  234881046) /* MutateFilter */;
