The scripts in this folder will drop any tables that exist before creating new ones. Make sure you run them for the correct database as they do not have "use" statements

Example: 
mysql -uroot -proot realms_auth < AuthenticationBase.sql
mysql -uroot -proot realms_shard < ShardBase.sql
mysql -uroot -proot realms_world < WorldBase.sql
