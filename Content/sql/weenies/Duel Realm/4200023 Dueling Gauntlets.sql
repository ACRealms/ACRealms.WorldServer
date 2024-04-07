DELETE FROM `weenie` WHERE `class_Id` = 4200023;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200023, 'realm-duel-gear-1', 2, '2019-11-05 00:00:00') /* Clothing */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200023,   1,          2) /* ItemType - Armor */
     , (4200023,   3,         20) /* PaletteTemplate - Silver */
     , (4200023,   4,      32768) /* ClothingPriority - Hands */
     , (4200023,   5,        919) /* EncumbranceVal */
     , (4200023,   8,        460) /* Mass */
     , (4200023,   9,         32) /* ValidLocations - HandWear */
     , (4200023,  16,          1) /* ItemUseable - No */
     , (4200023,  19,       1600) /* Value */
     , (4200023,  27,         32) /* ArmorType - Metal */
     , (4200023,  28,        600) /* ArmorLevel */
     , (4200023,  44,          3) /* Damage */
     , (4200023,  45,          4) /* DamageType - Bludgeon */
     , (4200023,  93,       1044) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity */
     , (4200023, 124,          3) /* Version */
     , (4200023,  33,          1) /* Bonded - Bonded */
     , (4200023,  114,         1) /* Attuned - Attuned */
     , (4200023, 169,  151651588) /* TsysMutationData */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200023,  22, True ) /* Inscribable */
     , (4200023, 100, True ) /* Dyable */
     , (4200023,  23, True ) /* DestroyOnSell */
     , (4200023,  69, False) /* IsSellable */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200023,  12,    0.66) /* Shade */
     , (4200023,  13,     1.3) /* ArmorModVsSlash */
     , (4200023,  14,       1) /* ArmorModVsPierce */
     , (4200023,  15,       1) /* ArmorModVsBludgeon */
     , (4200023,  16,     0.4) /* ArmorModVsCold */
     , (4200023,  17,     0.4) /* ArmorModVsFire */
     , (4200023,  18,     0.6) /* ArmorModVsAcid */
     , (4200023,  19,     0.4) /* ArmorModVsElectric */
     , (4200023,  22,    0.75) /* DamageVariance */
     , (4200023, 110,       1) /* BulkMod */
     , (4200023, 111,       1) /* SizeMod */
     , (4200023, 165,       1) /* ArmorModVsNether */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200023,   1, 'Dueling Gauntlets') /* Name */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200023,   1,   33554648) /* Setup */
     , (4200023,   3,  536870932) /* SoundTable */
     , (4200023,   6,   67108990) /* PaletteBase */
     , (4200023,   7,  268435473) /* ClothingBase */
     , (4200023,   8,  100667341) /* Icon */
     , (4200023,  22,  872415275) /* PhysicsEffectTable */
     , (4200023,  36,  234881042) /* MutateFilter */
     , (4200023,  46,  939524146) /* TsysMutationFilter */;

     INSERT INTO `weenie_properties_spell_book` (`object_Id`, `spell`, `probability`)
VALUES (4200023,  2006,      2)  /* Warrior's Ultimate Vitality */
     , (4200023,  2010,      2)  /* Warrior's Ultimate Vigor */
     , (4200023,  2014,      2)  /* Wizard's Ultimate Intellect */
     , (4200023,  3810,      2)  /* Asheron's Benediction */
     , (4200023,  3811,      2)  /* Blackmoor's Favor */
     , (4200023,  5146,      2)  /* Augmented Health III */
     , (4200023,  5149,      2)  /* Augmented Mana III */
     , (4200023,  5152,      2)  /* Augmented Stamina III */;

   