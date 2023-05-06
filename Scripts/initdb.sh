#!/usr/bin/env bash

domains="auth shard world";
declare -A db_base

db_base["auth"]="/ace/Database/Base/AuthenticationBase.sql"
db_base["shard"]="/ace/Database/Base/ShardBase.sql"
db_base["world"]="/ace/Database/Base/WorldBase.sql"

echo "** Creating default DB and users"
for domain in $domains; do
    mysql -u root -p$MYSQL_ROOT_PASSWORD --execute \
    "CREATE DATABASE IF NOT EXISTS ace_$domain;
    USE ace_$domain;
    source ${db_base[$domain]};
    CREATE USER ace_$domain@ IDENTIFIED BY '$MYSQL_ROOT_PASSWORD';
    GRANT ALL PRIVILEGES ON ace_$domain.* TO ace_$domain@;
    FLUSH PRIVILEGES;"
done
echo "** Finished creating default DB and users"
