FROM ubuntu:latest

ENV DEBIAN_FRONTEND=noninteractive

WORKDIR /root

# apt
RUN apt-get update && apt-get install -y --no-install-recommends \
    cmake \
    build-essential \
    ca-certificates \
    curl \
    git \
    tar \
    wget \
    netcat \
    libc6-dev-i386  \
    gcc-multilib \
    g++-multilib \
    libboost-all-dev \
    libssl-dev \
    libsasl2-dev \
    libjsoncpp-dev \
    libcurl4-openssl-dev \
    libevent-dev \
    --fix-missing

# Mongo C Driver
RUN wget https://github.com/mongodb/mongo-c-driver/releases/download/1.17.2/mongo-c-driver-1.17.2.tar.gz && \
    tar xzf mongo-c-driver-1.17.2.tar.gz && \
    cd mongo-c-driver-1.17.2 && \
    mkdir cmake-build && \
    cd cmake-build && \
    cmake -DENABLE_AUTOMATIC_INIT_AND_CLEANUP=OFF .. && \
    cmake --build . && \
    cmake --build . --target install && \
    cd ../../ && \
    rm -r mongo-c-driver-1.17.2 && \
    rm mongo-c-driver-1.17.2.tar.gz

# Mongo CXX Driver
RUN curl -OL https://github.com/mongodb/mongo-cxx-driver/releases/download/r3.6.1/mongo-cxx-driver-r3.6.1.tar.gz && \
    tar -xzf mongo-cxx-driver-r3.6.1.tar.gz && \
    cd mongo-cxx-driver-r3.6.1/build && \
    cmake .. \
    -DCMAKE_BUILD_TYPE=Release \
    -DCMAKE_INSTALL_PREFIX=/usr/local && \
    cmake --build . && \
    cmake --build . --target install && \
    cd ../../ && \
    rm -r mongo-cxx-driver-r3.6.1 && \
    rm mongo-cxx-driver-r3.6.1.tar.gz

# Paho MQTT C Client Library
RUN git clone https://github.com/eclipse/paho.mqtt.c.git && \
    cd paho.mqtt.c && \
    make && \
    make install && \
    cd .. && \
    rm -r paho.mqtt.c

# cpp_redis: Asynchronous Multi-Platform Lightweight Redis Client for C++11
RUN git clone https://github.com/cpp-redis/cpp_redis.git && \
    cd cpp_redis && \
    git submodule init && git submodule update && \
    mkdir build && cd build && \
    cmake .. -DCMAKE_BUILD_TYPE=Release && \
    make && \
    make install && \
    cd ../.. && \
    rm -rf cpp_redis

# curlpp: cURLpp is a C++ wrapper for libcURL
RUN git clone https://github.com/jpbarrette/curlpp.git && \
    cd curlpp && \
    cmake . && \
    make install && \
    cd .. && \
    rm -r curlpp && \
    ldconfig

# Delete cached files we don't need anymore:
RUN apt-get clean && \
    rm -rf /var/lib/apt/lists/*