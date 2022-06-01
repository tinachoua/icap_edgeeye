FROM       ubuntu:16.04
WORKDIR /root
RUN apt-get update && \
    apt-get install -y git make gcc libssl-dev libcurl4-openssl-dev g++ automake autoconf libtool && \
    apt-get remove --purge -y git make gcc g++ automake autoconf && \
    apt-get autoremove -y
COPY iCAP_CoreService_CM /root/iCAP_CoreService_CM
ENTRYPOINT ["./iCAP_CoreService_CM"]
