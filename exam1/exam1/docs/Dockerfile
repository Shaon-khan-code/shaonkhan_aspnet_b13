FROM ubuntu

ARG DEBIAN_FRONTEND=noninteractive
RUN apt update
RUN apt install apache2 -y

EXPOSE 80

#CMD apachectl -D FOREGROUND