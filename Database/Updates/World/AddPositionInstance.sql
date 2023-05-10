USE ace_world;
ALTER TABLE weenie_properties_position ADD COLUMN instance TINYINT UNSIGNED AFTER position_Type;

USE ace_shard;
ALTER TABLE biota_properties_position ADD COLUMN instance TINYINT UNSIGNED AFTER position_Type;
