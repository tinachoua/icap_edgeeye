#include <cstdlib>
#include <async.h>
#include <adapters/libevent.h>
#include "Logger.hpp"
#include "RedisCommunicator.hpp"

using namespace std;

RedisCommu::~RedisCommu()
{
    if (rct_ != NULL)
		redisFree(rct_);
}

int RedisCommu::Connect()
{
	rct_ = redisConnect("172.30.0.5", 6379);
	if (rct_ != NULL && rct_->err) {
		LOG_WARN << "connection error:" << rct_->errstr;
		return -1;
	}

	redisReply* reply = (redisReply*)redisCommand(rct_, "select 2");
	LOG_DEBUG << "Redis:" << reply->str;
	freeReplyObject(reply);

	return 0;
}

redisReply* RedisCommu::Command(const string& command)
{
    redisReply* reply = (redisReply*)redisCommand(rct_, command.c_str());
	return reply;
}

void pubCallback(redisAsyncContext *c, void *r, void *privdata)
{
	(void)(privdata);
	redisReply *reply = (redisReply*)r;
	
	if (reply == NULL) {
    	LOG_ERROR << "Response not recev"; 
    	return;
  	}
  	
	LOG_DEBUG << "message published";
  	redisAsyncDisconnect(c);
}

void RedisCommu::Publish(const string& channel, const string& message)
{
	struct event_base* base = event_base_new();
	redisAsyncContext* _redisContext = redisAsyncConnect("172.30.0.5", 6379);

	if (_redisContext->err) {
		LOG_ERROR << "Redis publish error: "<< _redisContext->errstr;
    	return;
	}

	redisLibeventAttach(_redisContext,base);

	string command("publish ");
	command.append(channel);
	command.append(" ");
	command.append(message);

	redisAsyncCommand(_redisContext, pubCallback, (char*)"pub", command.c_str());
	LOG_DEBUG << "command:" << command;
	
	event_base_dispatch(base);
}