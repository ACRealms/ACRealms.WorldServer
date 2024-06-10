DELETE FROM `weenie` WHERE `class_Id` = 4200009;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200009, '4200009', 7, '2019-02-04 06:52:23') /* Portal */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200009,   1,      65536) /* ItemType - Portal */
     , (4200009,  16,         32) /* ItemUseable - Remote */
     , (4200009,  86,         20) /* MinLevel */
     , (4200009,  93,       3084) /* PhysicsState - Ethereal, ReportCollisions, Gravity, LightingOn */
     , (4200009, 111,          1) /* PortalBitmask - Unrestricted */
     , (4200009, 133,          4) /* ShowableOnRadar - ShowAlways */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200009,   1, True ) /* Stuck */
     , (4200009,  11, False) /* IgnoreCollisions */
     , (4200009,  12, True ) /* ReportCollisions */
     , (4200009,  13, True ) /* Ethereal */
     , (4200009,  14, True ) /* GravityStatus */
     , (4200009,  15, True ) /* LightsStatus */
     , (4200009,  19, True ) /* Attackable */
     , (4200009,  88, True ) /* PortalShowDestination */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200009,  54, -0.100000001490116) /* UseRadius */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200009,   1, 'Disaster Maze') /* Name */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200009,   1,   33555923) /* Setup */
     , (4200009,   2,  150994947) /* MotionTable */
     , (4200009,   8,  100667499) /* Icon */;

INSERT INTO `weenie_properties_position` (`object_Id`, `position_Type`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`)
VALUES (4200009, 2, 27656724, 90, -30, 0, -0.678557, 0, 0, -0.734547) /* Destination */
/* @teleloc 0x01A60214 [90.000000 -30.000000 0.000000] -0.678557 0.000000 0.000000 -0.734547 */;
