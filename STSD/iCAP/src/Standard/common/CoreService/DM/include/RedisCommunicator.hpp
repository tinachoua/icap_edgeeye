#ifndef DM_REDISCOMMUNICATOR_H_
#define DM_REDISCOMMUNICATOR_H_

#include <hiredis.h>

class RedisCommu
{
public:
    ~RedisCommu();
    int Connect();
    redisReply* Command(const std::string& command);
    void Publish(const std::string& channel, const std::string& message);

private:
    redisContext* rct_ = NULL;
};

#endif // DM_REDISCOMMUNICATOR_H_