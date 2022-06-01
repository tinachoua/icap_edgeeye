# Web Service Dockerfile
From node

ADD . /app/OOBService
ENV PORT=8165
ENV ICAP_AUTH_SERVICE=172.30.0.6:50000
ENV OOB_HOST=172.30.0.100:8161
ENV OOB_WEBSOCKET_HOST=ws://172.30.0.100:8161/ws/innoage
ENV REDIS_HOST=172.30.0.5
ENV DEBUG_MODE=fales

WORKDIR /app/OOBService

RUN npm install

ENTRYPOINT ["npm","run"]
CMD ["start-ts"]

