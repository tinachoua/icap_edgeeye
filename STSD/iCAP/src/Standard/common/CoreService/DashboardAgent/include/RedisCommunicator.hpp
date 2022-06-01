#ifndef DA_REDISCOMMUNICATOR_H_
#define DA_REDISCOMMUNICATOR_H_

#include <hiredis.h>

class RedisCommu
{
public:
    ~RedisCommu();
    int Connect();
    void Select(const int& db_index);
    redisReply* Command(const std::string& command);

private:
    redisContext* conn;
};

#endif // DA_REDISCOMMUNICATOR_H_