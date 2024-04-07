DELETE FROM `weenie` WHERE `class_Id` = 4200017;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200017, '4200017', 7, '2019-02-04 06:52:23') /* Portal */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200017,   1,      65536) /* ItemType - Portal */
     , (4200017,  16,         32) /* ItemUseable - Remote */
     , (4200017,  26,          1) /* AccountRequirements - AsheronsCall_Subscription */
     , (4200017,  93,       3084) /* PhysicsState - Ethereal, ReportCollisions, Gravity, LightingOn */
     , (4200017, 111,          1) /* PortalBitmask - Unrestricted */
     , (4200017, 133,          4) /* ShowableOnRadar - ShowAlways */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200017,   1, True ) /* Stuck */
     , (4200017,  11, False) /* IgnoreCollisions */
     , (4200017,  12, True ) /* ReportCollisions */
     , (4200017,  13, True ) /* Ethereal */
     , (4200017,  15, True ) /* LightsStatus */
     , (4200017,  88, True ) /* PortalShowDestination */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200017,  54, -0.100000001490116) /* UseRadius */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200017,   1, 'Abayar''s Laboratory') /* Name */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200017,   1,   33555925) /* Setup */
     , (4200017,   2,  150994947) /* MotionTable */
     , (4200017,   8,  100667499) /* Icon */;

INSERT INTO `weenie_properties_position` (`object_Id`, `position_Type`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`)
VALUES (4200017, 2, 26084219, 11.736, -150.022, 0.005, 0.707107, 0, 0, -0.707107) /* Destination */
/* @teleloc 0x018E037B [11.736000 -150.022003 0.005000] 0.707107 0.000000 0.000000 -0.707107 */;
