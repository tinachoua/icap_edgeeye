#ifndef LIBCOMMON_LIBMONGODB_NOSQLCOMMAND_H_
#define LIBCOMMON_LIBMONGODB_NOSQLCOMMAND_H_

#include <string>
#include <vector>

extern const char* mongodb_addr;
bool InitializeMongoDB(void);
bool CheckCollExists(const std::string &collection_name);
int GetDocCountInColl(const std::string &collection_name);
bool DropColl(const std::string &collection_name);
int InsertDataToCappedColl(const std::string& collection_name, const std::string& json_string);
int InsertDataToDB(const std::string& collection_name, const std::string& json_string);
int InsertAnalyzerDataToDB(const std::string& SN, const int& time, double health, int value);
int InsertScreenshotDataToDB(const std::string& dev_name, const int64_t& id, const std::string& path);
int QueryLatestDataFromDB(const std::string& collection_name, std::string& data);
int QueryDataFromDB(const std::string& collection_name, int direction, int limit, std::vector<std::string>& data);
int QueryAnalyzerDataFromDB(const std::string& SN, std::string& data);
int QueryOverThrsholdDataFromDB(const std::string& collection_name, const std::string& start_date, 
								const std::string& end_date, const std::string& data_path, double threshold_value);
int QueryLessThrsholdDataFromDB(const std::string& collection_name, const std::string& start_date,
								const std::string& end_date, const std::string& data_path, double threshold_value);
int QueryFakeDeviceStatusFromDB(const std::string& dev_name);
int QueryScreenshotDataFromDB(const std::string& dev_name, std::string& data);
int QueryLatestEventByDevNameFromDB(const std::string& dev_name, std::string& event);
int QueryLatestEventByMsgFromDB(const std::string& message, std::string& event);
int QueryEventDataWithinTimeFromDB(const std::string& dev_name, const std::string& message);
int DeleteAnalyzerDocFromDB(const std::string& SN);
int DeleteScreenshotDocFromDB(const std::string& dev_name);
int DeleteScreenshotDataFromDB(const std::string& dev_name, const int64_t& id);
int DeleteOldDataInDB(const int& expired_days);

#endif // LIBCOMMON_LIBMONGODB_NOSQLCOMMAND_H_