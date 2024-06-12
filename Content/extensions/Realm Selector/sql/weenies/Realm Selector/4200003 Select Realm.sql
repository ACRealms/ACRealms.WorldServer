DELETE FROM `weenie` WHERE `class_Id` = 4200003;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200003, 'realm-selector-token', 1, '2005-02-09 10:00:00') /* Generic */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200003,   1,    1048576) /* ItemType - Service */
     , (4200003,   5,          0) /* EncumbranceVal */
     , (4200003,   8,          0) /* Mass */
     , (4200003,   9,          0) /* ValidLocations - None */
     , (4200003,  16,          1) /* ItemUseable - No */
     , (4200003,  19,          0) /* Value */
     , (4200003,  93,       1044) /* PhysicsState - Ethereal, IgnoreCollisions, Gravity */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200003,  22, False) /* Inscribable */
     , (4200003,  51, True ) /* VendorService */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200003,   1, 'Select Realm') /* Name */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200003,   1,   33554667) /* Setup */
     , (4200003,   8,  100689461) /* Icon */
     , (4200003,  22,  872415275) /* PhysicsEffectTable */
     , (4200003,  50,  100690178) /* IconOverlay */;
