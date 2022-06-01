#ifndef DLM_DATALIFEMANAGER_H_
#define DLM_DATALIFEMANAGER_H_

#include <mutex>

struct DBSetting
{
    bool enable = false;
    unsigned days;
};

class DLM
{
public:
    DLM();
    ~DLM();
    void ParseDaysFromPayload(const std::string& payload);
    void DeleteOldData();

private:
    std::mutex setting_mutex_;
    DBSetting setting_;
    void GetDBSetting();
    void UpdateDBSetting(const std::string& setting);
};

#endif // DLM_DATALIFEMANAGER_H_