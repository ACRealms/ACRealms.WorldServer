DELETE FROM `weenie` WHERE `class_Id` = 4200021;

INSERT INTO `weenie` (`class_Id`, `class_Name`, `type`, `last_Modified`)
VALUES (4200021, 'ace4200021-creepycanyons', 7, '2019-03-26 20:02:53') /* Portal */;

INSERT INTO `weenie_properties_int` (`object_Id`, `type`, `value`)
VALUES (4200021,   1,      65536) /* ItemType - Portal */
     , (4200021,  16,         32) /* ItemUseable - Remote */
     , (4200021,  86,        100) /* MinLevel */
     , (4200021,  93,       3084) /* PhysicsState - Ethereal, ReportCollisions, Gravity, LightingOn */
     , (4200021, 111,          1) /* PortalBitmask - Unrestricted */
     , (4200021, 133,          4) /* ShowableOnRadar - ShowAlways */;

INSERT INTO `weenie_properties_bool` (`object_Id`, `type`, `value`)
VALUES (4200021,   1, True ) /* Stuck */
     , (4200021,  12, True ) /* ReportCollisions */
     , (4200021,  13, True ) /* Ethereal */
     , (4200021,  14, True ) /* GravityStatus */
     , (4200021,  15, True ) /* LightsStatus */
     , (4200021,  19, True ) /* Attackable */
     , (4200021,  88, True ) /* PortalShowDestination */;

INSERT INTO `weenie_properties_float` (`object_Id`, `type`, `value`)
VALUES (4200021,  54, -0.100000001490116) /* UseRadius */;

INSERT INTO `weenie_properties_string` (`object_Id`, `type`, `value`)
VALUES (4200021,   1, 'Creepy Canyons') /* Name */
     , (4200021,  33, 'lostpetportal') /* Quest */;

INSERT INTO `weenie_properties_d_i_d` (`object_Id`, `type`, `value`)
VALUES (4200021,   1,   33555925) /* Setup */
     , (4200021,   2,  150994947) /* MotionTable */
     , (4200021,   8,  100667499) /* Icon */;

INSERT INTO `weenie_properties_position` (`object_Id`, `position_Type`, `obj_Cell_Id`, `origin_X`, `origin_Y`, `origin_Z`, `angles_W`, `angles_X`, `angles_Y`, `angles_Z`)
VALUES (4200021, 2, 3212088, 150.537, -200.946, -17.995, 0.998491, 0, 0, -0.054907) /* Destination */
/* @teleloc 0x00310338 [150.537003 -200.945999 -17.995001] 0.998491 0.000000 0.000000 -0.054907 */;
