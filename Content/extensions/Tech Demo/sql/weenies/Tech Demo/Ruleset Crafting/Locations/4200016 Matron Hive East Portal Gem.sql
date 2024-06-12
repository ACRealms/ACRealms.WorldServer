DELETE FROM `weenie` WHERE `class_Id` = 4200016;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200016, '4200016', 38, '2019-08-11 06:52:23') /* Gem */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200016,   1,       2048) /* ItemType - Gem */
     , (4200016,   3,         82) /* PaletteTemplate - PinkPurple */
     , (4200016,   5,         10) /* EncumbranceVal */
     , (4200016,   8,         10) /* Mass */
     , (4200016,  11,         25) /* MaxStackSize */
     , (4200016,  12,          1) /* StackSize */
     , (4200016,  13,         10) /* StackUnitEncumbrance */
     , (4200016,  14,         10) /* StackUnitMass */
     , (4200016,  15,       1) /* StackUnitValue */
     , (4200016,  16,          8) /* ItemUseable - Contained */
     , (4200016,  18,          1) /* UiEffects - Magical */
     , (4200016,  19,       1) /* Value */
     , (4200016,  53,        101) /* PlacementPosition - Resting */
     , (4200016,  93,       3092) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity, LightingOn */
     , (4200016,  94,         16) /* TargetType - Creature */
     , (4200016, 106,        210) /* ItemSpellcraft */
     , (4200016, 107,         50) /* ItemCurMana */
     , (4200016, 108,         50) /* ItemMaxMana */
     , (4200016, 109,          0) /* ItemDifficulty */
     , (4200016, 110,          0) /* ItemAllegianceRankLimit */
     , (4200016, 150,        103) /* HookPlacement - Hook */
     , (4200016, 151,          2) /* HookType - Wall */
     , (4200016, 280,       1000) /* SharedCooldown */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200016,  11, True ) /* IgnoreCollisions */
     , (4200016,  13, True ) /* Ethereal */
     , (4200016,  14, True ) /* GravityStatus */
     , (4200016,  15, True ) /* LightsStatus */
     , (4200016,  19, True ) /* Attackable */
     , (4200016,  23, True ) /* DestroyOnSell */
     , (4200016,  42002, True);

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200016, 167,      15) /* CooldownDuration */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200016,   1, 'Matron Hive East') /* Name */
     , (4200016,  15, 'This portal summoning gem works best if used outside in a relatively flat area.') /* ShortDesc */
     , (4200016,  16, 'This portal summoning gem works best if used outside in a relatively flat area.') /* LongDesc */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200016,   1,   33556769) /* Setup */
     , (4200016,   3,  536870932) /* SoundTable */
     , (4200016,   6,   67111919) /* PaletteBase */
     , (4200016,   7,  268435723) /* ClothingBase */
     , (4200016,   8,  100674869) /* Icon */
     , (4200016,  22,  872415275) /* PhysicsEffectTable */
     , (4200016,  28,        157) /* Spell - Summon Primary Portal I */
     , (4200016,  31,    4200015) /* LinkedPortalOne - Deep Mukkir Nest */
     , (4200016,  36,  234881046) /* MutateFilter */;
