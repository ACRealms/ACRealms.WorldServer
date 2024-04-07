DELETE FROM `weenie` WHERE `class_Id` = 4200007;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200007, 'ace4200007-deepmukkirnest', 7, '2019-08-20 01:45:15') /* Portal */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200007,   1,      65536) /* ItemType - Portal */
     , (4200007,  16,         32) /* ItemUseable - Remote */
     , (4200007,  86,        150) /* MinLevel */
     , (4200007,  93,       3084) /* PhysicsState - Ethereal, ReportCollisions, Gravity, LightingOn */
     , (4200007, 111,         49) /* PortalBitmask - Unrestricted, NoSummon, NoRecall */
     , (4200007, 133,          4) /* ShowableOnRadar - ShowAlways */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200007,   1, True ) /* Stuck */
     , (4200007,  12, True ) /* ReportCollisions */
     , (4200007,  13, True ) /* Ethereal */
     , (4200007,  14, True ) /* GravityStatus */
     , (4200007,  15, True ) /* LightsStatus */
     , (4200007,  19, True ) /* Attackable */
     , (4200007,  88, True ) /* PortalShowDestination */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200007,  54, -0.100000001490116) /* UseRadius */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200007,   1, 'Deep Mukkir Nest') /* Name */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200007,   1,   33555925) /* Setup */
     , (4200007,   2,  150994947) /* MotionTable */
     , (4200007,   8,  100667499) /* Icon */;

INSERT INTO `weenie_properties_position` (`object_Id`, `position_Type`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`)
VALUES (4200007, 2, 5506499, 59.9318, -413.711, 0.005, 0.021953, 0, 0, -0.999759) /* Destination */
/* @teleloc 0x005405C3 [59.931801 -413.710999 0.005000] 0.021953 0.000000 0.000000 -0.999759 */;
