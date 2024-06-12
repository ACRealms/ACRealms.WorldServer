DELETE FROM `weenie` WHERE `class_Id` = 4200027;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200027, 'realm-duel-gear-5', 2, '2005-02-09 10:00:00') /* Clothing */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200027,   1,          2) /* ItemType - Armor */
     , (4200027,   3,         18) /* PaletteTemplate - YellowBrown */
     , (4200027,   4,       2816) /* ClothingPriority - OuterwearUpperLegs, OuterwearLowerLegs, OuterwearAbdomen */
     , (4200027,   5,       2288) /* EncumbranceVal */
     , (4200027,   8,       1275) /* Mass */
     , (4200027,   9,      25600) /* ValidLocations - AbdomenArmor, UpperLegArmor, LowerLegArmor */
     , (4200027,  16,          1) /* ItemUseable - No */
     , (4200027,  19,       3040) /* Value */
     , (4200027,  27,          2) /* ArmorType - Leather */
     , (4200027,  28,        600) /* ArmorLevel */
     , (4200027,  33,          1) /* Bonded - Bonded */
     , (4200027,  114,          1) /* Attuned - Attuned */
     , (4200027,  93,       1044) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200027,  22, True ) /* Inscribable */
     , (4200027,  23, True ) /* DestroyOnSell */
     , (4200027,  69, False) /* IsSellable */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200027,  12,     0.3) /* Shade */
     , (4200027,  13,       1) /* ArmorModVsSlash */
     , (4200027,  14,     0.8) /* ArmorModVsPierce */
     , (4200027,  15,       1) /* ArmorModVsBludgeon */
     , (4200027,  16,     0.8) /* ArmorModVsCold */
     , (4200027,  17,     0.8) /* ArmorModVsFire */
     , (4200027,  18,     0.8) /* ArmorModVsAcid */
     , (4200027,  19,     0.6) /* ArmorModVsElectric */
     , (4200027, 110,       1) /* BulkMod */
     , (4200027, 111,       1) /* SizeMod */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200027,   1, 'Dueling Leggings') /* Name */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200027,   1,   33554856) /* Setup */
     , (4200027,   3,  536870932) /* SoundTable */
     , (4200027,   6,   67108990) /* PaletteBase */
     , (4200027,   7,  268435872) /* ClothingBase */
     , (4200027,   8,  100670443) /* Icon */
     , (4200027,  22,  872415275) /* PhysicsEffectTable */;

     INSERT INTO `weenie_properties_spell_book` (`object_Id`, `spell`, `probability`)
VALUES

     (4200027,  6101,      2)  /* Legendary Willpower */

     , (4200027,  6103,      2)  /* Legendary Coordination */
     , (4200027,  6104,      2)  /* Legendary Endurance */
     , (4200027,  6105,      2)  /* Legendary Focus */
     , (4200027,  6106,      2)  /* Legendary Quickness */
     , (4200027,  6107,      2)  /* Legendary Strength */;
     