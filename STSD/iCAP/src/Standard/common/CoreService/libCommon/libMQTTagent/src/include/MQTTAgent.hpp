#ifndef LIBCOMMON_LIBMQTTAGENT_MQTTAGENT_H_
#define LIBCOMMON_LIBMQTTAGENT_MQTTAGENT_H_

#include <vector>
#include <string>
#include <utility>
#include <jsoncpp/json/json.h>
#include "MQTTAsync.h"

typedef std::pair<std::string, int> mqttsub_rule; // topic, Qos
typedef struct mqttconnect_option{
	const char* username;
	const char* password;
	std::vector<mqttsub_rule> sub_rule;
} Udefined_connect_option;
typedef int (*CommandAgent_SendToGateway)(const char*, const char*, int);
typedef void (*devicecmd_handler)(const std::string& topic, const std::string& message);

void MQTTAgent_AddSubTopics(Udefined_connect_option &opts, const std::vector<mqttsub_rule> &sub_rule);
int MQTTAgent_PublishData(const char* topic, const char* payload, int qos = 1);
int MQTTAgent_Start(Udefined_connect_option opts, const std::string& server_uri, devicecmd_handler temp);
int MQTTAgent_Stop();
int MQTTAgent_Destroy();

#endif // LIBCOMMON_LIBMQTTAGENT_MQTTAGENT_H_