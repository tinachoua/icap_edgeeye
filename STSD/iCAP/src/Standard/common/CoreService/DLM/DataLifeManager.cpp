#include <string>
#include <jsoncpp/json/json.h>
#include "DataLifeManager.hpp"
#include "Logger.hpp"
#include "main.hpp"
#include "Nosqlcommand.hpp"
#include "WebCommand.hpp"

using namespace std;

DLM::DLM()
{
    BOOST_LOG_NAMED_SCOPE(SCOPE_NAME);
    GetDBSetting();
}

DLM::~DLM()
{
}

void DLM::GetDBSetting()
{
    string setting;
    if (Webcommand_GetDBSetting(setting) == 0)
        UpdateDBSetting(setting);
}

void DLM::UpdateDBSetting(const string& setting)
{
    if (!setting.empty()) {
        Json::Value setting_obj;
        Json::Reader reader;

        reader.parse(setting, setting_obj);
        lock_guard<mutex> lock(setting_mutex_);
        if (setting_obj["ExpireDate"].asInt() == -1) {
            setting_.enable = false;
        } else {
            setting_.enable = true;
            setting_.days = setting_obj["ExpireDate"].asInt();
        } 
    }
}

void DLM::ParseDaysFromPayload(const string& payload)
{
    if (!payload.empty()) {
        Json::Value payload_obj;
        Json::Reader reader;

        reader.parse(payload, payload_obj);
        lock_guard<mutex> lock(setting_mutex_);
        setting_.enable = payload_obj["Enable"].asBool();
        setting_.days = payload_obj["Days"].asInt();
    }
}

void DLM::DeleteOldData()
{
    if (setting_.enable && setting_.days >= 30) 
        DeleteOldDataInDB(setting_.days);
}