#!/usr/bin/env bash

rm -rf db-data/ && docker compose down -v && docker compose up ace-db
