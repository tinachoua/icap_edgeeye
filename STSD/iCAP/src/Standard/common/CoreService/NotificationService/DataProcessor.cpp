#include <cmath>
#include <iomanip>
#include <sstream>
#include <jsoncpp/json/json.h>
#include "DataParser.hpp"
#include "DataProcessor.hpp"
#include "Logger.hpp"
#include "Nosqlcommand.hpp"

using namespace std;

double rounding(double num, int index)
{
    bool isNegative = false; // whether is negative number or not
	
    if (num < 0) { // if this number is negative, then convert to positive number
        isNegative = true;	
        num = -num;
    }
	
    if (index >= 0) {
        int multiplier;
        multiplier = pow(10, index);
        num = (int)(num * multiplier + 0.5) / (multiplier * 1.0);
    }
	
    if (isNegative) // if this number is negative, then convert to negative number
        num = -num;
	
    return num;
}

DataProcessor::DataProcessor()
{
}

DataProcessor::~DataProcessor()
{
}

DevInfo DataProcessor::GetDeviceInfo(const ThresholdSetting& th, const string dev_name)
{
    THAgent th_agent;
    DevInfo dev_info;

    dev_info.dev_name = dev_name;
    if (th_agent.CheckTHDataCase(th.Data.qpath_list) >= Storage)
		dev_info.storage_list = GetStorageList(dev_name);

    return dev_info;
}

map<string, int> DataProcessor::GetStorageList(const string& dev_name)
{
	map<string, int> ret;
	string collection = dev_name + "-static";
	string static_doc;
	
    int result = QueryLatestDataFromDB(collection, static_doc);
	if (result != 1 || static_doc.empty())
		return ret;

    Json::Value static_obj;
	Json::Reader reader;
	if (!reader.parse(static_doc, static_obj)) {
		LOG_ERROR << "Failed to parse satic raw data of " << dev_name << ".";
		return ret;
	}

    for (Json::Value::ArrayIndex sid = 0; sid < static_obj["Storage"].size(); sid++)
		ret.insert(pair<string, int>(static_obj["Storage"][sid]["SN"].asString(), 
                    static_obj["Storage"][sid]["Par"]["NumofPar"].asInt()));

	return ret;
}

vector<DevData> DataProcessor::GetData(const ThresholdSetting& th, const DevInfo& dev_info, const string& raw)
{
    vector<DevData> ret;

#ifdef DEBUG
    LOG_TRACE << "[Numerator qpath_list]";
    PrintQpath(th.Data.qpath_list);
#endif

    /* Get numerator */
    vector<DevData> nume_data = GetDevDataFromDB(dev_info, th.Data.qpath_list, raw);
#ifdef DEBUG
    PrintData(nume_data);
#endif

    ret = nume_data; // copy dev_name and time

    /* Get denominator */
    vector<DevData> deno_data;
    if (th.Denominator.qpath_list.size() != 0) {
#ifdef DEBUG
        LOG_TRACE << "[Denominator qpath_list]";
        PrintQpath(th.Denominator.qpath_list);
#endif

        deno_data = GetDevDataFromDB(dev_info, th.Denominator.qpath_list, raw);
#ifdef DEBUG
        PrintData(deno_data);
#endif

        THAgent th_agent;
        if (th_agent.CheckTHDataCase(th.Data.qpath_list) == ParInfo) {
            vector<DevData> deno_tmp;
            unsigned deno_index = 0;

            for (const auto& storage : dev_info.storage_list) {
                for (int partition_index = 0; partition_index < storage.second; partition_index++)
                    deno_tmp.push_back(deno_data[deno_index]);

                deno_index++;
            }

            if (deno_tmp.size() > 0)
                deno_data.assign(deno_tmp.begin(), deno_tmp.end());
        }

        /* Numerator devided by denominator */
        double nume, deno;
        for (auto nit = nume_data.begin(); nit != nume_data.end(); ++nit) {
            auto nume_index = distance(nume_data.begin(), nit);

            if (IsAnyNullType(nit->value))
                continue;
            nume = GetDoubleFromAny(nit->value);
            
            if (IsAnyNullType(deno_data[nume_index].value))
                continue;
            deno = GetDoubleFromAny(deno_data[nume_index].value);
            
            auto value = (nume / deno) * 100;
            ret[nume_index].value = value;
        }
    }

#ifdef DEBUG
    PrintData(ret);
#endif

    return ret;
}

void DataProcessor::PrintQpath(const list<string>& qpath)
{
    string log;
    for (const auto& item : qpath)
        log += item + " ";
    LOG_TRACE << log;
}

void DataProcessor::PrintData(const vector<DevData>& data)
{
	LOG_TRACE << "[data]";
    for (const auto& d : data) {
        string log = d.dev_name + " ";
        if (d.value.type() == typeid(int)) {
            log += to_string(boost::any_cast<int>(d.value));
        } else if (d.value.type() == typeid(double)) {
            log += to_string(boost::any_cast<double>(d.value));
        } else if (d.value.type() == typeid(pair<double, double>)) {	
            pair<double, double> pair_tmp = boost::any_cast<pair<double, double>>(d.value);
            log += to_string(pair_tmp.first) + " " + to_string(pair_tmp.second);
        } else if (d.value.type() == typeid(NULL)) {
            log += "NULL";
        }
        LOG_TRACE << log;
    }
}

vector<DevData> DataProcessor::GetDevDataFromDB(const DevInfo& dev_info, list<string> qpath_list, const string& raw)
{
	vector<DevData> ret;
    THAgent th_agent;
    DataParser data_parser;

    if (qpath_list.size() == 0)
        return ret;
	
    string collection_prefix = qpath_list.front();
	transform(collection_prefix.begin(), collection_prefix.end(), collection_prefix.begin(), ::tolower);
	qpath_list.pop_front();

    int data_case = th_agent.CheckTHDataCase(qpath_list);
	
    if (collection_prefix.compare("static") == 0 || collection_prefix.compare("dynamic") == 0) {
        string collection_name = dev_info.dev_name + "-" + collection_prefix;
        string db_data;
        
        if (collection_prefix.compare("static") == 0) {
            if (QueryLatestDataFromDB(collection_name, db_data) != 1)
                return ret;
        } else if (collection_prefix.compare("dynamic") == 0) {
            db_data = raw; // Use MQTT recieved raw data
        }
    
        Json::Value raw_obj;
        Json::Reader reader;
        if (!reader.parse(db_data, raw_obj)) {
            LOG_ERROR << "Failed to parse raw data." << reader.getFormattedErrorMessages();
            return ret;
        }

        if (data_case == ParInfo) { // parition case
            auto findit = find(qpath_list.begin(), qpath_list.end(), "Capacity");
            if (findit != qpath_list.end()) {
                for (const auto& storage : dev_info.storage_list) {
                    for (Json::ArrayIndex i = 0; i < raw_obj["Storage"].size(); i++) {
                        if (raw_obj["Storage"][i]["SN"].asString().compare(storage.first) == 0) {
                            for (int pid = 0; pid < storage.second; pid++) {
                                int capacity = raw_obj["Storage"][i]["Par"]["ParInfo"][pid]["Capacity"].asInt();
                                DevData dev_data_item = SetDevData(storage.first + '[' + to_string(pid) + ']', 
                                                                    capacity, raw_obj["time"].asInt());
                                ret.push_back(dev_data_item);
                            }
                            break;
                        }
                    }
                }
            }
        } else if (!qpath_list.front().compare("Storage")) {
            for (const auto& storage : dev_info.storage_list) {
                for (Json::ArrayIndex i = 0; i < raw_obj["Storage"].size(); i++) {
                    if (raw_obj["Storage"][i]["SN"].asString().compare(storage.first) == 0) {
                        ValueTimePair value_time_pair = data_parser.ParseValueFromDB(raw_obj["Storage"][i].toStyledString(), 
                                                                                        qpath_list);
                        DevData dev_data_item = SetDevData(storage.first, value_time_pair.first, value_time_pair.second);
                        ret.push_back(dev_data_item);
                        break;
                    }
                }
            }
        } else {
            ValueTimePair value_time_pair = data_parser.ParseValueFromDB(db_data, qpath_list);
            DevData dev_data_item = SetDevData(dev_info.dev_name, value_time_pair.first, value_time_pair.second);
            ret.push_back(dev_data_item);
        } 
    } else if (collection_prefix.compare("storageanalyzer") == 0) {
        for (const auto& storage : dev_info.storage_list) {
            string SN = storage.first;
            string db_data;
            QueryAnalyzerDataFromDB(SN, db_data);

            DevData dev_data_item;
            if (db_data.empty()) {
                dev_data_item = SetDevData(SN, NULL, 0);
                ret.push_back(dev_data_item);
                continue;
            }
            
            ValueTimePair value_time_pair = data_parser.ParseValueFromDB(db_data, qpath_list);
            dev_data_item = SetDevData(SN, value_time_pair.first, value_time_pair.second);
            ret.push_back(dev_data_item);
        }
	}

	return ret;
}

DevData DataProcessor::SetDevData(const string& dev_name, const boost::any& value, const int& time)
{
    DevData dev_data;
    dev_data.dev_name = dev_name;
    dev_data.value = value;
    dev_data.time = time;
    return dev_data;
}

bool DataProcessor::IsAnyNullType(const boost::any& value)
{
    if (value.type() == typeid(NULL))
        return true;
    return false;
}

double DataProcessor::GetDoubleFromAny(const boost::any& value)
{
    double value_f = 0.0;
    if (value.type() == typeid(int)) {
        int temp = boost::any_cast<int>(value);
        value_f = static_cast<double>(temp);
    } else if (value.type() == typeid(double)) {
        value_f = boost::any_cast<double>(value);
    }
    return value_f;
}

bool DataProcessor::CheckDevDataOverTH(const ThresholdSetting& th, const DevData& dev_data, string& message)
{
    if (dev_data.value.type() == typeid(NULL))
        return false;

    bool over_threshold = false;
    ostringstream oss;
    
    oss << th.value_list[0];
    
    if (th.Denominator.qpath_list.size() == 0)
        message = dev_data.dev_name + " " + th.Data.name;
    else
        message = dev_data.dev_name + " " + th.Data.name + "/" + th.Denominator.name;

    double data_tmp = GetDoubleFromAny(dev_data.value);
            
    switch (th.func) {
        case 0:
            if (data_tmp == th.value_list[0]) {
                message += " = " + oss.str();
                if (th.Denominator.qpath_list.size() == 0)
                    message += " " + th.Data.unit;
                over_threshold = true;
            }
            break;
        case 1:
            if (data_tmp < th.value_list[0]) {
                message += " < " + oss.str();
                if (th.Denominator.qpath_list.size() == 0)
                    message += " " + th.Data.unit;
                over_threshold = true;
            }
            break;
        case 2:
            if (data_tmp > th.value_list[0]) {
                message += " > " + oss.str();
                if (th.Denominator.qpath_list.size() == 0)
                    message += " " + th.Data.unit;
                over_threshold = true;
            }
            break;
        case 3:
            if (data_tmp != th.value_list[0]) {
                message += " ≠ " + oss.str();
                if (th.Denominator.qpath_list.size() == 0)
                    message += " " + th.Data.unit;
                over_threshold = true;
            }
            break;
        case 4:
            if (data_tmp <= th.value_list[0]) {
                message += " ≤ " + oss.str();
                if (th.Denominator.qpath_list.size() == 0)
                    message += " " + th.Data.unit;
                over_threshold = true;
            }
            break;
        case 5:
            if (data_tmp >= th.value_list[0]) {
                message += " ≥ " + oss.str();
                if (th.Denominator.qpath_list.size() == 0)
                    message += " " + th.Data.unit;
                over_threshold = true;
            }
            break;
        case 6:
            if (data_tmp >= th.value_list[0] && data_tmp <= th.value_list[1]) {
                message += " between " + oss.str() + "-";
                oss.str("");
                oss << th.value_list[1];
                message += oss.str();
                if (th.Denominator.qpath_list.size() == 0)
                    message += " " + th.Data.unit;
                over_threshold = true;
            }
            break;
        case 7:
            if (data_tmp == th.value_list[0]) {
                over_threshold = true;
                if (data_tmp == 1)
                    message += " is online";
                else
                    message += " is offline";
            }
            break;
        case 8:
            if (data_tmp > th.value_list[0]) {
                message += " > " + oss.str();
                if (th.Denominator.qpath_list.size() == 0)
                    message += " " + th.Data.unit;
                over_threshold = true;
            }
            if (data_tmp < th.value_list[1]) {
                message += " < " + oss.str();
                if (th.Denominator.qpath_list.size() == 0)
                    message += " " + th.Data.unit;
                over_threshold = true;
            }
            break;
        default:
            break;
    }
    
    string value_str;
    if (dev_data.value.type() == typeid(int)) {
        value_str = to_string(boost::any_cast<int>(dev_data.value));
    } else if (dev_data.value.type() == typeid(string)) {
        value_str = boost::any_cast<std::string>(dev_data.value);
    } else if (dev_data.value.type() == typeid(double)) {
        stringstream stream;
        stream << fixed << setprecision(2) << rounding(boost::any_cast<double>(dev_data.value), 2);
        value_str = stream.str();
    }
    
    if (th.Denominator.qpath_list.size() == 0)
        message += ", value: " + value_str + " " + th.Data.unit + "."; 
    else
        message += " %, value: " + value_str + " %.";

    return over_threshold;
}