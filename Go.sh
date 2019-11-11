#!/bin/bash
rm -r Docker/output/*

dotnet publish gettingstarted/GettingStarted/ -c release -o Docker/output

#cp docker/dockerfile docker/output/dockerfile

# docker build docker -t consumer:v1

#docker run -it consumer:v1

cd docker

docker-compose up -d

docker logs consumer

#docker-compose logs  -f
