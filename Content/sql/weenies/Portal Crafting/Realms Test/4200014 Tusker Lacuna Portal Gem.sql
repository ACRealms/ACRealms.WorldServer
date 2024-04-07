DELETE FROM `weenie` WHERE `class_Id` = 4200014;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200014, '4200014', 38, '2019-08-11 06:52:23') /* Gem */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200014,   1,       2048) /* ItemType - Gem */
     , (4200014,   3,         82) /* PaletteTemplate - PinkPurple */
     , (4200014,   5,         10) /* EncumbranceVal */
     , (4200014,   8,         10) /* Mass */
     , (4200014,  11,         25) /* MaxStackSize */
     , (4200014,  12,          1) /* StackSize */
     , (4200014,  13,         10) /* StackUnitEncumbrance */
     , (4200014,  14,         10) /* StackUnitMass */
     , (4200014,  15,       1) /* StackUnitValue */
     , (4200014,  16,          8) /* ItemUseable - Contained */
     , (4200014,  18,          1) /* UiEffects - Magical */
     , (4200014,  19,       1) /* Value */
     , (4200014,  53,        101) /* PlacementPosition - Resting */
     , (4200014,  93,       3092) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity, LightingOn */
     , (4200014,  94,         16) /* TargetType - Creature */
     , (4200014, 106,        210) /* ItemSpellcraft */
     , (4200014, 107,         50) /* ItemCurMana */
     , (4200014, 108,         50) /* ItemMaxMana */
     , (4200014, 109,          0) /* ItemDifficulty */
     , (4200014, 110,          0) /* ItemAllegianceRankLimit */
     , (4200014, 150,        103) /* HookPlacement - Hook */
     , (4200014, 151,          2) /* HookType - Wall */
     , (4200014, 280,       1000) /* SharedCooldown */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200014,  11, True ) /* IgnoreCollisions */
     , (4200014,  13, True ) /* Ethereal */
     , (4200014,  14, True ) /* GravityStatus */
     , (4200014,  15, True ) /* LightsStatus */
     , (4200014,  19, True ) /* Attackable */
     , (4200014,  23, True ) /* DestroyOnSell */
     , (4200014,  42002, True);

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200014, 167,      15) /* CooldownDuration */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200014,   1, 'Tusker Lacuna') /* Name */
     , (4200014,  15, 'This portal summoning gem works best if used outside in a relatively flat area.') /* ShortDesc */
     , (4200014,  16, 'This portal summoning gem works best if used outside in a relatively flat area.') /* LongDesc */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200014,   1,   33556769) /* Setup */
     , (4200014,   3,  536870932) /* SoundTable */
     , (4200014,   6,   67111919) /* PaletteBase */
     , (4200014,   7,  268435723) /* ClothingBase */
     , (4200014,   8,  100674869) /* Icon */
     , (4200014,  22,  872415275) /* PhysicsEffectTable */
     , (4200014,  28,        157) /* Spell - Summon Primary Portal I */
     , (4200014,  31,    4200013) /* LinkedPortalOne - Deep Mukkir Nest */
     , (4200014,  36,  234881046) /* MutateFilter */;
