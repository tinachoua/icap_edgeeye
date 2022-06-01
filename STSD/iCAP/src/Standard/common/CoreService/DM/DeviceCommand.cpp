#include <algorithm>    // std::find
#include <fstream>
#include <iostream>
#include <list>
#include <jsoncpp/json/json.h>
#include "CheckAlive.hpp"
#include "CheckStatus.hpp"
#include "CommandAgent.hpp"
#include "DeviceCommand.hpp"
#include "Logger.hpp"
#include "main.hpp"
#include "Nosqlcommand.hpp"
#include "WebCommand.hpp"

using namespace std;

#define UNUSED(x) (void)(x)

int GetFakeDevice()
{
	ifstream in_file("/var/iCAP/DeviceCount.json");
	
	if (in_file.is_open()) {
		Json::Value file_obj;
		in_file >> file_obj;
		in_file.close();

		if (file_obj.isObject() && file_obj.isMember("Total")) {
			int dev_count = file_obj["Total"].asInt();
			LOG_DEBUG << "fake device count:" << dev_count;
			return  dev_count;
		}
	}

	LOG_WARN << "Failed to load DeviceCount.json. Use default device count:100.";
	return 100;
}

void DeviceCommand_InitialCheck()
{
	string dev_list;
	if (WebCommand_GetDeviceList(dev_list) != 0) {
		LOG_ERROR << "Failed to get device list.";
		return;
	}

	if (dev_list.empty()) return;

	Json::Value dev_list_obj;
	Json::Reader reader;
	if (!reader.parse(dev_list, dev_list_obj)) {
		LOG_ERROR << "Failed to parse device list." << reader.getFormattedErrorMessages();
		return;
	}
	
	if (!dev_list_obj.isObject() || !dev_list_obj.isMember("DeviceList")) {
		LOG_ERROR << "The content of the device list is wrong.";
		return;
	}
	
	dev_list_obj = dev_list_obj["DeviceList"];

	int all_dev_count = dev_list_obj.size();
	int fake_dev_count = GetFakeDevice();

	for (int i = 0; i < all_dev_count; i++) {
		string dev_name = dev_list_obj[i].asString();
		string tmp_str = dev_name;
		tmp_str.erase(0, 6); // erase "Device"
		
#ifdef FAKE_DATA
		int dev_id = stoi(tmp_str);
		if (dev_id > fake_dev_count) {
			LOG_INFO << "Start check device " << dev_name << " status.";
			Status_insert_new_data(dev_name);
		} else {
			bool is_offline = QueryFakeDeviceStatusFromDB(dev_name);
			int response;
			if (is_offline)
				WebCommand_UpdateDeviceStatus(dev_name, 0, response);
			else
				WebCommand_UpdateDeviceStatus(dev_name, 1, response);
		}
#else
		(void)(fake_dev_count);
		LOG_INFO << "Start check device " << dev_name << " status.";
		Status_insert_new_data(dev_name);
#endif
	}
}

void MqttMessageReceiver(const std::string& topic, const std::string& message)
{
	BOOST_LOG_NAMED_SCOPE(SCOPE_NAME);
	(void)topic;
	Json::Value root;
	Json::Reader reader;

	if (!reader.parse(message, root)) {
		LOG_WARN << "Get invalid command from Gateway, drop the command.";
		return;
	}
	
	string id;
	if (root.isObject() && root.isMember("ID")) {
		id = root["ID"].asString();
	} else {
		LOG_WARN << "No device ID in command, drop the command.";
		return;
	}
	
	if (root.isObject() && root.isMember("Cmd")) {
		int device_status = 0;
		Json::Value Cmd = root["Cmd"];

		if (Cmd.asString().compare("reg") == 0) {
			LOG_INFO << id << ", Gateway receive, Register command.";
			if (WebCommand_GetDeviceStatus(id, device_status) != 0) {
				LOG_ERROR << "Get device status failed. Device:" << id;
				return;
			}

			LOG_DEBUG << "Get device status response: " << device_status;

			switch (device_status) {
				case 0:
					LOG_DEBUG << id << ", DeviceAPI response, Exist and offline.";
					break;
				case 1:
					LOG_DEBUG << id << ", DeviceAPI response, Exist and online.";
					break;

				case 2:
					LOG_DEBUG << id << ", DeviceAPI response, Not exist.";
					break;
			}

			if (device_status == 0) {
				CommandAgent_SendOK(id);

				if (Status_find_data(id) == 0) {
					LOG_DEBUG << "Device " << id << " not in list, create new node";
					Alive_insert_new_data(&id[0]);
				}
			} else {
				CommandAgent_SendFail(id);
			}

			return;
		}

		if (Cmd.asString().compare("busy") == 0) {
			LOG_DEBUG << id << ", Gateway receive, Device, Response busy.";
			int response;
			if (WebCommand_UpdateDeviceStatus(id, 1, response) != 0) {
				LOG_WARN << id << ", WebSocket response, Device not found.";
				CommandAgent_SendFail(id);
				return;
			}

			if (response == 1) {
				LOG_DEBUG << id << ", WebSocket response, Set online successfuliy.";
				Status_update_data_status(id);
				
				if (Alive_find_data(id) == 0)
					Alive_insert_new_data(&id[0]);
				
				CommandAgent_PublishDevStatusToNS(id, 1);
			}

			return;
		}

		if (Cmd.asString().compare("online") == 0) {
			LOG_DEBUG << id << ", Gateway receive, Device response online.";
			int response;
			if (WebCommand_UpdateDeviceStatus(id, 1, response) != 0) {
				LOG_WARN << id << ", WebSocket response, Device not found.";
				CommandAgent_SendFail(id);
				return;
			}
			
			if (response == 1) {
				LOG_DEBUG << id << ", WebSocket response, Set online successfully.";
				Status_update_data_status(id);
				CommandAgent_SendStart(id);
				
				if (Alive_find_data(id) == 0)
					Alive_insert_new_data(&id[0]);
				
				CommandAgent_PublishDevStatusToNS(id, 1);
			}
			
			return;
		}

		if (Cmd.asString().compare("offline") == 0) {
			LOG_DEBUG << id << ", Gateway receive, Device offline command.";
			int response;
			if (WebCommand_UpdateDeviceStatus(id, 0, response) != 0) {
				LOG_WARN << id << ", WebSocket response, Device not found.";
				CommandAgent_SendFail(id);
				return;
			}
			
			if (response == 0) {
				LOG_DEBUG << id << ", WebSocket response, Set offline successfully.";
				CommandAgent_PublishDevStatusToNS(id, 0);
				Alive_remove_data(id);
				CommandAgent_SendOK(id);
			}
			
			return;
		}

		if (Cmd.asString().compare("ok") == 0) {
			LOG_DEBUG << "[Gateway:Recevice] Device " << id << " ok command.";
			Alive_remove_data(id);
			return;
		}
	}
}