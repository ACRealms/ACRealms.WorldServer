DELETE FROM `weenie` WHERE `class_Id` = 4200026;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200026, 'realm-duel-gear-4', 2, '2005-02-09 10:00:00') /* Clothing */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200026,   1,          2) /* ItemType - Armor */
     , (4200026,   3,         21) /* PaletteTemplate - Gold */
     , (4200026,   4,      13312) /* ClothingPriority - OuterwearChest, OuterwearUpperArms, OuterwearLowerArms */
     , (4200026,   5,       1600) /* EncumbranceVal */
     , (4200026,   8,       1000) /* Mass */
     , (4200026,   9,       6656) /* ValidLocations - ChestArmor, UpperArmArmor, LowerArmArmor */
     , (4200026,  16,          1) /* ItemUseable - No */
     , (4200026,  19,       2610) /* Value */
     , (4200026,  27,          8) /* ArmorType - Scalemail */
     , (4200026,  28,        600) /* ArmorLevel */
     , (4200026,  33,          1) /* Bonded - Bonded */
     , (4200026,  114,          1) /* Attuned - Attuned */
     , (4200026,  93,       1044) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200026,  22, True ) /* Inscribable */
     , (4200026,  23, True ) /* DestroyOnSell */
     , (4200026,  69, False) /* IsSellable */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200026,  12,     0.3) /* Shade */
     , (4200026,  13,       1) /* ArmorModVsSlash */
     , (4200026,  14,     1.1) /* ArmorModVsPierce */
     , (4200026,  15,       1) /* ArmorModVsBludgeon */
     , (4200026,  16,     0.8) /* ArmorModVsCold */
     , (4200026,  17,     0.8) /* ArmorModVsFire */
     , (4200026,  18,     0.8) /* ArmorModVsAcid */
     , (4200026,  19,     0.5) /* ArmorModVsElectric */
     , (4200026, 110,       1) /* BulkMod */
     , (4200026, 111,       1) /* SizeMod */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200026,   1, 'Dueling Coat') /* Name */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200026,   1,   33554854) /* Setup */
     , (4200026,   3,  536870932) /* SoundTable */
     , (4200026,   6,   67108990) /* PaletteBase */
     , (4200026,   7,  268435873) /* ClothingBase */
     , (4200026,   8,  100670435) /* Icon */
     , (4200026,  22,  872415275) /* PhysicsEffectTable */;


     INSERT INTO `weenie_properties_spell_book` (`object_Id`, `spell`, `probability`)
VALUES      (4200026,  6079,      2)  /* Legendary Storm Ward */
     , (4200026,  6080,      2)  /* Legendary Acid Ward */
     , (4200026,  6081,      2)  /* Legendary Bludgeoning Ward */
     , (4200026,  6082,      2)  /* Legendary Flame Ward */
     , (4200026,  6083,      2)  /* Legendary Frost Ward */
     , (4200026,  6084,      2)  /* Legendary Piercing Ward */
     , (4200026,  6085,      2)  /* Legendary Slashing Ward */
          , (4200026,  6102,      2)  /* Legendary Armor */;