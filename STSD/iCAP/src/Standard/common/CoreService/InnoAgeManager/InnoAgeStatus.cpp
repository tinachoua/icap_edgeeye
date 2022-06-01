#include <jsoncpp/json/json.h>
#include "Configurer.hpp"
#include "InnoAgeStatus.hpp"
#include "Logger.hpp"
#include "RedisCommunicator.hpp"
#include "WebCommand.hpp"

using namespace std;

void InitInnoAgeStatus()
{
	string innoage_list;
	if (WebCommand_GetInnoAgeList(innoage_list) == 0)
		WriteAllInnoAgeStatus(innoage_list);
	else
		LOG_ERROR << "Filed to get innoAge list.";
}

void WriteAllInnoAgeStatus(const string& innoage_list)
{
	if (innoage_list.empty())
		return;
	
	LOG_DEBUG << "innoage_list:" << innoage_list;
	
	Json::Value innoage_list_obj;
	Json::Reader reader;
	reader.parse(innoage_list, innoage_list_obj);

	for (auto innoage : innoage_list_obj["InnoAGElist"])
		WriteInnoAgeStatusFromWebAPI(innoage.asString());
}

void WriteInnoAgeStatusFromWebAPI(const string& sn)
{
	RedisCommu redis_commu;
	if (redis_commu.Connect() == 0) {
		int status;
		string cmd = "set " + sn + " ";
		
		Configurer config;
		string setting_url = config.GetInnoAgeWebserviceUrl();
		
		if (WebCommand_GetInnoAgeStatus(setting_url, sn, status) == 0)
			cmd += to_string(status);
		else
			cmd += "0";

		LOG_DEBUG << "Redis command:" << cmd;
		redisReply *reply = redis_commu.Command(cmd);

		freeReplyObject(reply);
	}
}

void WriteInnoAgeStatusFromMqtt(const string& sn, const string& status)
{
    RedisCommu redis_commu;
	if (redis_commu.Connect() == 0) {
        string cmd = "set " + sn + " " + status;
        LOG_DEBUG << "Redis command:" << cmd;
        redisReply *reply = redis_commu.Command(cmd);

        freeReplyObject(reply);
	}
}

void NotifyInnoAgeStatusToNS(const string& id, const string& status)
{
	Json::Value msg;
	msg["DevType"] = "innoAge";
	msg["ID"] = id;
	msg["Status"] = stoi(status);
	
	Json::FastWriter fastWriter;
	RedisCommu redis_commu;
	redis_commu.Publish("/devstatus", fastWriter.write(msg));
}