use ace_world;

DROP TABLE IF EXISTS `realm_properties_string`;
DROP TABLE IF EXISTS `realm_properties_bool`;
DROP TABLE IF EXISTS `realm_properties_float`;
DROP TABLE IF EXISTS `realm_properties_int`;
DROP TABLE IF EXISTS `realm_properties_int64`;
DROP TABLE IF EXISTS `realm`;

CREATE TABLE `realm` (
  `id` smallint unsigned NOT NULL AUTO_INCREMENT COMMENT 'Unique Realm Id within the Shard',
  `type` smallint unsigned NOT NULL,
  `name` text NOT NULL COMMENT 'Name of this realm',
  `parent_realm_id` smallint unsigned NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='Dynamic Realm of a Shard/World';


CREATE TABLE `realm_properties_string` (
  `realm_Id` smallint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to',
  `type` smallint unsigned NOT NULL DEFAULT '0' COMMENT 'Type of Property the value applies to (RealmPropertyString.????)',
  `value` text NOT NULL COMMENT 'Value of this Property',
  PRIMARY KEY (`realm_Id`,`type`),
  KEY `idx_type` (`type`),
  CONSTRAINT `realm_string` FOREIGN KEY (`realm_Id`) REFERENCES `realm` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='String Properties of Realms';

CREATE TABLE `realm_properties_int64` (
  `realm_Id` smallint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to',
  `type` smallint unsigned NOT NULL DEFAULT '0' COMMENT 'Type of Property the value applies to (RealmPropertyInt64.????)',
  `value` bigint NOT NULL DEFAULT '0' COMMENT 'Value of this Property',
  PRIMARY KEY (`realm_Id`,`type`),
  CONSTRAINT `realm_int64` FOREIGN KEY (`realm_Id`) REFERENCES `realm` (`id`) ON DELETE CASCADE
  
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='Int64 Properties of Realms';


CREATE TABLE `realm_properties_int` (
  `realm_Id` smallint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to',
  `type` smallint unsigned NOT NULL DEFAULT '0' COMMENT 'Type of Property the value applies to (RealmPropertyInt.????)',
  `value` int NOT NULL DEFAULT '0' COMMENT 'Value of this Property',
  PRIMARY KEY (`realm_Id`,`type`),
  KEY `idx_type` (`type`),
  CONSTRAINT `realm_int` FOREIGN KEY (`realm_Id`) REFERENCES `realm` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='Int Properties of Realms';


CREATE TABLE `realm_properties_float` (
  `realm_Id` smallint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to',
  `type` smallint unsigned NOT NULL DEFAULT '0' COMMENT 'Type of Property the value applies to (RealmPropertyFloat.????)',
  `value` double NOT NULL DEFAULT '0' COMMENT 'Value of this Property',
  PRIMARY KEY (`realm_Id`,`type`),
  KEY `idx_type` (`type`),
  CONSTRAINT `realm_float` FOREIGN KEY (`realm_Id`) REFERENCES `realm` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='Float Properties of Realms';


CREATE TABLE `realm_properties_bool` (
  `realm_Id` smallint unsigned NOT NULL DEFAULT '0' COMMENT 'Id of the object this property belongs to',
  `type` smallint unsigned NOT NULL DEFAULT '0' COMMENT 'Type of Property the value applies to (RealmPropertyBool.????)',
  `value` bit(1) NOT NULL COMMENT 'Value of this Property',
  PRIMARY KEY (`realm_Id`,`type`),
  KEY `idx_type` (`type`),
  CONSTRAINT `realm_bool` FOREIGN KEY (`realm_Id`) REFERENCES `realm` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='Bool Properties of Realms';
