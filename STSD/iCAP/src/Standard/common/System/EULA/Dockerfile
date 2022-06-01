FROM alpine

WORKDIR /root

RUN echo http://nl.alpinelinux.org/alpine/edge/testing >> /etc/apk/repositories
RUN apk add --no-cache w3m
COPY EULA.html /root/EULA.html

ENTRYPOINT ["/usr/bin/w3m", "EULA.html"]
