#include <iostream>
#include <map>
#include <mutex>
#include <thread>
#include <jsoncpp/json/json.h>
#include "Analyzer.hpp"
#include "Logger.hpp"
#include "Nosqlcommand.hpp"
#include "WebCommand.hpp"

using namespace std;

#define UNUSED(x)	(void)(x)

mutex analyzer_list_mutex;
map<string, analyzer_data> analyzer_list; // map<SN,struct>

int Analyzer_GetInitValueFromArray(const string& devName, const string& SN, const int& index, const Json::Value& storage_array_object, analyzer_data& AD)
{
	Json::Value part_info, dy_obj, dy_stor_obj;
	Json::Reader reader;

	if (storage_array_object.isMember("Par"))
	{
		part_info = storage_array_object["Par"];
		int capacity = part_info["TotalCap"].asInt();
		AD.InitialCapacity = (double)(capacity / 1024.0 / 1024.0);
		
		string dyCollectionName = devName + "-dynamic";
		string dyRawStr;
		int result = QueryLatestDataFromDB(dyCollectionName, dyRawStr);
		LOG_DEBUG << "Get dynamic raw data:" << dyRawStr;
		
		if (result != 1 || dyRawStr.empty())
		{
			LOG_WARN << "Storage " << SN << " is not contain dynamic raw data, delete data.";
			Analyzer_remove_data(SN);
			return -2;
		}

		reader.parse(dyRawStr, dy_obj);
		if (dy_obj.isMember("Storage"))
		{
			dy_stor_obj = dy_obj["Storage"][index];
			
			if (dy_stor_obj["PECycle"].asInt() > 0) 
			{
				AD.PECycle = dy_stor_obj["PECycle"].asInt();
				LOG_DEBUG << "Get PECycle:" << AD.PECycle;
			}
			else
			{
				LOG_WARN << "Storage " << SN << " is not contain PECycle, delete data.";
				Analyzer_remove_data(SN);
				return -4;
			}

			if (dy_stor_obj["Health"].asDouble() > 0)
			{
				AD.InitialHealth = dy_stor_obj["Health"].asDouble();
				LOG_DEBUG << "Get Health:" << AD.InitialHealth;
			}
			else
			{
				LOG_WARN << "Storage " << SN << " is not contain Health, delete data";
				Analyzer_remove_data(SN);
				return -5;
			}

			AD.CurrentCapacity = AD.InitialCapacity * (AD.InitialHealth/ 100.0);
			LOG_DEBUG << "Get current capacity:" << AD.CurrentCapacity;
		}
		else
		{
			LOG_WARN << "Device " << SN << " is not contain storage information, delete data.";
			Analyzer_remove_data(SN);
			return -3;
		}

		if (dy_obj.isMember("time"))
			AD.LastTime = dy_obj["time"].asInt();

		//Create now data into analyzer DB
		Json::Value analyzer_obj;
		analyzer_obj["SN"] = SN;
		analyzer_obj["Capacity"] = AD.InitialCapacity;
		analyzer_obj["InitHealth"] = AD.InitialHealth;
		analyzer_obj["InitTime"] = AD.LastTime;
		analyzer_obj["PECycle"] = AD.PECycle;
		analyzer_obj["Lifespan"] = Json::Value(Json::arrayValue);
		LOG_DEBUG << endl << "SotrageAnalyzer:" << analyzer_obj.toStyledString();

		Json::FastWriter writer;
		string jstr = writer.write(analyzer_obj);
		InsertDataToDB("StorageAnalyzer", jstr);
	}
	else
	{
		LOG_WARN << "Storage " << SN << " is not contain partition data, delete data.";
		Analyzer_remove_data(SN);
		return -1;
	}	
	
	return 0;
}

int Analyzer_GetInitValueFromQueryStr(const string& queryStr, analyzer_data& AD)
{
	Json::Value root;
	Json::Reader reader;

	if (!reader.parse(queryStr, root))
	{
		LOG_ERROR << "Failed to parse initial value from DB.";
		return -1;
	}

	if (root.isObject() && root.isMember("Capacity"))
		AD.InitialCapacity = root["Capacity"].asDouble();
	else
		return -2;

	if (root.isObject() && root.isMember("InitHealth"))
		AD.InitialHealth = root["InitHealth"].asDouble();
	else
		return -3;
	
	if (root.isObject() && root.isMember("PECycle"))
		AD.PECycle = root["PECycle"].asInt();
	else
		return -4;
	
	if (root.isObject() && root.isMember("InitTime"))
		AD.LastTime = root["InitTime"].asInt();
	else
		return -5;

	return 0;
}

int Analyzer_GetStaticRawData(const string& raw_string)
{	
	int result;
	Json::Value root;
	Json::Reader reader;
	if (!reader.parse(raw_string, root))
	{
		LOG_ERROR << "Failed to parse static raw data.";
		return -1;
	}
	if (!root.isObject())
	{
		LOG_ERROR << "raw_string is not a object.";
		return -2;
	}

	string dev_name;
	if (root.isMember("Dev"))
	{
		dev_name = root["Dev"]["Name"].asString();
		LOG_DEBUG << "Get device " << dev_name << " static raw data, find storage info.";
	}
	else
	{
		LOG_ERROR << "There is no any Static Data in DB.";
		return -3;
	}

	// Step 1. Find storages SN from static raw data.	
	Json::Value storage_array;
	if (root.isMember("Storage"))
	{
		storage_array = root["Storage"];

		for (Json::Value::ArrayIndex i = 0; i < storage_array.size(); i++)
		{
			string SN = storage_array[i]["SN"].asString();
			LOG_DEBUG << dev_name << ",  Get storage data, index " << i << ", storage S/N:" << SN;

			if (SN.compare(0, 9, "BCADevicexxxxx") == 0)
			{
				LOG_DEBUG << dev_name << ", Storage" << SN << " is fake data, skip.";
				continue;
			}

			// Step 2. Query SN from DB, then insert in analyzer_list(map).
			string querystr;
			QueryAnalyzerDataFromDB(SN, querystr);
			analyzer_data AD;
			Analyzer_insert_data(SN, AD); // AD is empty temp
			
			// Step 3. Calculate initial data.
			LOG_DEBUG << "SN=" << SN;
			if (!querystr.empty())
				result = Analyzer_GetInitValueFromQueryStr(querystr, AD);
			else
				result = Analyzer_GetInitValueFromArray(dev_name, SN, i, storage_array[i], AD);
			
			if (result != 0)
				continue;

			// Step 4. Query old analyzer data.
			if (!querystr.empty())
			{
				Json::Value analyzer_obj, analyzer_array_obj, analyzer_health_obj;
				reader.parse(querystr, analyzer_obj);

				if (analyzer_obj.isObject() && analyzer_obj.isMember("Lifespan"))
				{
					AD.days = analyzer_obj["Lifespan"].size();
					
					if (AD.days > 0)
					{
						analyzer_array_obj = analyzer_obj["Lifespan"][AD.days - 1];
						if (analyzer_array_obj.isMember("health"))
						{
							analyzer_health_obj = analyzer_array_obj["health"];
							AD.CurrentCapacity = AD.InitialCapacity * analyzer_health_obj.asDouble() / 100.0;
							if (analyzer_array_obj.isMember("time"))
								AD.LastTime = analyzer_array_obj["time"].asInt();
							else
								AD.CurrentCapacity = AD.InitialCapacity * AD.InitialHealth / 100.0;
						}
						else
							AD.CurrentCapacity = AD.InitialCapacity * AD.InitialHealth / 100.0;
					}
					else {
						AD.CurrentCapacity = AD.InitialCapacity * AD.InitialHealth / 100.0;
					}
				}
			}
			else
			{
				AD.days = 0;
				AD.CurrentCapacity = AD.InitialCapacity * AD.InitialHealth / 100.0;
			}

			// Step 5. Update the data to the  analyzer_list(map).	
			LOG_DEBUG << "[AnalyzerData parameters]";
			LOG_DEBUG << "SN:" << SN << ", "
					  << "InitialCapacity:" << AD.InitialCapacity << ", "
					  << "InitialHealth:" << AD.InitialHealth << ", "
					  << "CurrentCapacity:" << AD.CurrentCapacity << ", "
					  << "PECycle:" << AD.PECycle << ", "
					  << "Days:" << AD.days << ", "
					  << "LastTime:" << AD.LastTime;

			Analyzer_update_data(SN, AD);
		}
	}

	return 0;
}

int Analyzer_GetDynamicRawData(const string& raw_string)
{
	int flag = 0;
	Json::Value root, dy_stor_arr;
	Json::Reader reader;
	if (!reader.parse(raw_string, root))
	{
		LOG_ERROR << "Failed to parse dynamic raw data.";
		return -1;
	}
	if (!root.isObject())
	{
		LOG_ERROR << "raw_string is not a object.";
		return -2;
	}

	dy_stor_arr = root["Storage"];

	// Step 1. Get device name from dynamic raw data.
	string dev_name;
	if (root.isMember("Dev"))
	{
		dev_name = root["Dev"].asString();
	}
	LOG_DEBUG << "Get device name:" << dev_name;

	// Step 2. Query static raw data from DB using device name, and get storage SN.
	string collectionName = dev_name + "-static";
	string stRawStr;
	int result = QueryLatestDataFromDB(collectionName, stRawStr);
	if ((result != 1) || stRawStr.empty())
	{
		LOG_WARN << "There is no any Static raw data in DB!";
		return -3;
	}

	Json::Value st_raw_obj;
	if (!reader.parse(stRawStr, st_raw_obj))
	{
		LOG_ERROR << "Failed to parse static raw data.";
		return -4;
	}

	Json::Value storage_array, storage_array_obj, dy_stor_arr_obj;
	if (st_raw_obj.isMember("Storage")) 
	{
		storage_array = st_raw_obj["Storage"];

		for (Json::Value::ArrayIndex i = 0; i < storage_array.size(); i++)
		{
			storage_array_obj = storage_array[i];
			if (storage_array_obj.isMember("SN"))
			{
				string SN = storage_array_obj["SN"].asString();
				if (SN.compare(0, 9, "BCADevicexxxxx") == 0)
				{
					LOG_DEBUG << dev_name << ", Storage " << SN << " is fake data, skip.";
					continue;
				}

				// Step 3. Get storage data from the analyzer_list(map).
				LOG_DEBUG << "SN is:" << SN;
				
				auto iter = analyzer_list.find(SN);
				if (iter == analyzer_list.end())
				{
					flag = 1;
					LOG_DEBUG << "SN not exist in analyzer_list.";
				}

				if (flag == 0)
				{
					int current_time = time(nullptr);
					LOG_DEBUG << "current time:" << current_time;
					LOG_DEBUG << "time difference:" << current_time - iter->second.LastTime;

					if ((current_time - iter->second.LastTime) > 86400)
					{
						// Step 4. Calculate lifespan.
						dy_stor_arr_obj = dy_stor_arr[i];
						if (dy_stor_arr_obj.isMember("Health"))
						{
							double current_health = dy_stor_arr_obj["Health"].asDouble();
							iter->second.LastTime = current_time;
							iter->second.days++;
							double diff_health = iter->second.InitialHealth - current_health;
							diff_health = diff_health / iter->second.days / 100.0;
							double DWPD = iter->second.InitialCapacity * diff_health;
							iter->second.CurrentCapacity -= DWPD;
							double TBW = iter->second.CurrentCapacity * iter->second.PECycle / 10240;
							int calc_lifespan;
							if (DWPD > 0)
							{
								calc_lifespan = (int)floor(TBW * 1024 / DWPD / 365.0);
							}
							else
							{
								calc_lifespan = (int)floor(TBW * 1024 / 365.0);
							}

							// Step 5. Insert the result into DB.
 							InsertAnalyzerDataToDB(SN, current_time, current_health, calc_lifespan);

							//　Step 6. Update analyzer_list(map).
							Analyzer_update_data(SN, iter->second);

							LOG_DEBUG << "[analyzer data parameters after calculation]";
							LOG_DEBUG << "SN:" << SN << ", "
									  << "InitialCapacity:" << iter->second.InitialCapacity << ", "
									  << "InitialHealth:" << iter->second.InitialHealth << ", "
									  << "CurrentCapacity:" << iter->second.CurrentCapacity << ", "
									  << "PECycle:" << iter->second.PECycle << ", "
									  << "Days:" << iter->second.days << ", "
									  << "LastTime:" << iter->second.LastTime;
						}
						else
						{
							LOG_WARN << SN << ", Storage wasn't contain health data, skip.";
						}
					}
					else
					{
						LOG_DEBUG << SN << ", Storage data not over one day, skip.";
					}
					
				}
				else
				{
					LOG_INFO << SN << ", Storage wasn't contain analyzer data, create new.";
					Analyzer_GetStaticRawData(stRawStr);
					break;
				}
			}
			else
			{
				LOG_WARN << "No SN in static raw data.";
				return -6;
			}
		}
	}
	else
	{
		LOG_WARN << "No Storage information in static raw data.";
		return -5;
	}

	return 0;
}

void Analyzer_insert_data(const string& storSN, const analyzer_data& temp)
{
	lock_guard<std::mutex> lock(analyzer_list_mutex);
	auto iter = analyzer_list.find(storSN);	
	if (iter == analyzer_list.end())
	{
		analyzer_list.insert(pair<string, analyzer_data>(storSN, temp));
		LOG_DEBUG << "Insert new SN " << storSN;
	}
}

void Analyzer_update_data(const string& storSN, const analyzer_data& temp)
{
	lock_guard<mutex> lock(analyzer_list_mutex);
	if (!storSN.empty())
	{
		auto iter = analyzer_list.find(storSN);
		if (iter != analyzer_list.end())
		{
			LOG_DEBUG << "SN " << storSN << " exists.";
			iter->second = temp; //update AnalyzerData
		}
	}
}

int Analyzer_remove_data(const string& storSN)
{
	lock_guard<mutex> lock(analyzer_list_mutex);

	if (!storSN.empty())
    {
		auto iter = analyzer_list.find(storSN);
		analyzer_list.erase(iter);
        if (iter == analyzer_list.end()) //find element
			LOG_DEBUG << "Remove SN " << storSN;
	}

    return 0;
}

int Analyzer_find_data(const string& storSN)
{
	lock_guard<mutex> lock(analyzer_list_mutex);
	
	if (!storSN.empty())
	{
		auto iter = analyzer_list.find(storSN);
		if (iter != analyzer_list.end())
		{
			LOG_DEBUG << "SN " << storSN << " exists.";
			return 1;
		}  
		else
		{
			LOG_DEBUG << "Can't find SN " << storSN;
			return 0;
		}
	}

	return 0;
}