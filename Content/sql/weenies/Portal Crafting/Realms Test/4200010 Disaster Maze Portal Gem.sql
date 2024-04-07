DELETE FROM `weenie` WHERE `class_Id` = 4200010;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200010, '4200010', 38, '2019-08-11 06:52:23') /* Gem */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200010,   1,       2048) /* ItemType - Gem */
     , (4200010,   3,         82) /* PaletteTemplate - PinkPurple */
     , (4200010,   5,         10) /* EncumbranceVal */
     , (4200010,   8,         10) /* Mass */
     , (4200010,  11,         25) /* MaxStackSize */
     , (4200010,  12,          1) /* StackSize */
     , (4200010,  13,         10) /* StackUnitEncumbrance */
     , (4200010,  14,         10) /* StackUnitMass */
     , (4200010,  15,       1) /* StackUnitValue */
     , (4200010,  16,          8) /* ItemUseable - Contained */
     , (4200010,  18,          1) /* UiEffects - Magical */
     , (4200010,  19,       1) /* Value */
     , (4200010,  53,        101) /* PlacementPosition - Resting */
     , (4200010,  93,       3092) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity, LightingOn */
     , (4200010,  94,         16) /* TargetType - Creature */
     , (4200010, 106,        210) /* ItemSpellcraft */
     , (4200010, 107,         50) /* ItemCurMana */
     , (4200010, 108,         50) /* ItemMaxMana */
     , (4200010, 109,          0) /* ItemDifficulty */
     , (4200010, 110,          0) /* ItemAllegianceRankLimit */
     , (4200010, 150,        103) /* HookPlacement - Hook */
     , (4200010, 151,          2) /* HookType - Wall */
     , (4200010, 280,       1000) /* SharedCooldown */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200010,  11, True ) /* IgnoreCollisions */
     , (4200010,  13, True ) /* Ethereal */
     , (4200010,  14, True ) /* GravityStatus */
     , (4200010,  15, True ) /* LightsStatus */
     , (4200010,  19, True ) /* Attackable */
     , (4200010,  23, True ) /* DestroyOnSell */
     , (4200010,  42002, True);

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200010, 167,      15) /* CooldownDuration */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200010,   1, 'Disaster Maze') /* Name */
     , (4200010,  15, 'This portal summoning gem works best if used outside in a relatively flat area.') /* ShortDesc */
     , (4200010,  16, 'This portal summoning gem works best if used outside in a relatively flat area.') /* LongDesc */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200010,   1,   33556769) /* Setup */
     , (4200010,   3,  536870932) /* SoundTable */
     , (4200010,   6,   67111919) /* PaletteBase */
     , (4200010,   7,  268435723) /* ClothingBase */
     , (4200010,   8,  100674869) /* Icon */
     , (4200010,  22,  872415275) /* PhysicsEffectTable */
     , (4200010,  28,        157) /* Spell - Summon Primary Portal I */
     , (4200010,  31,    4200009) /* LinkedPortalOne - Disaster Maze */
     , (4200010,  36,  234881046) /* MutateFilter */;
