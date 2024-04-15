USE realms_shard;

ALTER TABLE biota_properties_allegiance DROP FOREIGN KEY FK_allegiance_character_Id;
ALTER TABLE character_properties_contract_registry DROP FOREIGN KEY wcid_contract;
ALTER TABLE character_properties_fill_comp_book DROP FOREIGN KEY wcid_fillcompbook;
ALTER TABLE character_properties_friend_list DROP FOREIGN KEY wcid_friend;
ALTER TABLE character_properties_quest_registry DROP FOREIGN KEY wcid_questbook;
ALTER TABLE character_properties_shortcut_bar DROP FOREIGN KEY wcid_shortcutbar;
ALTER TABLE character_properties_spell_bar DROP FOREIGN KEY characterId_spellbar;
ALTER TABLE character_properties_squelch DROP FOREIGN KEY squelch_character_Id_constraint;
ALTER TABLE character_properties_title_book DROP FOREIGN KEY wcid_titlebook;
ALTER TABLE character_properties_spell_bar DROP FOREIGN KEY wcid_spellbar;

ALTER TABLE `biota_properties_allegiance` CHANGE `character_Id` `character_Id` bigint unsigned;
ALTER TABLE `character_properties_contract_registry` CHANGE `character_Id` `character_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the character this property belongs to';
ALTER TABLE `character_properties_fill_comp_book` CHANGE `character_Id` `character_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the character this property belongs to';
ALTER TABLE `character_properties_friend_list` CHANGE `character_Id` `character_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the character this property belongs to';
ALTER TABLE `character_properties_quest_registry` CHANGE `character_Id` `character_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the character this property belongs to';
ALTER TABLE `character_properties_shortcut_bar` CHANGE `character_Id` `character_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the character this property belongs to';
ALTER TABLE `character_properties_spell_bar` CHANGE `character_Id` `character_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the character this property belongs to';
ALTER TABLE `character_properties_squelch` CHANGE `character_Id` `character_Id` bigint unsigned NOT NULL;
ALTER TABLE `character_properties_title_book` CHANGE `character_Id` `character_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the character this property belongs to';

ALTER TABLE `character` CHANGE `id` `id` bigint unsigned

ALTER TABLE `biota_properties_allegiance` ADD CONSTRAINT `FK_allegiance_character_Id` FOREIGN KEY(character_Id) REFERENCES `character`(id);
ALTER TABLE `character_properties_contract_registry` ADD CONSTRAINT `wcid_contract` FOREIGN KEY(character_Id) REFERENCES `character`(id);
ALTER TABLE `character_properties_fill_comp_book` ADD CONSTRAINT `wcid_fillcompbook` FOREIGN KEY(character_Id) REFERENCES `character`(id);
ALTER TABLE `character_properties_friend_list` ADD CONSTRAINT `wcid_friend` FOREIGN KEY(character_Id) REFERENCES `character`(id);
ALTER TABLE `character_properties_quest_registry` ADD CONSTRAINT `wcid_questbook` FOREIGN KEY(character_Id) REFERENCES `character`(id);
ALTER TABLE `character_properties_shortcut_bar` ADD CONSTRAINT `wcid_shortcutbar` FOREIGN KEY(character_Id) REFERENCES `character`(id);
ALTER TABLE `character_properties_spell_bar` ADD CONSTRAINT `characterId_spellbar` FOREIGN KEY(character_Id) REFERENCES `character`(id);
ALTER TABLE `character_properties_squelch` ADD CONSTRAINT `squelch_character_Id_constraint` FOREIGN KEY(character_Id) REFERENCES `character`(id);
ALTER TABLE `character_properties_title_book` ADD CONSTRAINT `wcid_titlebook` FOREIGN KEY(character_Id) REFERENCES `character`(id);
ALTER TABLE `character_properties_spell_bar` ADD CONSTRAINT `wcid_spellbar` FOREIGN KEY(character_Id) REFERENCES `character`(id);

ALTER TABLE biota_properties_allegiance DROP FOREIGN KEY FK_allegiance_biota_Id;
ALTER TABLE biota_properties_anim_part DROP FOREIGN KEY wcid_animpart;
ALTER TABLE biota_properties_attribute DROP FOREIGN KEY wcid_attribute;
ALTER TABLE biota_properties_attribute_2nd DROP FOREIGN KEY wcid_attribute2nd;
ALTER TABLE biota_properties_body_part DROP FOREIGN KEY wcid_bodypart;
ALTER TABLE biota_properties_book DROP FOREIGN KEY wcid_bookdata;
ALTER TABLE biota_properties_book_page_data DROP FOREIGN KEY wcid_pagedata;
ALTER TABLE biota_properties_bool DROP FOREIGN KEY wcid_bool;
ALTER TABLE biota_properties_create_list DROP FOREIGN KEY wcid_createlist;
ALTER TABLE biota_properties_d_i_d DROP FOREIGN KEY wcid_did;
ALTER TABLE biota_properties_emote DROP FOREIGN KEY wcid_emote;
ALTER TABLE biota_properties_enchantment_registry DROP FOREIGN KEY wcid_enchantmentregistry;
ALTER TABLE biota_properties_event_filter DROP FOREIGN KEY wcid_eventfilter;
ALTER TABLE biota_properties_float DROP FOREIGN KEY wcid_float;
ALTER TABLE biota_properties_generator DROP FOREIGN KEY wcid_generator;
ALTER TABLE biota_properties_i_i_d DROP FOREIGN KEY wcid_iid;
ALTER TABLE biota_properties_int DROP FOREIGN KEY wcid_int;
ALTER TABLE biota_properties_int64 DROP FOREIGN KEY wcid_int64;
ALTER TABLE biota_properties_palette DROP FOREIGN KEY wcid_palette;
ALTER TABLE biota_properties_position DROP FOREIGN KEY wcid_position;
ALTER TABLE biota_properties_skill DROP FOREIGN KEY wcid_skill;
ALTER TABLE biota_properties_spell_book DROP FOREIGN KEY wcid_spellbook;
ALTER TABLE biota_properties_string DROP FOREIGN KEY wcid_string;
ALTER TABLE biota_properties_texture_map DROP FOREIGN KEY wcid_texturemap;
ALTER TABLE house_permission DROP FOREIGN KEY biota_Id_house_Id;

ALTER TABLE `biota_properties_allegiance` CHANGE `allegiance_Id` `allegiance_Id` bigint unsigned;
ALTER TABLE `biota_properties_anim_part` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to';
ALTER TABLE `biota_properties_attribute` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to';
ALTER TABLE `biota_properties_attribute_2nd` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to';
ALTER TABLE `biota_properties_body_part` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to';
ALTER TABLE `biota_properties_book` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to';
ALTER TABLE `biota_properties_book_page_data` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the Book object this page belongs to';
ALTER TABLE `biota_properties_bool` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to';
ALTER TABLE `biota_properties_create_list` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to';
ALTER TABLE `biota_properties_d_i_d` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to';
ALTER TABLE `biota_properties_emote` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to';
ALTER TABLE `biota_properties_enchantment_registry` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to';
ALTER TABLE `biota_properties_event_filter` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to';
ALTER TABLE `biota_properties_float` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to';
ALTER TABLE `biota_properties_generator` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to';
ALTER TABLE `biota_properties_i_i_d` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to';
ALTER TABLE `biota_properties_int` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to';
ALTER TABLE `biota_properties_int64` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to';
ALTER TABLE `biota_properties_palette` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to';
ALTER TABLE `biota_properties_position` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL COMMENT 'Id of the object this property belongs to';
ALTER TABLE `biota_properties_skill` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to';
ALTER TABLE `biota_properties_spell_book` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to';
ALTER TABLE `biota_properties_string` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to';
ALTER TABLE `biota_properties_texture_map` CHANGE `object_Id` `object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to';
ALTER TABLE `house_permission` CHANGE `house_Id` `house_Id` bigint unsigned NOT NULL COMMENT 'GUID of House Biota Object';

ALTER TABLE `biota` CHANGE `id` `id` bigint unsigned

ALTER TABLE `biota_properties_allegiance` ADD CONSTRAINT `FK_allegiance_biota_Id` FOREIGN KEY(allegiance_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_anim_part` ADD CONSTRAINT `wcid_animpart` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_attribute` ADD CONSTRAINT `wcid_attribute` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_attribute_2nd` ADD CONSTRAINT `wcid_attribute2nd` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_body_part` ADD CONSTRAINT `wcid_bodypart` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_book` ADD CONSTRAINT `wcid_bookdata` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_book_page_data` ADD CONSTRAINT `wcid_pagedata` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_bool` ADD CONSTRAINT `wcid_bool` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_create_list` ADD CONSTRAINT `wcid_createlist` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_d_i_d` ADD CONSTRAINT `wcid_did` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_emote` ADD CONSTRAINT `wcid_emote` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_enchantment_registry` ADD CONSTRAINT `wcid_enchantmentregistry` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_event_filter` ADD CONSTRAINT `wcid_eventfilter` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_float` ADD CONSTRAINT `wcid_float` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_generator` ADD CONSTRAINT `wcid_generator` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_i_i_d` ADD CONSTRAINT `wcid_iid` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_int` ADD CONSTRAINT `wcid_int` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_int64` ADD CONSTRAINT `wcid_int64` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_palette` ADD CONSTRAINT `wcid_palette` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_position` ADD CONSTRAINT `wcid_position` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_skill` ADD CONSTRAINT `wcid_skill` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_spell_book` ADD CONSTRAINT `wcid_spellbook` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_string` ADD CONSTRAINT `wcid_string` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `biota_properties_texture_map` ADD CONSTRAINT `wcid_texturemap` FOREIGN KEY(object_Id) REFERENCES biota(id);
ALTER TABLE `house_permission` ADD CONSTRAINT `biota_Id_house_Id` FOREIGN KEY(house_Id) REFERENCES biota(id);



ALTER TABLE `biota_properties_book_page_data` CHANGE `author_Id` `author_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the Author of this page';
ALTER TABLE `biota_properties_emote_action` CHANGE `Id` `Id` bigint unsigned NOT NULL AUTO_INCREMENT COMMENT 'Unique Id of this Property';
ALTER TABLE `biota_properties_enchantment_registry` CHANGE `caster_Object_Id` `caster_Object_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object that cast this spell';
ALTER TABLE `biota_properties_i_i_d` CHANGE `value` `value` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Value of this Property';
ALTER TABLE `character_properties_friend_list` CHANGE `friend_Id` `friend_Id` bigint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of Friend';