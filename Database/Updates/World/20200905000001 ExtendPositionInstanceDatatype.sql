USE ace_world;
ALTER TABLE weenie_properties_position MODIFY COLUMN instance int UNSIGNED;

USE ace_shard;
ALTER TABLE biota_properties_position MODIFY COLUMN instance int UNSIGNED;
