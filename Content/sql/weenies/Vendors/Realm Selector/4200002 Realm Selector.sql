DELETE FROM `weenie` WHERE `class_Id` = 4200002;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200002, 'realmselector', 12, '2024-05-31 00:10:00') /* Vendor */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200002,   1,         16) /* ItemType - Creature */
     , (4200002,   2,         31) /* CreatureType - Human */
     , (4200002,   6,         -1) /* ItemsCapacity */
     , (4200002,   7,         -1) /* ContainersCapacity */
     , (4200002,   8,        120) /* Mass */
     , (4200002,  16,         32) /* ItemUseable - Remote */
     , (4200002,  25,          0) /* Level */
     , (4200002,  27,          0) /* ArmorType - None */
     , (4200002,  74,          0) /* MerchandiseItemTypes - None */
     , (4200002,  75,          0) /* MerchandiseMinValue */
     , (4200002,  76,     100000) /* MerchandiseMaxValue */
     , (4200002,  93,    2098200) /* PhysicsState - ReportCollisions, IgnoreCollisions, Gravity, ReportCollisionsAsEnvironment */
     , (4200002, 126,       2000) /* VendorHappyMean */
     , (4200002, 127,       1000) /* VendorHappyVariance */
     , (4200002, 133,          4) /* ShowableOnRadar - ShowAlways */
     , (4200002, 134,         16) /* PlayerKillerStatus - RubberGlue */
     , (4200002, 146,        391) /* XpOverride */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200002,   1, True ) /* Stuck */
     , (4200002,  12, True ) /* ReportCollisions */
     , (4200002,  13, False) /* Ethereal */
     , (4200002,  19, False) /* Attackable */
     , (4200002,  39, True ) /* DealMagicalItems */
     , (4200002,  41, True ) /* ReportCollisionsAsEnvironment */
     , (4200002, 42000, True ) /* RealmSelectorVendor */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200002,   1,       5) /* HeartbeatInterval */
     , (4200002,   2,       0) /* HeartbeatTimestamp */
     , (4200002,   3,    0.16) /* HealthRate */
     , (4200002,   4,       5) /* StaminaRate */
     , (4200002,   5,       1) /* ManaRate */
     , (4200002,  11,     300) /* ResetInterval */
     , (4200002,  13,     0.9) /* ArmorModVsSlash */
     , (4200002,  14,       1) /* ArmorModVsPierce */
     , (4200002,  15,     1.1) /* ArmorModVsBludgeon */
     , (4200002,  16,     0.4) /* ArmorModVsCold */
     , (4200002,  17,     0.4) /* ArmorModVsFire */
     , (4200002,  18,       1) /* ArmorModVsAcid */
     , (4200002,  19,     0.6) /* ArmorModVsElectric */
     , (4200002,  37,     0.9) /* BuyPrice */
     , (4200002,  38,    1.55) /* SellPrice */
     , (4200002,  54,       3) /* UseRadius */
     , (4200002,  64,       1) /* ResistSlash */
     , (4200002,  65,       1) /* ResistPierce */
     , (4200002,  66,       1) /* ResistBludgeon */
     , (4200002,  67,       1) /* ResistFire */
     , (4200002,  68,       1) /* ResistCold */
     , (4200002,  69,       1) /* ResistAcid */
     , (4200002,  70,       1) /* ResistElectric */
     , (4200002,  71,       1) /* ResistHealthBoost */
     , (4200002,  72,       1) /* ResistStaminaDrain */
     , (4200002,  73,       1) /* ResistStaminaBoost */
     , (4200002,  74,       1) /* ResistManaDrain */
     , (4200002,  75,       1) /* ResistManaBoost */
     , (4200002, 104,      10) /* ObviousRadarRange */
     , (4200002, 125,       1) /* ResistHealthDrain */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200002,   1, 'Blaine') /* Name */
     , (4200002,   3, 'Male') /* Sex */
     , (4200002,   4, 'Empyrean') /* HeritageGroup */
     , (4200002,   5, 'Master of Realms') /* Template */
     , (4200002,  24, 'Uziz') /* TownName */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200002,   1,   33557825) /* Setup */
     , (4200002,   2,  150994945) /* MotionTable */
     , (4200002,   3,  536870913) /* SoundTable */
     , (4200002,   4,  805306368) /* CombatTable */
     , (4200002,   6,   67108990) /* PaletteBase */
     , (4200002,   7,  268436397) /* ClothingBase */
     , (4200002,   8,  100667446) /* Icon */
     , (4200002,  57,      29335) /* AlternateCurrency - Academy Exit Token */;

INSERT INTO `weenie_properties_attribute` (`object_Id`, `type`, `init_Level`, `level_From_C_P`, `c_P_Spent`)
VALUES (4200002,   1,   1, 0, 0) /* Strength */
     , (4200002,   2,   1, 0, 0) /* Endurance */
     , (4200002,   3,   1, 0, 0) /* Quickness */
     , (4200002,   4,   1, 0, 0) /* Coordination */
     , (4200002,   5,   1, 0, 0) /* Focus */
     , (4200002,   6,   1, 0, 0) /* Self */;

INSERT INTO `weenie_properties_attribute_2nd` (`object_Id`, `type`, `init_Level`, `level_From_C_P`, `c_P_Spent`, `current_Level`)
VALUES (4200002,   1,  9995, 0, 0, 165) /* MaxHealth */
     , (4200002,   3,  9990, 0, 0, 200) /* MaxStamina */
     , (4200002,   5,  9990, 0, 0, 110) /* MaxMana */;

INSERT INTO `weenie_properties_body_part` (`object_Id`, `key`, `d_Type`, `d_Val`, `d_Var`, `base_Armor`, `armor_Vs_Slash`, `armor_Vs_Pierce`, `armor_Vs_Bludgeon`, `armor_Vs_Cold`, `armor_Vs_Fire`, `armor_Vs_Acid`, `armor_Vs_Electric`, `armor_Vs_Nether`, `b_h`, `h_l_f`, `m_l_f`, `l_l_f`, `h_r_f`, `m_r_f`, `l_r_f`, `h_l_b`, `m_l_b`, `l_l_b`, `h_r_b`, `m_r_b`, `l_r_b`)
VALUES (4200002,  0,  4,  0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0, 1, 0.33,    0,    0, 0.33,    0,    0, 0.33,    0,    0, 0.33,    0,    0) /* Head */
     , (4200002,  1,  4,  0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0, 2, 0.44, 0.17,    0, 0.44, 0.17,    0, 0.44, 0.17,    0, 0.44, 0.17,    0) /* Chest */
     , (4200002,  2,  4,  0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0, 3,    0, 0.17,    0,    0, 0.17,    0,    0, 0.17,    0,    0, 0.17,    0) /* Abdomen */
     , (4200002,  3,  4,  0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0, 1, 0.23, 0.03,    0, 0.23, 0.03,    0, 0.23, 0.03,    0, 0.23, 0.03,    0) /* UpperArm */
     , (4200002,  4,  4,  0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0, 2,    0,  0.3,    0,    0,  0.3,    0,    0,  0.3,    0,    0,  0.3,    0) /* LowerArm */
     , (4200002,  5,  4,  2, 0.75,    0,    0,    0,    0,    0,    0,    0,    0,    0, 2,    0,  0.2,    0,    0,  0.2,    0,    0,  0.2,    0,    0,  0.2,    0) /* Hand */
     , (4200002,  6,  4,  0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0, 3,    0, 0.13, 0.18,    0, 0.13, 0.18,    0, 0.13, 0.18,    0, 0.13, 0.18) /* UpperLeg */
     , (4200002,  7,  4,  0,    0,    0,    0,    0,    0,    0,    0,    0,    0,    0, 3,    0,    0,  0.6,    0,    0,  0.6,    0,    0,  0.6,    0,    0,  0.6) /* LowerLeg */
     , (4200002,  8,  4,  2, 0.75,    0,    0,    0,    0,    0,    0,    0,    0,    0, 3,    0,    0, 0.22,    0,    0, 0.22,    0,    0, 0.22,    0,    0, 0.22) /* Foot */;

INSERT INTO `weenie_properties_emote` (`object_Id`, `category`, `probability`, `weenie_Class_Id`, `style`, `substyle`, `quest`, `vendor_Type`, `min_Health`, `max_Health`)
VALUES (4200002,  2 /* Vendor */,    0.8, NULL, NULL, NULL, NULL, 1 /* Open */, NULL, NULL);

SET @parent_id = LAST_INSERT_ID();

INSERT INTO `weenie_properties_emote_action` (`emote_Id`, `order`, `type`, `delay`, `extent`, `motion`, `message`, `test_String`, `min`, `max`, `min_64`, `max_64`, `min_Dbl`, `max_Dbl`, `stat`, `display`, `amount`, `amount_64`, `hero_X_P_64`, `percent`, `spell_Id`, `wealth_Rating`, `treasure_Class`, `treasure_Type`, `p_Script`, `sound`, `destination_Type`, `weenie_Class_Id`, `stack_Size`, `palette`, `shade`, `try_To_Bond`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`)
VALUES (@parent_id,  0,  10 /* Tell */, 0, 1, NULL, 'Purchase an item to choose that realm as your home world. Choose wisely! This is a permanent decision for your character.', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
