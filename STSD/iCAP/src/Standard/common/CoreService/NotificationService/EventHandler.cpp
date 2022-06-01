#include <jsoncpp/json/json.h>
#include "DataProcessor.hpp"
#include "EventHandler.hpp"
#include "Logger.hpp"
#include "main.hpp"
#include "Nosqlcommand.hpp"
#include "WebCommand.hpp"

using namespace std;
const string DevStatus { "Device status" };
const string InnoAgeStatus { "InnoAGE status" };

EventHandler::EventHandler()
{
	BOOST_LOG_NAMED_SCOPE(SCOPE_NAME);
	GetTHSetting();
	GetMailSendor();
}

EventHandler::~EventHandler()
{
}

int EventHandler::GetTHSetting()
{
	string setting_str;
	
	if (Webcommand_GetTHSetting(setting_str) != 0) {
		LOG_ERROR << "Failed to get threshold setting.";
		return -1;
	}

	th_agent_.SetTHSetting(setting_str);

	return 0;
}

int EventHandler::GetMailSendor()
{
	string mailsendor_str;
	
	if (Webcommand_GetEmailSendor(1, mailsendor_str) != 0) {
		LOG_ERROR << "Failed to get mail sendor setting.";
		return -1;
	}

	mail_handler_.SetMailSendor(mailsendor_str);

    return 0;
}

void EventHandler::ProcessDevStatusEvent(const string& msg)
{
	bool is_innoage = false;
	string id;
	int dev_status;
	
	Json::Value msg_obj;
	Json::Reader reader;
	
	if (!reader.parse(msg, msg_obj))
		return;

	if (msg_obj["DevType"].asString() == "innoAge")
		is_innoage = true;
	
	id = msg_obj["ID"].asString();
	
	if (msg_obj["Status"].asInt() == 1)
		dev_status = 1;
	else 
		dev_status = 0;
	
	auto th_settings = th_agent_.GetTHSetting();

	for (const auto& th : th_settings) {
		if (th_agent_.CheckDataTypeInTH(th, DevStatus) || th_agent_.CheckDataTypeInTH(th, InnoAgeStatus)) {
			if (dev_status != th.value_list[0])
				continue;
			
			string message;
			bool notify = false;
			string dev_name;
			if (is_innoage && th_agent_.CheckDataTypeInTH(th, InnoAgeStatus)) {
				if (th_agent_.CheckInnoAgeInTH(th, id, dev_name)) {
					if (dev_status == 1)
						message = dev_name + " " + InnoAgeStatus + " is online, value: 1 .";
					else if (dev_status == 0)
						message = dev_name + " " + InnoAgeStatus + " is offline, value: 0 .";
					
					notify = true;
				}
			} else if (!is_innoage && th_agent_.CheckDataTypeInTH(th, DevStatus)) {
				dev_name = id;
				if (th_agent_.CheckDevInTH(th, id)) {
					if (dev_status == 1)
						message = id + " " + DevStatus + " is online, value: 1 .";
					else if (dev_status == 0)
						message = id + " " + DevStatus + " is offline, value: 0 .";
					
					notify = true;
				}
			}

			if (notify) {
				if (IsNewEvent(dev_name, message))
					CreateEvent(dev_name, message);
						
				if (th.enable_mail && mail_handler_.CheckResend(message))
					mail_handler_.SendEmail(th.email_list, message);
			}
		}
	}
}

int EventHandler::IsNewEvent(const string& dev_name, const string& message)
{
	if (QueryEventDataWithinTimeFromDB(dev_name, message)) {
		LOG_DEBUG << "Event already exist.";
		return 0;
	}

	LOG_INFO << "New event happened. Message:" << message;
	return 1;
}

void EventHandler::CreateEvent(const string& dev_name, const string& message)
{	
	Json::Value log_obj;

	log_obj["Dev"] = dev_name;
	log_obj["Message"] = message;
	log_obj["Checked"] = false;
	log_obj["Time"] = static_cast<int>(time(nullptr));
	
	InsertDataToDB("EventLog", log_obj.toStyledString());
}

void EventHandler::ProcessRawEvent(const string& dev_name, const string& raw)
{
	DataProcessor data_proc;
	auto th_settings = th_agent_.GetTHSetting();

	for (const auto& th : th_settings) {
		if (!th_agent_.CheckDataTypeInTH(th, DevStatus) && th_agent_.CheckDevInTH(th, dev_name)) {
			DevInfo dev = data_proc.GetDeviceInfo(th, dev_name);
			vector<DevData> data = data_proc.GetData(th, dev, raw);
			for (const auto d : data) {
				string message;
				if (data_proc.CheckDevDataOverTH(th, d, message)) {
					if (IsNewEvent(dev_name, message))
						CreateEvent(dev_name, message);
						
					if (th.enable_mail && mail_handler_.CheckResend(message))
						mail_handler_.SendEmail(th.email_list, message);
				}
			}
		}
	}
}