DELETE FROM `weenie` WHERE `class_Id` = 4200031;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200031, '4200031', 38, '2019-08-11 06:52:23') /* Gem */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200031,   1,       2048) /* ItemType - Gem */
     , (4200031,   3,         82) /* PaletteTemplate - PinkPurple */
     , (4200031,   5,         10) /* EncumbranceVal */
     , (4200031,   8,         10) /* Mass */
     , (4200031,  11,         25) /* MaxStackSize */
     , (4200031,  12,          1) /* StackSize */
     , (4200031,  13,         10) /* StackUnitEncumbrance */
     , (4200031,  14,         10) /* StackUnitMass */
     , (4200031,  15,          1) /* StackUnitValue */
     , (4200031,  16,          8) /* ItemUseable - Contained */
     , (4200031,  18,          1) /* UiEffects - Magical */
     , (4200031,  19,          1) /* Value */
     , (4200031,  53,        101) /* PlacementPosition - Resting */
     , (4200031,  93,       3092) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity, LightingOn */
     , (4200031,  94,         16) /* TargetType - Creature */
     , (4200031, 106,        210) /* ItemSpellcraft */
     , (4200031, 107,         50) /* ItemCurMana */
     , (4200031, 108,         50) /* ItemMaxMana */
     , (4200031, 109,          0) /* ItemDifficulty */
     , (4200031, 110,          0) /* ItemAllegianceRankLimit */
     , (4200031, 150,        103) /* HookPlacement - Hook */
     , (4200031, 151,          2) /* HookType - Wall */
     , (4200031, 280,       1000) /* SharedCooldown */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200031,  11, True ) /* IgnoreCollisions */
     , (4200031,  13, True ) /* Ethereal */
     , (4200031,  14, True ) /* GravityStatus */
     , (4200031,  15, True ) /* LightsStatus */
     , (4200031,  19, True ) /* Attackable */
     , (4200031,  23, True ) /* DestroyOnSell */
     , (4200031, 42002, True ) /* AllowRulesetStamp */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200031, 167,      15) /* CooldownDuration */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200031,   1, '110 Platform') /* Name */
     , (4200031,  15, 'This portal summoning gem works best if used outside in a relatively flat area.') /* ShortDesc */
     , (4200031,  16, 'This portal summoning gem works best if used outside in a relatively flat area.') /* LongDesc */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200031,   1,   33556769) /* Setup */
     , (4200031,   3,  536870932) /* SoundTable */
     , (4200031,   6,   67111919) /* PaletteBase */
     , (4200031,   7,  268435723) /* ClothingBase */
     , (4200031,   8,  100674869) /* Icon */
     , (4200031,  22,  872415275) /* PhysicsEffectTable */
     , (4200031,  28,        157) /* Spell - Summon Primary Portal I */
     , (4200031,  31,    4200030) /* LinkedPortalOne - 110 Platform */
     , (4200031,  36,  234881046) /* MutateFilter */;
