DELETE FROM `weenie` WHERE `class_Id` = 4200012;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200012, '4200012', 38, '2019-08-11 06:52:23') /* Gem */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200012,   1,       2048) /* ItemType - Gem */
     , (4200012,   3,         82) /* PaletteTemplate - PinkPurple */
     , (4200012,   5,         10) /* EncumbranceVal */
     , (4200012,   8,         10) /* Mass */
     , (4200012,  11,         25) /* MaxStackSize */
     , (4200012,  12,          1) /* StackSize */
     , (4200012,  13,         10) /* StackUnitEncumbrance */
     , (4200012,  14,         10) /* StackUnitMass */
     , (4200012,  15,       1) /* StackUnitValue */
     , (4200012,  16,          8) /* ItemUseable - Contained */
     , (4200012,  18,          1) /* UiEffects - Magical */
     , (4200012,  19,       1) /* Value */
     , (4200012,  53,        101) /* PlacementPosition - Resting */
     , (4200012,  93,       3092) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity, LightingOn */
     , (4200012,  94,         16) /* TargetType - Creature */
     , (4200012, 106,        210) /* ItemSpellcraft */
     , (4200012, 107,         50) /* ItemCurMana */
     , (4200012, 108,         50) /* ItemMaxMana */
     , (4200012, 109,          0) /* ItemDifficulty */
     , (4200012, 110,          0) /* ItemAllegianceRankLimit */
     , (4200012, 150,        103) /* HookPlacement - Hook */
     , (4200012, 151,          2) /* HookType - Wall */
     , (4200012, 280,       1000) /* SharedCooldown */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200012,  11, True ) /* IgnoreCollisions */
     , (4200012,  13, True ) /* Ethereal */
     , (4200012,  14, True ) /* GravityStatus */
     , (4200012,  15, True ) /* LightsStatus */
     , (4200012,  19, True ) /* Attackable */
     , (4200012,  23, True ) /* DestroyOnSell */
     , (4200012,  42002, True);

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200012, 167,      15) /* CooldownDuration */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200012,   1, 'Arena of the Tusker Hero') /* Name */
     , (4200012,  15, 'This portal summoning gem works best if used outside in a relatively flat area.') /* ShortDesc */
     , (4200012,  16, 'This portal summoning gem works best if used outside in a relatively flat area.') /* LongDesc */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200012,   1,   33556769) /* Setup */
     , (4200012,   3,  536870932) /* SoundTable */
     , (4200012,   6,   67111919) /* PaletteBase */
     , (4200012,   7,  268435723) /* ClothingBase */
     , (4200012,   8,  100674869) /* Icon */
     , (4200012,  22,  872415275) /* PhysicsEffectTable */
     , (4200012,  28,        157) /* Spell - Summon Primary Portal I */
     , (4200012,  31,    4200011) /* LinkedPortalOne - Deep Mukkir Nest */
     , (4200012,  36,  234881046) /* MutateFilter */;
