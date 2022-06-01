#include <iostream>
#include <jsoncpp/json/json.h>
#include "Analyzer.hpp"
#include "DeviceCommand.hpp"
#include "Logger.hpp"
#include "main.hpp"
#include "Nosqlcommand.hpp"
#include "WebCommand.hpp"

using namespace std;

void DeviceCommand_InitialCheck(void)
{
	string responseStr;
	if (WebCommand_GetDeviceList(responseStr) != 0)
	{
		LOG_ERROR << "Failed to get device list.";
		return;
	}
	if (responseStr.empty())
		return;

	Json::Value root, devlist_obj;
	Json::Reader reader;
	if (!reader.parse(responseStr, root)) 
	{
		LOG_ERROR << "Failed to parse responseStr." << reader.getFormattedErrorMessages();
		exit(-1);
	}
	
	if (root.isObject() && root.isMember("DeviceList"))
	{
		devlist_obj = root["DeviceList"];
	}
	else
	{
		LOG_ERROR << "The format of the responseStr is incorrect.";
		return;
	}

	for (Json::Value::ArrayIndex i = 0; i < devlist_obj.size(); i++)
	{
		Json::Value dev_obj;
		string dev_name;

		dev_obj = devlist_obj[i];
		
		if (dev_obj.isString())
		{
			dev_name = dev_obj.asString();
			if (dev_name.empty())
			{
				break;
			}
		}
		else
			break;

		LOG_INFO << "[Initial check]Get device name " << dev_name;
		
		string CollectionStr = dev_name + "-static";
		string DBResponse;
		int result = QueryLatestDataFromDB(CollectionStr, DBResponse);
		if ((result != 1) || DBResponse.empty())
			continue;

		LOG_DEBUG << "Get static raw data:" << DBResponse;
		Analyzer_GetStaticRawData(DBResponse);
	}
}

void MqttMessageReceiver(const std::string& topic, const std::string& message)
{
	BOOST_LOG_NAMED_SCOPE(SCOPE_NAME);
	(void)topic;
	Json::Value root;
	Json::Reader reader;
	Json::FastWriter writer;

	if (!reader.parse(message, root))
	{
		LOG_WARN << "Get invalid command from Gateway, drop the command.";
		return;
	}
	
	string dev_name;
	if (!(root.isObject() && root.isMember("Dev"))) {
		LOG_WARN << "No device name in command, drop the command.";
		return;
	}

	if (root.isObject() && root.isMember("Sys")) {
		dev_name = root["Dev"]["Name"].asString();
		LOG_DEBUG << dev_name << ", Gateway receive, Get static raw data.";
		string jstr = writer.write(root);
		Analyzer_GetStaticRawData(jstr);	
	} else {
		dev_name = root["Dev"].asString();
		LOG_DEBUG << dev_name << ", Gateway receive, Get dynamic raw data!!";
		string jstr = writer.write(root);
		Analyzer_GetDynamicRawData(jstr);
	}
}