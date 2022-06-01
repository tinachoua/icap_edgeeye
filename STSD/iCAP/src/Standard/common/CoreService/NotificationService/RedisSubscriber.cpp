#include <cstdlib>
#include <adapters/libevent.h>
#include <async.h>
#include <hiredis.h>
#include "EventHandler.hpp"
#include "Logger.hpp"
#include "RedisSubscriber.hpp"

using namespace std;

redisAsyncContext *c;
struct event_base *base = event_base_new();
const string channel_name = "/devstatus";

extern EventHandler event_handler;

void subCallback(redisAsyncContext *c, void *r, void *priv)
{
	(void)(c);
	redisReply *reply = (redisReply *)r;
    
	if (reply == NULL) return;
	
	if ((reply->type == REDIS_REPLY_ARRAY) && (reply->elements == 3)) {
		if (strcmp(reply->element[0]->str, "subscribe") != 0) {
			LOG_DEBUG << "Received[" << (char*)priv << "] channel " << reply->element[1]->str 
				<< ": " << reply->element[2]->str;
		
			if (channel_name.compare(reply->element[1]->str) == 0)
				event_handler.ProcessDevStatusEvent(reply->element[2]->str);
		}
	}
}

void connectCallback(const redisAsyncContext *c, int status)
{
	if (status != REDIS_OK) {
	    LOG_ERROR << "Connection error: " << c->errstr << endl;
        return;
    }
    LOG_INFO << "Redis connected...";
}

void disconnectCallback(const redisAsyncContext *c, int status)
{
	if (status != REDIS_OK) {
		LOG_ERROR << "Disconnection error: " << c->errstr << endl;
        return;
    }
	LOG_INFO << "Redis disconnected..." << endl;
}

void RedisSubscriber(void)
{ 	        
	c = redisAsyncConnect("172.30.0.5", 6379);
	redisLibeventAttach(c,base);
	redisAsyncSetConnectCallback(c,connectCallback);
	redisAsyncSetDisconnectCallback(c,disconnectCallback);
	string cmd = "SUBSCRIBE " + channel_name;
	redisAsyncCommand(c, subCallback, (char*)"sub", cmd.c_str());
	event_base_dispatch(base);
}

void RedisDisconnect()
{
	redisAsyncDisconnect(c);
}