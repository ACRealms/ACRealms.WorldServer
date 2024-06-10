DELETE FROM `weenie` WHERE `class_Id` = 4200005;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200005, 'realm-ruleset-stamp', 44, '2005-02-09 10:00:00') /* CraftTool */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200005,   1,        128) /* ItemType - Misc */
     , (4200005,   5,         10) /* EncumbranceVal */
     , (4200005,   8,         10) /* Mass */
     , (4200005,   9,          0) /* ValidLocations - None */
     , (4200005,  11,          1) /* MaxStackSize */
     , (4200005,  12,          1) /* StackSize */
     , (4200005,  13,         10) /* StackUnitEncumbrance */
     , (4200005,  14,         10) /* StackUnitMass */
     , (4200005,  15,        1) /* StackUnitValue */
     , (4200005,  16,     524296) /* ItemUseable - SourceContainedTargetContained */
     , (4200005,  19,         1) /* Value */
     , (4200005,  93,       1044) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity */
     , (4200005,  94,       2048) /* TargetType - Gem */
     , (4200005, 150,        103) /* HookPlacement - Hook */
     , (4200005, 151,          2) /* HookType - Wall */
     , (4200005, 42005,        2);

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200005,  22, True ) /* Inscribable */
     , (4200005,  23, True ) /* DestroyOnSell */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200005,  12,       0) /* Shade */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200005,   1, 'Realm Ruleset Stamp') /* Name */
     , (4200005,  14, '') /* Use */
     , (4200005,  15, '') /* ShortDesc */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200005,   1,   33556922) /* Setup */
     , (4200005,   3,  536870932) /* SoundTable */
     , (4200005,   6,   67111092) /* PaletteBase */
     , (4200005,   7,  268436417) /* ClothingBase */
     , (4200005,   8,  100673243) /* Icon */
     , (4200005,  22,  872415275) /* PhysicsEffectTable */
     , (4200005,  50,  100673628) /* IconOverlay */
     , (4200005,  51,  100673082) /* IconOverlaySecondary */;
