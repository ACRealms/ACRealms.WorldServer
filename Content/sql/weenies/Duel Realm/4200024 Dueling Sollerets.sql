DELETE FROM `weenie` WHERE `class_Id` = 4200024;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200024, 'realm-duel-gear-2', 2, '2019-11-05 00:00:00') /* Clothing */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200024,   1,          2) /* ItemType - Armor */
     , (4200024,   3,         20) /* PaletteTemplate - Silver */
     , (4200024,   4,      65536) /* ClothingPriority - Feet */
     , (4200024,   5,        540) /* EncumbranceVal */
     , (4200024,   8,        360) /* Mass */
     , (4200024,   9,        256) /* ValidLocations - FootWear */
     , (4200024,  16,          1) /* ItemUseable - No */
     , (4200024,  19,        653) /* Value */
     , (4200024,  27,         32) /* ArmorType - Metal */
     , (4200024,  33,          1) /* Bonded - Bonded */
     , (4200024,  28,        600) /* ArmorLevel */
     , (4200024,  44,          3) /* Damage */
     , (4200024,  45,          4) /* DamageType - Bludgeon */
     , (4200024,  93,       1044) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity */
     , (4200024, 124,          3) /* Version */
     , (4200024,  114,          1) /* Attuned - Attuned */
     , (4200024, 169,  151650564) /* TsysMutationData */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200024,  22, True ) /* Inscribable */
     , (4200024, 100, True ) /* Dyable */
     , (4200024,  23, True ) /* DestroyOnSell */
     , (4200024,  69, False) /* IsSellable */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200024,  12,    0.66) /* Shade */
     , (4200024,  13,     1.3) /* ArmorModVsSlash */
     , (4200024,  14,       1) /* ArmorModVsPierce */
     , (4200024,  15,       1) /* ArmorModVsBludgeon */
     , (4200024,  16,     0.4) /* ArmorModVsCold */
     , (4200024,  17,     0.4) /* ArmorModVsFire */
     , (4200024,  18,     0.6) /* ArmorModVsAcid */
     , (4200024,  19,     0.4) /* ArmorModVsElectric */
     , (4200024,  22,    0.75) /* DamageVariance */
     , (4200024, 110,       1) /* BulkMod */
     , (4200024, 111,       1) /* SizeMod */
     , (4200024, 165,       1) /* ArmorModVsNether */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200024,   1, 'Dueling Sollerets') /* Name */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200024,   1,   33554654) /* Setup */
     , (4200024,   3,  536870932) /* SoundTable */
     , (4200024,   6,   67108990) /* PaletteBase */
     , (4200024,   7,  268435540) /* ClothingBase */
     , (4200024,   8,  100667309) /* Icon */
     , (4200024,  22,  872415275) /* PhysicsEffectTable */
     , (4200024,  36,  234881042) /* MutateFilter */
     , (4200024,  46,  939524146) /* TsysMutationFilter */;

     INSERT INTO `weenie_properties_spell_book` (`object_Id`, `spell`, `probability`)
VALUES 
      (4200024,  4803,      2)  /* Master Archer's Missile Weapon Aptitude */
     , (4200024,  4811,      2)  /* Master Enchanter's Creature Aptitude */
     , (4200024,  4815,      2)  /* Master Archer's Missile Weapon Aptitude */
     , (4200024,  4819,      2)  /* Master Soldier's Finesse Weapon Aptitude */
     , (4200024,  4827,      2)  /* Master Artifex's Item Aptitude */
     , (4200024,  4839,      2)  /* Master Theurge's Life Magic Aptitude */
     , (4200024,  4883,      2)  /* Master Soldier's Heavy Weapon Aptitude */
     , (4200024,  4887,      2)  /* Master Archer's Missile Weapon Aptitude */
     , (4200024,  4891,      2)  /* Master Soldier's Light Weapon Aptitude */
     , (4200024,  4895,      2)  /* Master Warlock's War Magic Aptitude */
     , (4200024,  5110,      2)  /* Master Soldier's Two Handed Combat Aptitude */
          , (4200024,  5434,      2)  /* Master Voidlock's Void Magic Aptitude */
     , (4200024,  5949,      2)  /* Master Soldier's Dirty Fighting Aptitude */
     , (4200024,  5953,      2)  /* Master Soldier's Dual Wield Aptitude */
     , (4200024,  5957,      2)  /* Master Soldier's Recklessness Aptitude */
     , (4200024,  5961,      2)  /* Master Soldier's Shield Aptitude */
     , (4200024,  5965,      2)  /* Master Soldier's Sneak Attack Aptitude */;