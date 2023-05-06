use ace_world;

DROP TABLE IF EXISTS `realm_properties_string`;
DROP TABLE IF EXISTS `realm_properties_bool`;
DROP TABLE IF EXISTS `realm_properties_float`;
DROP TABLE IF EXISTS `realm_properties_int`;
DROP TABLE IF EXISTS `realm_properties_int64`;
DROP TABLE IF EXISTS `realm_ruleset_links`;
DROP TABLE IF EXISTS `realm`;

CREATE TABLE `realm` (
  `id` smallint unsigned NOT NULL DEFAULT 0 COMMENT 'Unique Realm Id within the Shard',
  `type` smallint unsigned NOT NULL,
  `name` text NOT NULL COMMENT 'Name of this realm',
  `parent_realm_id` smallint unsigned NULL,
  `property_count_randomized` smallint unsigned NULL COMMENT 'Maximum number of properties that will be picked from the ruleset at random.',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='Dynamic Realm of a Shard/World';

CREATE TABLE `realm_ruleset_links` (
  `realm_id` smallint unsigned NOT NULL,
  `order` smallint unsigned NOT NULL,
  `link_type` smallint unsigned NOT NULL,
  `linked_realm_id` smallint unsigned NOT NULL,
  `probability_group` tinyint unsigned NULL,
  `probability` double NULL COMMENT 'A random number between 0 and 1 will be generated. The first link with a probability value greater than the number value in the probability_group will be applied, and the rest in the group ignored (per landblock).',
  PRIMARY KEY (`realm_id`, `order`),
  CONSTRAINT `realm_link_parent` FOREIGN KEY (`realm_Id`) REFERENCES `realm` (`id`) ON DELETE CASCADE,
  CONSTRAINT `realm_link_child` FOREIGN KEY (`linked_realm_id`) REFERENCES `realm` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `realm_properties_string` (
  `realm_Id` smallint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to',
  `type` smallint unsigned NOT NULL DEFAULT '0' COMMENT 'Type of Property the value applies to (RealmPropertyString.????)',
  `value` text NOT NULL COMMENT 'Value of this Property',
  `locked` bit(1) NOT NULL COMMENT 'If true, this property cannot be overriden by inherited realms or rulesets.',
  `probability` double NULL,
  PRIMARY KEY (`realm_Id`,`type`),
  KEY `idx_type` (`type`),
  CONSTRAINT `realm_string` FOREIGN KEY (`realm_Id`) REFERENCES `realm` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='String Properties of Realms';

CREATE TABLE `realm_properties_int64` (
  `realm_Id` smallint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to',
  `type` smallint unsigned NOT NULL DEFAULT '0' COMMENT 'Type of Property the value applies to (RealmPropertyInt64.????)',
  `value` bigint NULL COMMENT 'Value of this Property',
  `locked` bit(1) NOT NULL COMMENT 'If true, this property cannot be overriden by inherited realms or rulesets.',
  `probability` double NULL,
  `random_low_range` bigint NULL,
  `random_high_range` bigint NULL,
  `random_type` tinyint unsigned NOT NULL,
  `composition_type` tinyint unsigned NOT NULL,
  PRIMARY KEY (`realm_Id`,`type`),
  CONSTRAINT `realm_int64` FOREIGN KEY (`realm_Id`) REFERENCES `realm` (`id`) ON DELETE CASCADE
  
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='Int64 Properties of Realms';


CREATE TABLE `realm_properties_int` (
  `realm_Id` smallint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to',
  `type` smallint unsigned NOT NULL DEFAULT '0' COMMENT 'Type of Property the value applies to (RealmPropertyInt.????)',
  `value` int NULL COMMENT 'Value of this Property',
  `locked` bit(1) NOT NULL COMMENT 'If true, this property cannot be overriden by inherited realms or rulesets.',
  `random_low_range` int NULL,
  `random_high_range` int NULL,
  `probability` double NULL,
  `random_type` tinyint unsigned NOT NULL,
  `composition_type` tinyint unsigned NOT NULL,
  PRIMARY KEY (`realm_Id`,`type`),
  KEY `idx_type` (`type`),
  CONSTRAINT `realm_int` FOREIGN KEY (`realm_Id`) REFERENCES `realm` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='Int Properties of Realms';


CREATE TABLE `realm_properties_float` (
  `realm_Id` smallint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to',
  `type` smallint unsigned NOT NULL DEFAULT '0' COMMENT 'Type of Property the value applies to (RealmPropertyFloat.????)',
  `value` double NULL COMMENT 'Value of this Property',
  `locked` bit(1) NOT NULL COMMENT 'If true, this property cannot be overriden by inherited realms or rulesets.',
  `random_low_range` double NULL,
  `random_high_range` double NULL,
  `probability` double NULL,
  `random_type` tinyint unsigned NOT NULL,
  `composition_type` tinyint unsigned NOT NULL,
  PRIMARY KEY (`realm_Id`,`type`),
  KEY `idx_type` (`type`),
  CONSTRAINT `realm_float` FOREIGN KEY (`realm_Id`) REFERENCES `realm` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='Float Properties of Realms';


CREATE TABLE `realm_properties_bool` (
  `realm_Id` smallint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to',
  `type` smallint unsigned NOT NULL DEFAULT '0' COMMENT 'Type of Property the value applies to (RealmPropertyBool.????)',
  `value` bit(1) NOT NULL COMMENT 'Value of this Property',
  `locked` bit(1) NOT NULL COMMENT 'If true, this property cannot be overriden by inherited realms or rulesets.',
  `probability` double NULL,
  PRIMARY KEY (`realm_Id`,`type`),
  KEY `idx_type` (`type`),
  CONSTRAINT `realm_bool` FOREIGN KEY (`realm_Id`) REFERENCES `realm` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='Bool Properties of Realms';
