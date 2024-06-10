DELETE FROM `weenie` WHERE `class_Id` = 4200028;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200028, 'realm-duel-gear-6', 35, '2019-08-11 06:52:23') /* Caster */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200028,   1,      32768) /* ItemType - Caster */
     , (4200028,   5,        150) /* EncumbranceVal */
     , (4200028,   8,         10) /* Mass */
     , (4200028,   9,   16777216) /* ValidLocations - Held */
     , (4200028,  16,    6291464) /* ItemUseable - SourceContainedTargetRemoteNeverWalk */
     , (4200028,  18,          1) /* UiEffects - Magical */
     , (4200028,  19,       8000) /* Value */
     , (4200028,  33,          1) /* Bonded - Bonded */
     , (4200028,  36,       9999) /* ResistMagic */
     , (4200028,  46,        512) /* DefaultCombatStyle - Magic */
     , (4200028,  52,          1) /* ParentLocation - RightHand */
     , (4200028,  53,        101) /* PlacementPosition - Resting */
     , (4200028,  93,       1044) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity */
     , (4200028,  94,         16) /* TargetType - Creature */
     , (4200028, 114,          1) /* Attuned - Attuned */
     , (4200028, 150,        103) /* HookPlacement - Hook */
     , (4200028, 151,          2) /* HookType - Wall */
     , (4200028, 166,         31) /* SlayerCreatureType - Human */
     , (4200028, 353,          0) /* WeaponType - Undef */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200028,  11, True ) /* IgnoreCollisions */
     , (4200028,  13, True ) /* Ethereal */
     , (4200028,  14, True ) /* GravityStatus */
     , (4200028,  19, True ) /* Attackable */
     , (4200028,  22, True ) /* Inscribable */
     , (4200028,  23, True ) /* DestroyOnSell */
     , (4200028,  69, False) /* IsSellable */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200028,   5, -0.025000000372529) /* ManaRate */
     , (4200028,  29,       1) /* WeaponDefense */
     , (4200028,  39,       1) /* DefaultScale */
     , (4200028, 138, 1.39999997615814) /* SlayerDamageBonus */
     , (4200028, 144, 0.0199999995529652) /* ManaConversionMod */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200028,   1, 'Dueling Wand') /* Name */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200028,   1,   33558300) /* Setup */
     , (4200028,   3,  536870932) /* SoundTable */
     , (4200028,   8,  100674265) /* Icon */
     , (4200028,  22,  872415275) /* PhysicsEffectTable */
     , (4200028,  27, 1073742049) /* UseUserAnimation - UseMagicWand */
     , (4200028,  36,  234881046) /* MutateFilter */;
