#ifndef LIBCOMMON_LIBMONGODB_MONGODB_H_
#define LIBCOMMON_LIBMONGODB_MONGODB_H_

#include <mongocxx/instance.hpp>
#include <mongocxx/pool.hpp>
#include <mongocxx/stdx.hpp>

class MongoDB
{
 public:
  MongoDB();
  ~MongoDB();
  bool Initialize(std::unique_ptr<mongocxx::instance> instance,
    std::unique_ptr<mongocxx::pool> pool);
  mongocxx::pool::entry GetConn();
  mongocxx::stdx::optional<mongocxx::pool::entry> TryGetConnection(); //for multi-thread 

 private:
  std::unique_ptr<mongocxx::instance> _instance = nullptr;
  std::unique_ptr<mongocxx::pool> _pool = nullptr;
};

extern MongoDB MongoDatabase;

#endif // LIBCOMMON_LIBMONGODB_MONGODB_H_