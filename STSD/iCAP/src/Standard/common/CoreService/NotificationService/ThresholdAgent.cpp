#include <algorithm>
// #include <thread>
#include <jsoncpp/json/json.h>
#include "Logger.hpp"
#include "Nosqlcommand.hpp"
#include "ThresholdAgent.hpp"

using namespace std;

// vector<ThresholdSetting> THSettings;

/*
Generic function to find an element in vector.
It returns a bool.
bool : Represents if element is present in vector or not.
*/
template < typename T>
bool FindInVector(const vector<T>  & vecOfElements, const T  & element)
{
	bool result;
 
	// Find given element in vector
	auto it = find(vecOfElements.begin(), vecOfElements.end(), element);
 
	if (it != vecOfElements.end())
		result = true;
	else
		result = false;
 
	return result;
}

THAgent::THAgent()
{
}

THAgent::~THAgent()
{
}

void THAgent::SetTHSetting(const string& setting_str)
{
	vector<ThresholdSetting> th_settings;
	
	if (setting_str.empty())
		LOG_DEBUG << "No threshold setting.";
	else
		th_settings = ParseTHSetting(setting_str);

	UpdateTHSetting(th_settings);
}

vector<ThresholdSetting> THAgent::ParseTHSetting(const string& setting_str)
{
	Json::Value setting_obj;
	Json::Reader reader;
	if (!reader.parse(setting_str, setting_obj)) {
		LOG_ERROR << "Failed to parse threshold setting." << endl << reader.getFormattedErrorMessages();
		return {};
	}

	vector<ThresholdSetting> ret;
	for (decltype(setting_obj.size()) th_index = 0; th_index < setting_obj.size(); th_index++) {
		ThresholdSetting th_obj;

		th_obj.func = setting_obj[th_index]["Func"].asInt();
		for (decltype(setting_obj[th_index]["Value"].size()) vl_index = 0; 
			vl_index < setting_obj[th_index]["Value"].size(); vl_index++)
			th_obj.value_list.push_back(setting_obj[th_index]["Value"][vl_index].asDouble());
		th_obj.enable_mail = setting_obj[th_index]["EnableEmail"].asBool();
		// th_obj.enable_screenshot = setting_obj[th_index]["EnableScreenshot"].asBool();
		
		th_obj.Data.name = setting_obj[th_index]["Data"]["Name"].asString();
		list<string> qpath_list;
		Getqpath(setting_obj[th_index]["Data"]["Path"].asString(), qpath_list);
		th_obj.Data.qpath_list.assign(qpath_list.begin(), qpath_list.end());
		th_obj.Data.unit = setting_obj[th_index]["Data"]["Unit"].asString();
		
		qpath_list.clear();
		th_obj.Denominator.name = setting_obj[th_index]["Denominator"]["Name"].asString();
		Getqpath(setting_obj[th_index]["Denominator"]["Path"].asString(), qpath_list);
		th_obj.Denominator.qpath_list.assign(qpath_list.begin(), qpath_list.end());
		th_obj.Denominator.unit = setting_obj[th_index]["Denominator"]["Unit"].asString();

		for (decltype(setting_obj[th_index]["EmailList"].size()) ml_index = 0;
			ml_index < setting_obj[th_index]["EmailList"].size(); ml_index++)
			th_obj.email_list.push_back(setting_obj[th_index]["EmailList"][ml_index].asString());

		for (decltype(setting_obj[th_index]["DevNameList"].size()) dl_index = 0;
			dl_index < setting_obj[th_index]["DevNameList"].size(); dl_index++)
			th_obj.dev_list.push_back(setting_obj[th_index]["DevNameList"][dl_index].asString());
		
		ret.push_back(th_obj);
	}

#ifdef DEBUG
	LOG_TRACE << "[Threshold Setting]";
	for (auto it = ret.begin(); it != ret.end(); ++it) {
		cout << static_cast<int>(distance(ret.begin(), it)) << endl;
		cout << "  Fun:" << it->func << endl;
		cout << "  Value" << endl;
		for (auto vit = it->value_list.begin(); vit != it->value_list.end(); ++vit)
		{
			cout << "  +--" << (int)distance(it->value_list.begin(), vit) << ":" << *vit << endl;
		}
		cout << "  EnableMail:" << it->enable_mail << endl;
		// cout << "  EnableScreenshot:" << it->enable_screenshot << endl;
		cout << "  Data" << endl;
		cout << "  +--Name:" << it->Data.name << endl;
		cout << "  +--Path:";
		for (auto qit = it->Data.qpath_list.begin(); qit != it->Data.qpath_list.end(); ++qit)
			cout << *qit << ' ';
		cout << endl;
		cout << "  +--Unit:" << it->Data.unit << endl;
		cout << "  Denominator" << endl;
		cout << "  +--Name:" << it->Denominator.name << endl;
		cout << "  +--Path:";
		for (auto qit = it->Denominator.qpath_list.begin(); qit != it->Denominator.qpath_list.end(); ++qit)
			cout << *qit << ' ';
		cout << endl;
		cout << "  +--Unit:" << it->Denominator.unit << endl;
		cout << "  EmailList" << endl;
		for (auto eit = it->email_list.begin(); eit != it->email_list.end(); ++eit)
			cout << "  +--" << (int)distance(it->email_list.begin(), eit) << ":" << *eit << endl;
		cout << "  DevNameList" << endl;
		for (auto dit = it->dev_list.begin(); dit != it->dev_list.end(); ++dit)
			cout << "  +--" << (int)distance(it->dev_list.begin(), dit) << ":" << *dit << endl;
	}
#endif

	return ret;
}

int THAgent::Getqpath(const string& db_path, list<string>& qpath_list) //(Mongodb collection, documents path, number of data you want to get)
{
	if (db_path.empty())
		return 0;

	Json::Value root;
	Json::Reader reader;

	bool parsingSuccessful = reader.parse(db_path, root);
	if (!parsingSuccessful) {
		LOG_ERROR << "Failed to parse configuration." << reader.getFormattedErrorMessages();
    	return -1;
	}
	
	Json::Value::Members members(root.getMemberNames());
	const string key = members.front();

	qpath_list.clear();
	qpath_list.push_back(key);

	string keyTitle = key;
	
	while (root[keyTitle].type() != Json::nullValue) {
		switch (root[keyTitle].type()) { 
			case Json::nullValue:
				return 0;
    			break;
			case Json::intValue:
				return 0;
    			break;
			case Json::arrayValue: {
				root = root[keyTitle][0];//Get first object{:}
                Json::Value::Members members(root.getMemberNames());
				const string name = members.front();
				qpath_list.push_back(name);
                keyTitle = name;
			}
				break;
			case Json::objectValue: {
				root = root[keyTitle];
				Json::Value::Members members(root.getMemberNames());
				const string name = members.front();

            	if (name == "Enable") {
					// auto keys_front = keys.begin();
					// std::advance(keys_front,2);
            		// qpath_list.push_back(*keys_front);//list "keys" second value
                    // keyTitle = *keys_front;
            	} else {
					qpath_list.push_back(name);
                    keyTitle = name;
            	}
			}
			break;
			default:
    			break; 
		} 
	}
	
	return 0;
}

void THAgent::UpdateTHSetting(const vector<ThresholdSetting>& ths)
{
	lock_guard<mutex> lock(th_mutex_);
	th_setting_.clear();
	if (!ths.empty())
		th_setting_.assign(ths.begin(), ths.end());
}

vector<ThresholdSetting> THAgent::GetTHSetting()
{
	lock_guard<mutex> lock(th_mutex_);
	return th_setting_;
}

bool THAgent::CheckDataTypeInTH(const ThresholdSetting& th, const string& data_type)
{
	if (th.Data.name.compare(data_type) == 0)
		return true;
	
	return false;
}

bool THAgent::CheckInnoAgeInTH(const ThresholdSetting& th, const string& innoage_sn, string& dev_name)
{
	for (auto dev : th.dev_list) {
		if (CheckDevContainInnoAge(dev, innoage_sn)) {
			dev_name = dev;
			return true;
		}
	}
	
	return false;
}

int THAgent::CheckDevContainInnoAge(const string& dev_name, const string& innoage_sn)
{
    string data;
	if (QueryLatestDataFromDB(dev_name + "-static", data) != 1)
		return -1;
	
	Json::Value data_obj;
    Json::Reader reader;
    if (!reader.parse(data, data_obj))
        return -1;
    
    if (data_obj.isObject() && data_obj.isMember("Storage")) {
		for (Json::ArrayIndex i = 0; i < data_obj["Storage"].size(); ++i) {
			string sn = data_obj["Storage"][i]["SN"].asString();
			if (sn == innoage_sn)
				return 1;
		}
    }

	return -1;
}

bool THAgent::CheckDevInTH(const ThresholdSetting& th, const string& dev)
{
	return FindInVector(th.dev_list, dev);
}

int THAgent::CheckTHDataCase(const list<string>& qpath_list)
{
	if (find(qpath_list.begin(), qpath_list.end(), "ParInfo") != qpath_list.end())
        return ParInfo;

	if (find(qpath_list.begin(), qpath_list.end(), "Storage") != qpath_list.end() || 
        find(qpath_list.begin(), qpath_list.end(), "Lifespan") != qpath_list.end())
        return Storage;

    return NormalData;
}