#include <ctime>
#include <future>
#include <list>
#include <mutex>
#include <jsoncpp/json/json.h>
#include "DeviceCommand.hpp"
#include "ImageHandler.hpp"
#include "Logger.hpp"
#include "main.hpp"
#include "Nosqlcommand.hpp"
#include "WebCommand.hpp"

using namespace std;

extern mutex PromiseListMutex;
extern map<string, promise<IMG_DATA_PAIR>*> PromiseList;

void UpdateDeviceAliasToWeb(const string& dev_name, const string& alias);

void MqttMessageReceiver(const std::string& topic, const std::string& message)
{
	BOOST_LOG_NAMED_SCOPE(SCOPE_NAME);
	Json::Value message_obj;
	Json::Reader reader;
	Json::FastWriter writer;

	if (!reader.parse(message, message_obj)) {
		LOG_WARN << "Get invalid command from Gateway, drop the command.";
		return;
	}

	if ((topic.compare("ActiveMsg") == 0) && message_obj.isObject() && message_obj.isMember("ID")) {
		UpdateDeviceAliasToWeb(message_obj["ID"].asString(), message_obj["Alias"].asString());
	} else if ((topic.compare("RawData") == 0) && message_obj.isObject() && message_obj.isMember("Dev")) {
		string dev_name, document;
		message_obj["time"] = static_cast<long long>(time(nullptr));
		
		if (message_obj.isMember("Sys")) {
			dev_name = message_obj["Dev"]["Name"].asString();
			LOG_DEBUG << dev_name << ", Gateway receive, Get static raw data.";
			document = writer.write(message_obj);
			LOG_DEBUG << document << ", String ready to insert to DataDB."; 
			InsertDataToDB(dev_name + "-static", document);

			if (GetDocCountInColl(dev_name + "-static") == 1) // new device
				UpdateDeviceAliasToWeb(dev_name, message_obj["Dev"]["Alias"].asString());
		} else {
			dev_name = message_obj["Dev"].asString();
			auto findit = PromiseList.find(dev_name);
			if (findit != PromiseList.end() && message_obj.isObject() && message_obj.isMember("Img")) {
				LOG_INFO << dev_name << ", Gateway receive, Get screenshot image.";			

				long long timestamp, id;
				if (SaveScreenshotImg(dev_name, message_obj["Img"].asString(), &timestamp, &id) == 0)
					AddImagePathToDB(dev_name, timestamp, id);
				
				PromiseListMutex.lock();
				findit->second->set_value(make_pair(message_obj["Img"].asString(), id));
				PromiseListMutex.unlock();
			} else {
				LOG_DEBUG << dev_name << ", Gateway receive, Get dynamic raw data.";
				document = writer.write(message_obj);
				LOG_DEBUG << document << ", String ready to insert to mongo db.";
				InsertDataToDB(dev_name + "-dynamic", document);
			}
		}
	} else {
		LOG_WARN << "Unrecognized command, drop the command.";
	}
}

void UpdateDeviceAliasToWeb(const string& dev_name, const string& alias)
{
	Json::Value alias_obj;
	alias_obj["Alias"] = alias;
	
	Json::FastWriter writer;  
	string alias_jsrt = writer.write(alias_obj);

	WebCommand_UpdateDeviceAlias(dev_name, alias_jsrt);
}