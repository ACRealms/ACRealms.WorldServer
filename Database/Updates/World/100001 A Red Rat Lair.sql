DELETE FROM `weenie` WHERE `class_Id` = 100001;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (100001, 'portalwhiteratlair3', 7, '2005-02-09 10:00:00') /* Portal */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (100001,   1,      65536) /* ItemType - Portal */
     , (100001,  16,         32) /* ItemUseable - Remote */
     , (100001,  93,       3084) /* PhysicsState - Ethereal, ReportCollisions, Gravity, LightingOn */
     , (100001, 111,          1) /* PortalBitmask - Unrestricted */
     , (100001, 133,          4) /* ShowableOnRadar - ShowAlways */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (100001,   1, True ) /* Stuck */
     , (100001,  11, False) /* IgnoreCollisions */
     , (100001,  12, True ) /* ReportCollisions */
     , (100001,  13, True ) /* Ethereal */
     , (100001,  15, True ) /* LightsStatus */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (100001,  54,    -0.1) /* UseRadius */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (100001,   1, 'A Red Rat Lair 3') /* Name */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (100001,   1,   33555922) /* Setup */
     , (100001,   2,  150994947) /* MotionTable */
     , (100001,   8,  100667499) /* Icon */;

INSERT INTO `weenie_properties_position` (`object_Id`, `position_Type`, `instance`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`)
VALUES (100001, 2, 2, 30998784, 3, -49.9, 0, 0.7071068, 0, 0, -0.7071068) /* Destination */
/* @teleloc 0x201D90100 [3.000000 -49.900000 0.000000] 0.707107 0.000000 0.000000 -0.707107 */;
