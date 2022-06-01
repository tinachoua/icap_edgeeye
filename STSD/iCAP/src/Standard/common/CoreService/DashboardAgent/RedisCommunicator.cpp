#include "Logger.hpp"
#include "RedisCommunicator.hpp"

using namespace std;

RedisCommu::~RedisCommu()
{
    redisFree(conn);
}

int RedisCommu::Connect()
{
	conn = redisConnect("172.30.0.5", 6379);
	if (conn != NULL && conn->err) {
		LOG_WARN << "connection error:" << conn->errstr;
		return -1;
	}

	return 0;
}

void RedisCommu::Select(const int& db_index)
{
	string cmd = "select " + to_string(db_index);
	redisReply* reply = (redisReply*)redisCommand(conn, cmd.c_str());
	LOG_DEBUG << "Redis:" << reply->str;
	freeReplyObject(reply);
}

redisReply* RedisCommu::Command(const string& command)
{
    redisReply* reply = (redisReply*)redisCommand(conn, command.c_str());
	return reply;
}