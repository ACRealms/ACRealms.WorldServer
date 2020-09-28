USE realms_world;
ALTER TABLE weenie_properties_position MODIFY COLUMN instance int UNSIGNED;

USE realms_shard;
ALTER TABLE biota_properties_position MODIFY COLUMN instance int UNSIGNED;
