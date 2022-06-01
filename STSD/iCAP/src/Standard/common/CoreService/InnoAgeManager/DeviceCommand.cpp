
#include <jsoncpp/json/json.h>
#include "DeviceCommand.hpp"
#include "Logger.hpp"
#include "main.hpp"
#include "InnoAgeStatus.hpp"

using namespace std;

void ProcessMqttMessage(const string& topic, const string& message);
vector<string> ParseTokenFromTopic(const string& topic);
int ParseStatus(const string message,  string& status);

void MqttMessageReceiver(const std::string& topic, const std::string& message)
{
	BOOST_LOG_NAMED_SCOPE(SCOPE_NAME);
    ProcessMqttMessage(topic, message);
}

void ProcessMqttMessage(const string& topic, const string& message)
{
    vector<string> token = ParseTokenFromTopic(topic);
    string innoage_sphere = token[0];
    string command = token[1];
    string sn = token[2];
    string status;
    
    if (ParseStatus(message, status) == 0) {
        WriteInnoAgeStatusFromMqtt(sn, status);
        NotifyInnoAgeStatusToNS(sn, status);
    }
}

vector<string> ParseTokenFromTopic(const string& topic)
{
    const string delimiter = "/";    
    size_t pos_start = 0, pos_end, delim_len = delimiter.length();
    string token;
    vector<string> result;

    while ((pos_end = topic.find(delimiter, pos_start)) != string::npos) {
        token = topic.substr(pos_start, pos_end - pos_start);
        pos_start = pos_end + delim_len;
        result.push_back(token);
    }
    result.push_back(topic.substr(pos_start));
    
    return result;
}

int ParseStatus(const string message, string& status)
{
    Json::Value message_obj;
    Json::Reader reader;

    if (!reader.parse(message, message_obj))
        return -1;

    status = message_obj["Status"].asString();

    return 0;
}