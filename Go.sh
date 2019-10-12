#!/bin/bash
rm -r docker/output/*

dotnet publish gettingstarted/ -c release -o ../docker/output

cp docker/dockerfile docker/output/dockerfile

docker build docker -t consumer:v1

docker run -it consumer:v1

cd docker

docker-compose up -d

docker logs consumer -f
