
#include <fstream>
#include <jsoncpp/json/json.h>
#include "Configurer.hpp"
#include "Logger.hpp"
using namespace std;

Configurer::Configurer(std::string path)
{
    _path = path;
}

Configurer::~Configurer()
{
}

string Configurer::LoadSetting()
{   
    string setting;
    ifstream setting_file(_path, ifstream::binary);
    
    setting_file >> setting;

    return setting;
}

string Configurer::GetInnoAgeMqttBrokerUrl()
{
    string setting = LoadSetting();

    Json::Value root;
    Json::Reader reader;

    if (!reader.parse(setting, root)) {
        LOG_FATAL << "Failed to parse configuration file. File path:" << _path;
    } else {
        if (root["MQTT_broker_url"].isString())
            return root["MQTT_broker_url"].asString();
    }

    return {};
}

string Configurer::GetInnoAgeWebserviceUrl()
{
    string setting = LoadSetting();

    Json::Value root;
    Json::Reader reader;

    if (!reader.parse(setting, root)) {
        LOG_FATAL << "Failed to parse configuration file. File path:" << _path;
    } else {
        if (root["webservice_url"].isString())
            return root["webservice_url"].asString();
    }

    return {};
}