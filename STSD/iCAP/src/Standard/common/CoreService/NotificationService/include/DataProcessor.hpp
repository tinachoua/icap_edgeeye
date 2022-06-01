#ifndef NS_DATAPROCESSOR_H_
#define NS_DATAPROCESSOR_H_

#include <string>
#include <map>
#include <boost/any.hpp>
#include "ThresholdAgent.hpp"

struct DevInfo
{
	std::string dev_name;
	std::map<std::string, int> storage_list; // first:S/N, second:partition count
};

struct DevData
{
    std::string dev_name; // device_name or storage S/N
    boost::any value;
    int time;
};

class DataProcessor
{
public:
    DataProcessor();
    ~DataProcessor();
    DevInfo GetDeviceInfo(const ThresholdSetting& th, const std::string dev_name);
    std::vector<DevData> GetData(const ThresholdSetting& th, const DevInfo& dev_info, const std::string& raw);
    bool CheckDevDataOverTH(const ThresholdSetting& th, const DevData& dev_data, std::string& message);

private:
    std::map<std::string, int> GetStorageList(const std::string& dev_name);
    void PrintQpath(const std::list<std::string>& qpath);
    void PrintData(const std::vector<DevData>& data);
    std::vector<DevData> GetDevDataFromDB(const DevInfo& dev_info, std::list<std::string> qpath_list, const std::string& raw);
    DevData SetDevData(const std::string& dev_name, const boost::any& value, const int& time);
    bool IsAnyNullType(const boost::any& value);
    double GetDoubleFromAny(const boost::any& value);
};

#endif // NS_DATAPROCESSOR_H_