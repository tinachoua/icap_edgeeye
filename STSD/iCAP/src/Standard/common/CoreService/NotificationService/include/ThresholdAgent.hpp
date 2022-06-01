#ifndef NS_THRESHOLDAGENT_H_
#define NS_THRESHOLDAGENT_H_

#include <list>
#include <mutex>
#include <string>
#include <vector>

enum DataCase { NormalData, Storage, ParInfo }; // special case

struct ThresholdSetting
{
    struct DataTemplate
	{
		std::string name;
		std::list<std::string> qpath_list;
		std::string unit;
	};

    int func;
    std::vector<double> value_list;
    bool enable_mail;
    // bool enable_screenshot;
    DataTemplate Data;
    DataTemplate Denominator;
    std::vector<std::string> email_list;
    std::vector<std::string> dev_list;
};

class THAgent
{
public:
    THAgent();
    ~THAgent();
    void SetTHSetting(const std::string& setting_str);
    std::vector<ThresholdSetting> GetTHSetting();
    bool CheckDataTypeInTH(const ThresholdSetting& th, const std::string& data_type);
    bool CheckInnoAgeInTH(const ThresholdSetting& th, const std::string& sn, std::string& dev_name);
    int CheckDevContainInnoAge(const std::string& dev_name, const std::string& innoage_sn);
    bool CheckDevInTH(const ThresholdSetting& th, const std::string& dev);
    int CheckTHDataCase(const std::list<std::string>& qpath_list);

private:
    std::mutex th_mutex_;
    std::vector<ThresholdSetting> th_setting_;
    std::vector<ThresholdSetting> ParseTHSetting(const std::string& setting_str);
    int Getqpath(const std::string& db_path, std::list<std::string>& qpath_list);
    void UpdateTHSetting(const std::vector<ThresholdSetting>& ths);
};

#endif // NS_THRESHOLDAGENT_H_