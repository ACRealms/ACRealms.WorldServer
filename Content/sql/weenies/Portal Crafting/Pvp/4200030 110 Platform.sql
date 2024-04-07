DELETE FROM `weenie` WHERE `class_Id` = 4200030;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200030, '4200030', 7, '2019-08-20 01:45:15') /* Portal */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200030,   1,      65536) /* ItemType - Portal */
     , (4200030,  16,         32) /* ItemUseable - Remote */
     , (4200030,  86,        150) /* MinLevel */
     , (4200030,  93,       3084) /* PhysicsState - Ethereal, ReportCollisions, Gravity, LightingOn */
     , (4200030, 111,         49) /* PortalBitmask - Unrestricted, NoSummon, NoRecall */
     , (4200030, 133,          4) /* ShowableOnRadar - ShowAlways */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200030,   1, True ) /* Stuck */
     , (4200030,  12, True ) /* ReportCollisions */
     , (4200030,  13, True ) /* Ethereal */
     , (4200030,  14, True ) /* GravityStatus */
     , (4200030,  15, True ) /* LightsStatus */
     , (4200030,  19, True ) /* Attackable */
     , (4200030,  88, True ) /* PortalShowDestination */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200030,  54, -0.100000001490116) /* UseRadius */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200030,   1, '110 Platform') /* Name */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200030,   1,   33555925) /* Setup */
     , (4200030,   2,  150994947) /* MotionTable */
     , (4200030,   8,  100667499) /* Icon */;

INSERT INTO `weenie_properties_position` (`object_Id`, `position_Type`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`)
VALUES (4200030, 2, 60621476, 99.8339, -48.9272, 48.005, -0.999985, 0, 0, 0.005419) /* Destination */
/* @teleloc 0x039D02A4 [99.833900 -48.927200 48.005001] -0.999985 0.000000 0.000000 0.005419 */;
