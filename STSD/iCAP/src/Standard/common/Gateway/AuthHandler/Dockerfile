FROM ubuntu:latest
WORKDIR /root
RUN apt-get update && apt-get install -y libjson-c-dev libevent-dev mosquitto
COPY mosquitto.conf /etc/mosquitto/conf.d/mosquitto.conf
COPY iCAP_AuthHandler /root/iCAP_AuthHandler
#EXPOSE 1883
COPY docker-entrypoint.sh /usr/local/bin/
RUN chmod a+x /usr/local/bin/docker-entrypoint.sh
ENTRYPOINT ["docker-entrypoint.sh"]
CMD ["/usr/sbin/mosquitto"]