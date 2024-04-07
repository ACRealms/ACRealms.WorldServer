DELETE FROM `weenie` WHERE `class_Id` = 4200025;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200025, 'realm-duel-gear-3', 2, '2019-02-04 06:52:23') /* Clothing */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200025,   1,          4) /* ItemType - Clothing */
     , (4200025,   3,         14) /* PaletteTemplate - Red */
     , (4200025,   4,      16384) /* ClothingPriority - Head */
     , (4200025,   5,         23) /* EncumbranceVal */
     , (4200025,   8,         15) /* Mass */
     , (4200025,   9,          1) /* ValidLocations - HeadWear */
     , (4200025,  16,          1) /* ItemUseable - No */
     , (4200025,  19,          5) /* Value */
     , (4200025,  27,          1) /* ArmorType - Cloth */
     , (4200025,  28,         600) /* ArmorLevel */
     , (4200025,  53,        101) /* PlacementPosition - Resting */
     , (4200025,  93,       1044) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity */
     , (4200025, 150,        103) /* HookPlacement - Hook */
     , (4200025, 151,          2) /* HookType - Wall */
     , (4200025,  33,          1) /* Bonded - Bonded */
     , (4200025,  114,          1) /* Attuned - Attuned */
     , (4200025, 169,  218104336) /* TsysMutationData */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200025,  11, True ) /* IgnoreCollisions */
     , (4200025,  13, True ) /* Ethereal */
     , (4200025,  14, True ) /* GravityStatus */
     , (4200025,  19, True ) /* Attackable */
     , (4200025,  22, True ) /* Inscribable */
     , (4200025, 100, True ) /* Dyable */
     , (4200025,  23, True ) /* DestroyOnSell */
     , (4200025,  69, False) /* IsSellable */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200025,  12, 0.800000011920929) /* Shade */
     , (4200025,  13, 0.800000011920929) /* ArmorModVsSlash */
     , (4200025,  14, 0.800000011920929) /* ArmorModVsPierce */
     , (4200025,  15,       1) /* ArmorModVsBludgeon */
     , (4200025,  16, 0.200000002980232) /* ArmorModVsCold */
     , (4200025,  17, 0.200000002980232) /* ArmorModVsFire */
     , (4200025,  18, 0.100000001490116) /* ArmorModVsAcid */
     , (4200025,  19, 0.200000002980232) /* ArmorModVsElectric */

     , (4200025, 165,       1) /* ArmorModVsNether */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200025,   1, 'Dueling Fez') /* Name */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200025,   1,   33556235) /* Setup */
     , (4200025,   3,  536870932) /* SoundTable */
     , (4200025,   6,   67108990) /* PaletteBase */
     , (4200025,   7,  268435858) /* ClothingBase */
     , (4200025,   8,  100670326) /* Icon */
     , (4200025,  22,  872415275) /* PhysicsEffectTable */
     , (4200025,  36,  234881046) /* MutateFilter */;

     INSERT INTO `weenie_properties_spell_book` (`object_Id`, `spell`, `probability`)
VALUES 
      (4200025,  6043,      2)  /* Legendary Light Weapon Aptitude */
     , (4200025,  6044,      2)  /* Legendary Missile Weapon Aptitude */
     , (4200025,  6046,      2)  /* Legendary Creature Enchantment Aptitude */
     , (4200025,  6047,      2)  /* Legendary Finesse Weapon Aptitude */
     , (4200025,  6048,      2)  /* Legendary Deception Prowess */
     , (4200025,  6049,      2)  /* Legendary Dirty Fighting Prowess */
     , (4200025,  6050,      2)  /* Legendary Dual Wield Aptitude */
     , (4200025,  6052,      2)  /* Legendary Fletching Prowess */
     , (4200025,  6053,      2)  /* Legendary Healing Prowess */
     , (4200025,  6054,      2)  /* Legendary Impregnability */
     , (4200025,  6055,      2)  /* Legendary Invulnerability */
     , (4200025,  6056,      2)  /* Legendary Item Enchantment Aptitude */
     , (4200025,  6058,      2)  /* Legendary Jumping Prowess */
     , (4200025,  6060,      2)  /* Legendary Life Magic Aptitude */
     , (4200025,  6063,      2)  /* Legendary Magic Resistance */
     , (4200025,  6064,      2)  /* Legendary Mana Conversion Prowess */
     , (4200025,  6066,      2)  /* Legendary Person Attunement */
     , (4200025,  6067,      2)  /* Legendary Recklessness Prowess */
     , (4200025,  6069,      2)  /* Legendary Shield Aptitude */
     , (4200025,  6070,      2)  /* Legendary Sneak Attack Prowess */
     , (4200025,  6071,      2)  /* Legendary Sprint */
     , (4200025,  6072,      2)  /* Legendary Heavy Weapon Aptitude */
     , (4200025,  6073,      2)  /* Legendary Two Handed Combat Aptitude */
     , (4200025,  6074,      2)  /* Legendary Void Magic Aptitude */
     , (4200025,  6075,      2)  /* Legendary War Magic Aptitude */
     , (4200025,  6125,      2)  /* Legendary Summoning Prowess */;