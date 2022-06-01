#include <iostream>
#include "Mongodb.hpp"

MongoDB MongoDatabase;

MongoDB::MongoDB()
{
}

MongoDB::~MongoDB()
{
}

bool MongoDB::Initialize(std::unique_ptr<mongocxx::instance> instance,
    std::unique_ptr<mongocxx::pool> pool)
{
    _instance = std::move(instance);
    _pool = std::move(pool);

    return true;
}

mongocxx::pool::entry MongoDB::GetConn()
{
    return _pool->acquire();
}

mongocxx::stdx::optional<mongocxx::pool::entry> MongoDB::TryGetConnection()
{
    return _pool->try_acquire();
}