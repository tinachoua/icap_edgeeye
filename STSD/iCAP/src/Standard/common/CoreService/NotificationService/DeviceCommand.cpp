#include <jsoncpp/json/json.h>
#include "DeviceCommand.hpp"
#include "EventHandler.hpp"
#include "Logger.hpp"
#include "main.hpp"

using namespace std;

EventHandler event_handler;

void ProcessMqttMessage(const string& topic, const string& message);

void MqttMessageReceiver(const std::string& topic, const std::string& message)
{
	BOOST_LOG_NAMED_SCOPE(SCOPE_NAME);
	ProcessMqttMessage(topic, message);
}

void ProcessMqttMessage(const string& topic, const string& message)
{
	(void)topic;
	Json::Value msg_obj;
	Json::Reader reader;

	if (!reader.parse(message, msg_obj)) {
		LOG_ERROR << "Get invalid command from Gateway, drop the command.";
		return;
	}

	if (msg_obj.isObject() && msg_obj.isMember("Dev") && !msg_obj.isMember("Sys")) { // static raw data include "Sys"
		LOG_DEBUG << msg_obj["Dev"].asString() << ", gateway received dynamic raw data.";
		event_handler.ProcessRawEvent(msg_obj["Dev"].asString(), msg_obj.toStyledString());
	}
}