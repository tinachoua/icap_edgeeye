#include <chrono>
#include <iostream>
#include <thread>
#include <jsoncpp/json/json.h>
#include "CommandAgent.hpp"
#include "Logger.hpp"
#include "MQTTAgent.hpp"
#include "RedisCommunicator.hpp"

using namespace std;

extern CommandAgent_SendToGateway CS;

void CommandAgent_SendCheckStatus(const string& devID)
{
	LOG_DEBUG << devID << ", Gateway send, Status command.";
	
	Json::Value root;
	root["ID"] = devID;
	root["Cmd"] = "status";

	Json::FastWriter writer;  
	string jstr = writer.write(root);

	if (CS != NULL)
	{
		CS("Command", jstr.c_str(), 1);
	}
}

void CommandAgent_SendOK(const string& devID)
{
	LOG_DEBUG << devID << ", Gateway send, OK command.";
	
	Json::Value root;
	root["ID"] = devID;
	root["Cmd"] = "Ok";
	
	Json::FastWriter writer;  
	string jstr = writer.write(root);

	if (CS != NULL)
	{
		CS("Command", jstr.c_str(), 1);
	}
}

void CommandAgent_SendFail(const string& devID)
{
	LOG_DEBUG << devID << ", Gateway send, Fail command.";
	Json::Value root;

	root["ID"] = devID;
	root["Cmd"] = "Fail";
	
	Json::FastWriter writer;  
	string jstr = writer.write(root);

	if (CS != NULL)
	{
		CS("Command", jstr.c_str(), 1);
	}
}

void CommandAgent_SendStart(const string& devID)
{
	LOG_DEBUG << devID << ", Gateway send, Start command.";

	Json::Value root;
	root["ID"] = devID;
	root["Cmd"] = "start";
	
	Json::FastWriter writer;  
	string jstr = writer.write(root);

	if (CS != NULL)
	{
		CS("Command", jstr.c_str(), 1);
	}
}

void CommandAgent_SendOffline(const string& devID)
{
	LOG_DEBUG << devID << ", Gateway send, Offline command.";
	
	Json::Value root;
	root["ID"] = devID;
	root["Cmd"] = "forceoffline";
	
	Json::FastWriter writer;  
	string jstr = writer.write(root);
	
	if (CS != NULL)
	{
		CS("Command", jstr.c_str(), 1);
	}
}

/*
 *	Publish device status to NotificationService through Redis channel
 */
void CommandAgent_PublishDevStatusToNS(const std::string& id, const int& status)
{
	LOG_INFO << id << ", Publish device status: " << status << " to NotificationService.";

	Json::Value msg;
	msg["DevType"] = "iCAPClient";
	msg["ID"] = id;
	msg["Status"] = status;
	
	Json::FastWriter fastWriter;
	RedisCommu redis_commu;
	redis_commu.Publish("/devstatus", fastWriter.write(msg));
}