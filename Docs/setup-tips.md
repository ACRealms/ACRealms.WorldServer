## General
### Make sure you have a decent text editor

Use a real text editor that can handle large files, not Notepad. Here are some recommendations:
- Sublime Text
- Notepad++
- Visual Studio Code

## MySQL

- Recommended installation:
	- [MySQL Community Server](https://dev.mysql.com/downloads/mysql/)
	- [MySQL Workbench](https://www.mysql.com/products/workbench/)

After installing, make sure you update the PATH environment variable to include the MySQL "bin" folder (leaving other PATH entries intact)
https://superuser.com/a/949577
https://www.wikihow.com/Change-the-PATH-Environment-Variable-on-Windows

If you cannot find your MySQL installation folder, and are using Windows, you can find it through a couple of methods:
1. Open Services.msc (from start menu) - look for a mysql service, right click, click properties, note the folder that contains bin\mysqld
2. (All Platforms) open MySQL Workbench, connect to your database, run the SQL command `select @@datadir;` then go to the installation folder and look for the bin folder that contains the mysql executable

To confirm that you have set the PATH correctly, open a command prompt or shell (from any folder, not the mysql bin folder), and run the "mysql" command, and you should see either `mysql>` or something containing `Access denied for user`. Either one means the path is set and you can run mysql.exe regardless of the working directory of your shell session.


### Understanding how to use backups

To take a backup of the database, make sure the ACRealms server is not running, then use the mysqldump command (example below) to generate a dump script. If you have special characters in your password you may not be able to use the commands in this way.

This command will back up the realms_shard database to a file called `2024-06-16-realms-shard.sql` assuming your db username is USERNAME and db password is PASSWORD:  

`mysqldump -uUSERNAME -pPASSWORD realms_shard > 2024-06-16-realms-shard.sql`

Now open this file `2024-06-16-realms-shard.sql` in your text editor (not notepad). This is just a list of SQL commands that will run. Notice at the top it will have a list of commands that will run, starting with:

```
-- MySQL dump 10.13  Distrib 8.0.32, for Win64 (x86_64)
--
-- Host: localhost    Database: realms_shard
-- ------------------------------------------------------
-- Server version	8.0.32

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `biota`
--

DROP TABLE IF EXISTS `biota`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `biota` (
  `id` bigint unsigned NOT NULL,
  `weenie_Class_Id` int unsigned NOT NULL COMMENT 'Weenie Class Id of the Weenie this Biota was created from',
  `weenie_Type` int NOT NULL DEFAULT '0' COMMENT 'WeenieType for this Object',
  `populated_Collection_Flags` int unsigned NOT NULL DEFAULT '4294967295',
  PRIMARY KEY (`id`),
  KEY `biota_wcid_idx` (`weenie_Class_Id`),
  KEY `biota_type_idx` (`weenie_Type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Dynamic Weenies of a Shard/World';
/*!40101 SET character_set_client = @saved_cs_client */;

```

You can later run this script to restore to *any* database, not simply one with realms_shard. **However**, if you were to take this backup using the MySQL Workbench method [recommended by ACE](https://github.com/ACEmulator/ACE/wiki/Backing-Up-Your-Database), you would get the same script, but with this instead at the top of the script:

```
CREATE DATABASE  IF NOT EXISTS `realms_shard` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `realms_shard`;
-- MySQL dump 10.13  Distrib 8.0.34, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: realms_shard
```

If you were to run THAT script instead, on a new database, the script would ignore your option to select the new database, and use the original DB name! This is because of the ```use `realms_shard`;``` line, which changes the rest of the script to use that database for all commands. 

Always make sure you do not run a script without knowing for sure that it is not going to run on the wrong database. This has actually happened to people, which underscores the importance of learning the tools you are working with.

### Restoring backups

To restore a database backup from a dump script, after making sure that script doesn't have a hard-coded database name in it:
`mysql -uUSERNAME -pPASSWORD realms_shard < 2024-06-16-realms-shard.sql` - We're using mysql instead of mysqldump, and using < instead of >, as the direction of the arrow bracket points from the file to the database this time.  


## Migrating from ACE

ACRealms will automatically detect whether you are running a server against data that was previously for an ACE Server. When this data is detected, a migration mode will be temporarily enabled during startup, and all characters will be transferred to the default realm you specified in `Config.realms.js`. This will include the transfer of houses owned by the characters, and the items on the hooks and storage. The guest lists may not transfer, however, as of the time of writing this document (v2.1.1). 


