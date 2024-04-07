DELETE FROM `weenie` WHERE `class_Id` = 4200013;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200013, '4200013', 7, '2019-02-04 06:52:23') /* Portal */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200013,   1,      65536) /* ItemType - Portal */
     , (4200013,  16,         32) /* ItemUseable - Remote */
     , (4200013,  86,        100) /* MinLevel */
     , (4200013,  93,       3084) /* PhysicsState - Ethereal, ReportCollisions, Gravity, LightingOn */
     , (4200013, 111,         49) /* PortalBitmask - Unrestricted, NoSummon, NoRecall */
     , (4200013, 133,          4) /* ShowableOnRadar - ShowAlways */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200013,   1, True ) /* Stuck */
     , (4200013,  11, False) /* IgnoreCollisions */
     , (4200013,  12, True ) /* ReportCollisions */
     , (4200013,  13, True ) /* Ethereal */
     , (4200013,  14, True ) /* GravityStatus */
     , (4200013,  15, True ) /* LightsStatus */
     , (4200013,  19, True ) /* Attackable */
     , (4200013,  88, True ) /* PortalShowDestination */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200013,  54, -0.100000001490116) /* UseRadius */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200013,   1, 'Tusker Lacuna') /* Name */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200013,   1,   33556212) /* Setup */
     , (4200013,   2,  150994947) /* MotionTable */
     , (4200013,   8,  100667499) /* Icon */;

INSERT INTO `weenie_properties_position` (`object_Id`, `position_Type`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`)
VALUES (4200013, 2, 1497957022, 90.2216, -136.362, 12.005, 1, 0, 0, 0) /* Destination */
/* @teleloc 0x5949029E [90.221603 -136.362000 12.005000] 1.000000 0.000000 0.000000 0.000000 */;
