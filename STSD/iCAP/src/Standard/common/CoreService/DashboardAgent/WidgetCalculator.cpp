#include <cmath>
#include <list>
#include <map>
#include <string>
#include <vector>
#include <boost/any.hpp>
#include <jsoncpp/json/json.h>
#include "DataParser.hpp"
#include "Logger.hpp"
#include "main.hpp"
#include "Nosqlcommand.hpp"
#include "RedisCommunicator.hpp"
#include "WidgetCalculator.hpp"

using namespace std;

enum DataCase { NormalData, Storage, ParInfo, InnoAge}; // special case

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

void WidgetCalculator::CalculateWidget(WidgetInfoTemplate& widget_info)
{
    BOOST_LOG_NAMED_SCOPE(SCOPE_NAME);
	
    GetDeviceInfoList(widget_info);

	WidgetData* wd = GetWidgetData(widget_info);
	
#ifdef DEBUG
    LOG_TRACE << "[WidgetData]";
	cout << "wd->id:" << wd->id << endl;
	cout << "wd->name:" << wd->name << endl;
	cout << "wd->width:"<< wd->width << endl;
	cout << "wd->type:"<< wd->type << endl;
	cout << "wd->file_path:" << wd->file_path << endl;
	cout << "wd->data:" << endl;

	if (wd->data.value.size() > 0) {
		cout << "   +--label:" << endl;
		for (auto lit = wd->data.label.begin(); lit != wd->data.label.end(); ++lit)
			cout << "      " << distance(wd->data.label.begin(), lit) << ":" << *lit << endl;
		cout << "   +--data:" << endl;
		for (auto vit = wd->data.value.begin(); vit != wd->data.value.end(); ++vit)
			cout << "      " << distance(wd->data.value.begin(), vit) << ":" << *vit << endl;
	}
	if (wd->map.value.size() > 0) {
		cout << "   +--centerLng:" << wd->map.center_lng << endl;
		cout << "   +--centerLat:" << wd->map.center_lat << endl;
		cout << "   +--value" << endl;
		for (auto it = wd->map.value.begin(); it != wd->map.value.end(); ++it) {
			cout << "      " << distance(wd->map.value.begin(), it) << endl;
			cout << "      +--name:" << it->devices_info.name << endl;
			cout << "      +--alias:" << it->devices_info.alias << endl;
			cout << "      +--color:" << it->devices_info.color << endl;
			cout << "      +--status:" << it->devices_info.status << endl;
			cout << "      +--owner:" << it->devices_info.owner << endl;
			cout << "      +--detail:" << it->devices_info.detail << endl;
			cout << "      +--time:" << it->devices_info.time << endl;
			cout << "      +--position:" << endl;
			cout << "         +--lat:" << it->position.lat << endl;
			cout << "         +--lng:" << it->position.lng << endl;
		}
	}
	if (wd->customized_map.size() > 0 ) {
		cout << "   +--markers" << endl;
		for (auto marker_it = wd->customized_map.begin(); marker_it != wd->customized_map.end(); ++marker_it) {
			cout << "      " << distance(wd->customized_map.begin(), marker_it) << endl;
			cout << "      +--name:" << marker_it->name << endl;
			cout << "      +--x:" << marker_it->x << endl;
			cout << "      +--y:" << marker_it->y << endl;
			cout << "      +--isNormal:" << marker_it->is_normal << endl;
			cout << "      +--devices" << endl;
			for (auto dev_it = marker_it->devices_info.begin(); dev_it != marker_it->devices_info.end(); ++dev_it) {
				cout << "         " << distance(marker_it->devices_info.begin(), dev_it) << endl;
				cout << "         +--name:" << dev_it->name << endl;
				cout << "         +--alias:" << dev_it->alias << endl;
				cout << "         +--detail:" << dev_it->detail << endl;
				cout << "         +--owner:" << dev_it->owner << endl;
				cout << "         +--status:" << dev_it->status << endl;
				cout << "         +--time:" << dev_it->time << endl;
			}
		}
	}
	if (wd->vectormap.markers.size() > 0) {	
		cout << "   +--mapIndex:" << wd->vectormap.mapIndex << endl;
		for (auto mit = wd->vectormap.markers.begin(); mit != wd->vectormap.markers.end(); ++mit) {
			cout << "   +--markers" << endl;
			cout << "      " << distance(wd->vectormap.markers.begin(), mit) << ":" << endl;
			cout << "      +--latLng:" << endl;
			cout << "         0:" << mit->latLng[0] << endl;
			cout << "         1:" << mit->latLng[1] << endl;
			cout << "      +--style:" << endl;
			cout << "         +--r:" << mit->style.r << endl;
			cout << "         +--fill:" << mit->style.fill << endl;
		}
		cout << "   +--dev_cnt" << endl;
		for (auto dcit = wd->vectormap.dev_cnt.begin(); dcit != wd->vectormap.dev_cnt.end(); ++dcit) {
			cout << "      " << distance(wd->vectormap.dev_cnt.begin(), dcit) << ":";
			cout << *dcit << endl;
		}
		cout << "   +--event_cnt" << endl;
		for (auto ecit = wd->vectormap.event_cnt.begin(); ecit != wd->vectormap.event_cnt.end(); ++ecit) {
			cout << "      " << distance(wd->vectormap.event_cnt.begin(), ecit) << ":";
			cout << *ecit << endl;
		}
	}
	cout << "wd->detailWidget:" << endl;
	cout << "   +--item:" << endl;
	for (auto dwi_it = wd->detailWidget.item.begin(); dwi_it != wd->detailWidget.item.end(); ++dwi_it)
		cout << "      " << distance(wd->detailWidget.item.begin(), dwi_it) << ":" << *dwi_it << endl;
	cout << "   +--record:" << endl;
	for (auto dit = wd->detailWidget.record_array.begin(); dit != wd->detailWidget.record_array.end(); ++dit) {
		cout << "      " << distance(wd->detailWidget.record_array.begin(), dit) << ":" << endl;
		for (auto rit = (*dit).begin(); rit != (*dit).end(); ++rit) {
			cout << "         " << distance((*dit).begin(), rit) << ":" << endl;
			cout << "         +--name:" << (*rit).name << endl;
			cout << "         +--dev_name:" << (*rit).dev_name << endl;
			cout << "         +--storage_sn:" << (*rit).storage_sn << endl;
			cout << "         +--time:" << (*rit).time << endl;
			cout << "         +--value:" << (*rit).value << endl;
			cout << "         +--alias:" << (*rit).alias << endl;
			cout << "         +--owner_name:" << (*rit).owner_name << endl;
		}
	}
#endif

	Json::Value widget_obj;
	widget_obj["name"] = wd->name;
	if (wd->data.value.size() > 0) {
		for (auto lit = wd->data.label.begin(); lit != wd->data.label.end(); ++lit)
			widget_obj["data"]["label"][(int)distance(wd->data.label.begin(), lit)] = *lit;
		for (auto vit = wd->data.value.begin(); vit != wd->data.value.end(); ++vit)
			widget_obj["data"]["value"][(int)distance(wd->data.value.begin(), vit)] = *vit;
	}

	widget_obj["width"] = wd->width;
	widget_obj["type"] = wd->type;	
	widget_obj["filepath"] = wd->file_path;
	if (wd->map.value.size() > 0) {
		widget_obj["data"]["centerLng"] = wd->map.center_lng;
		widget_obj["data"]["centerLat"] = wd->map.center_lat;
		for (auto it = wd->map.value.begin(); it != wd->map.value.end(); ++it) {
			widget_obj["data"]["value"][(int)distance(wd->map.value.begin(), it)]["name"] = it->devices_info.name;
			widget_obj["data"]["value"][(int)distance(wd->map.value.begin(), it)]["alias"] = it->devices_info.alias;
			widget_obj["data"]["value"][(int)distance(wd->map.value.begin(), it)]["color"] = it->devices_info.color;
			widget_obj["data"]["value"][(int)distance(wd->map.value.begin(), it)]["status"] = it->devices_info.status;
			widget_obj["data"]["value"][(int)distance(wd->map.value.begin(), it)]["owner"] = it->devices_info.owner;
			widget_obj["data"]["value"][(int)distance(wd->map.value.begin(), it)]["detail"] = it->devices_info.detail;
			widget_obj["data"]["value"][(int)distance(wd->map.value.begin(), it)]["time"] = it->devices_info.time;
			widget_obj["data"]["value"][(int)distance(wd->map.value.begin(), it)]["position"]["lat"] = it->position.lat;
			widget_obj["data"]["value"][(int)distance(wd->map.value.begin(), it)]["position"]["lng"] = it->position.lng;
		}
	} else {
		// no device case
		if ((wd->type.compare("google map") == 0) || (wd->type.compare("open street map") == 0)
			|| (wd->type.compare("gaode map") == 0)) {
			widget_obj["data"]["centerLng"] = wd->map.center_lng;
			widget_obj["data"]["centerLat"] = wd->map.center_lat;
		}
	}

	if (wd->customized_map.size() > 0) {
		for (auto marker_it = wd->customized_map.begin(); marker_it != wd->customized_map.end(); ++marker_it) {
			int marker_index = static_cast<int>(distance(wd->customized_map.begin(), marker_it));
			widget_obj["data"]["markers"][marker_index]["x"] = marker_it->x;
			widget_obj["data"]["markers"][marker_index]["y"] = marker_it->y;
			widget_obj["data"]["markers"][marker_index]["isNormal"] = marker_it->is_normal;
			widget_obj["data"]["markers"][marker_index]["name"] = marker_it->name;
			if (marker_it->devices_info.size() > 0) {
				for (auto dev_it = marker_it->devices_info.begin(); dev_it != marker_it->devices_info.end(); ++dev_it) {
					int dev_index = static_cast<int>(distance(marker_it->devices_info.begin(), dev_it));
					widget_obj["data"]["markers"][marker_index]["devices"][dev_index]["alias"] = dev_it->alias;
					widget_obj["data"]["markers"][marker_index]["devices"][dev_index]["detail"] = dev_it->detail;
					widget_obj["data"]["markers"][marker_index]["devices"][dev_index]["name"] = dev_it->name;
					widget_obj["data"]["markers"][marker_index]["devices"][dev_index]["owner"] = dev_it->owner;
					widget_obj["data"]["markers"][marker_index]["devices"][dev_index]["status"] = dev_it->status;
					widget_obj["data"]["markers"][marker_index]["devices"][dev_index]["time"] = dev_it->time;
				}
			} else {
				widget_obj["data"]["markers"][marker_index]["devices"] = Json::Value(Json::arrayValue);
			}
		}
	} else {
		widget_obj["data"]["markers"] = Json::Value(Json::arrayValue);
	}

	if (wd->vectormap.markers.size() > 0) {	
		widget_obj["data"]["mapIndex"] = wd->vectormap.mapIndex;
		for (auto mit = wd->vectormap.markers.begin(); mit != wd->vectormap.markers.end(); ++mit) {
			widget_obj["data"]["markers"][(int)distance(wd->vectormap.markers.begin(), mit)]["latLng"][0] = mit->latLng[0];
			widget_obj["data"]["markers"][(int)distance(wd->vectormap.markers.begin(), mit)]["latLng"][1] = mit->latLng[1];
			widget_obj["data"]["markers"][(int)distance(wd->vectormap.markers.begin(), mit)]["style"]["r"] = mit->style.r;
			widget_obj["data"]["markers"][(int)distance(wd->vectormap.markers.begin(), mit)]["style"]["fill"] = mit->style.fill;
		}
		for (auto dcit = wd->vectormap.dev_cnt.begin(); dcit != wd->vectormap.dev_cnt.end(); ++dcit)
			widget_obj["data"]["deviceCount"][(int)distance(wd->vectormap.dev_cnt.begin(), dcit)] = *dcit;
		for (auto ecit = wd->vectormap.event_cnt.begin(); ecit != wd->vectormap.event_cnt.end(); ++ecit)
			widget_obj["data"]["eventCount"][(int)distance(wd->vectormap.event_cnt.begin(), ecit)] = *ecit;
	} else {
		// no device case
		if (wd->type.compare("vector map") == 0) 
			widget_obj["data"]["mapIndex"] = wd->vectormap.mapIndex;
	}

	for (auto dwit = wd->detailWidget.item.begin(); dwit != wd->detailWidget.item.end(); ++dwit)
		widget_obj["detailWidget"]["item"][(int)distance(wd->detailWidget.item.begin(), dwit)] = (*dwit);
	for (auto rait = wd->detailWidget.record_array.begin(); rait != wd->detailWidget.record_array.end(); ++rait) {
		for (auto rit = (*rait).begin(); rit != (*rait).end(); ++rit) {
			widget_obj["detailWidget"]["record"][(int)distance(wd->detailWidget.record_array.begin(), rait)]
				[(int)distance((*rait).begin(), rit)]["name"] = (*rit).name;
			widget_obj["detailWidget"]["record"][(int)distance(wd->detailWidget.record_array.begin(), rait)]
				[(int)distance((*rait).begin(), rit)]["devName"] = (*rit).dev_name;
			widget_obj["detailWidget"]["record"][(int)distance(wd->detailWidget.record_array.begin(), rait)]
				[(int)distance((*rait).begin(), rit)]["storageSN"] = (*rit).storage_sn;
			widget_obj["detailWidget"]["record"][(int)distance(wd->detailWidget.record_array.begin(), rait)]
				[(int)distance((*rait).begin(), rit)]["value"] = (*rit).value;
			widget_obj["detailWidget"]["record"][(int)distance(wd->detailWidget.record_array.begin(), rait)]
				[(int)distance((*rait).begin(), rit)]["ownerName"] = (*rit).owner_name;
			widget_obj["detailWidget"]["record"][(int)distance(wd->detailWidget.record_array.begin(), rait)]
				[(int)distance((*rait).begin(), rit)]["alias"] = (*rit).alias;
			widget_obj["detailWidget"]["record"][(int)distance(wd->detailWidget.record_array.begin(), rait)]
				[(int)distance((*rait).begin(), rit)]["time"] = (*rit).time;
		}
	}

	widget_obj["time"] = static_cast<long long>(time(nullptr));
	string jstr = widget_obj.toStyledString();
	InsertDataToCappedColl("Widget" + to_string(wd->id), jstr);
	delete wd;
}

void WidgetCalculator::GetDeviceInfoList(WidgetInfoTemplate& widget_info)
{
	for (auto dit = widget_info.DeviceList.begin(); dit != widget_info.DeviceList.end(); ++dit) {	
		if (!(CheckCollExists(dit->name + "-static")))	continue;

		if (!(CheckCollExists(dit->name + "-dynamic")))	continue;
		
		DevInfo devInfo;
		devInfo.dev_name = dit->name;

		int data_case = CheckDataCase(widget_info.Data.qpath_list);
        
        map<string, int> storage_list;
        if (data_case == InnoAge) {
            storage_list = GetStorageList(dit->name, true);
            devInfo.storage_list = storage_list;
        } else if (data_case >= Storage) {
			storage_list = GetStorageList(dit->name);
			devInfo.storage_list = storage_list;
		}

        widget_info.dev_info_list.push_back(devInfo);
	}
}

int WidgetCalculator::CheckDataCase(const list<string>& qpath_list)
{
	if (find(qpath_list.begin(), qpath_list.end(), "ParInfo") != qpath_list.end())
        return ParInfo;

	if (find(qpath_list.begin(), qpath_list.end(), "Storage") != qpath_list.end() || 
        find(qpath_list.begin(), qpath_list.end(), "Lifespan") != qpath_list.end())
        return Storage;
    
    if (find(qpath_list.begin(), qpath_list.end(), "Redis") != qpath_list.end() && 
        find(qpath_list.begin(), qpath_list.end(), "DB2") != qpath_list.end())
        return InnoAge;

    return NormalData;
}

map<string, int> WidgetCalculator::GetStorageList(const string& dev_name, const bool& check_is_innoage)
{
	map<string, int> ret;
	string collection = dev_name + "-static";
	string static_doc;
	int result = QueryLatestDataFromDB(collection, static_doc);
	if (result != 1 || static_doc.empty())
		return ret;

    Json::Value static_obj;
	Json::Reader reader;
	bool parsing_successful = reader.parse(static_doc, static_obj);
	if (!parsing_successful) {
		LOG_ERROR << "Failed to parse configuration." << reader.getFormattedErrorMessages();
		return ret;
	}

    for (Json::Value::ArrayIndex sid = 0; sid < static_obj["Storage"].size(); sid++) {
        string sn = static_obj["Storage"][sid]["SN"].asString();
        if (sn.size() == 0)
            continue;
        if (check_is_innoage) {
            string fwver = static_obj["Storage"][sid]["FWVer"].asString();
            if (fwver.size() > 0 && fwver.at(0) != 'B')
                continue;
            ret.insert(pair<string, int>(static_obj["Storage"][sid]["SN"].asString(), static_obj["Storage"][sid]["Par"]["NumofPar"].asInt()));
        } else {
            ret.insert(pair<string, int>(static_obj["Storage"][sid]["SN"].asString(), static_obj["Storage"][sid]["Par"]["NumofPar"].asInt()));
        }
    }

	return ret;
}

WidgetData* WidgetCalculator::GetWidgetData(WidgetInfoTemplate wi)
{
    vector<DevData> data = GetData(wi);

    vector<unsigned> store_detail_index;
    vector<unsigned> statis_data = CaculateData(wi, data, store_detail_index);

    WidgetData* ret = new WidgetData;
    SortStatisDataByChartType(wi, statis_data, ret);

    SetWidgetDetail(wi, data, store_detail_index, ret);
	
	return ret;
}

vector<DevData> WidgetCalculator::GetData(WidgetInfoTemplate wi)
{
    vector<DevData> ret;

    if ((wi.Setting.Func >= 5) && (wi.Setting.Func <= 7)) // ignore map
        return ret;

#ifdef DEBUG
    LOG_TRACE << "[Numerator qpath_list]";
    PrintQpath(wi.Data.qpath_list);
#endif

    /* Get numerator */
    vector<DevData> nume_data = GetDevDataFromDB(wi.dev_info_list, wi.Data.qpath_list);
#ifdef DEBUG
    PrintData(nume_data);
#endif

    ret = nume_data; // copy dev_name and time

#ifdef DEBUG
    LOG_TRACE << "[Denominator qpath_list]";
    PrintQpath(wi.Denominator.qpath_list);
#endif
    
    /* Get denominator */
    vector<DevData> deno_data;
    if (wi.Setting.Divider.DenominatorId != 0) {
        deno_data = GetDevDataFromDB(wi.dev_info_list, wi.Denominator.qpath_list);
#ifdef DEBUG
        PrintData(deno_data);
#endif

        if (CheckDataCase(wi.Data.qpath_list) == ParInfo) {
            vector<DevData> deno_tmp;
            unsigned deno_index = 0;

            for (const auto& dev : wi.dev_info_list)
                for (const auto& storage : dev.storage_list) {
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

    return ret;
}

vector<unsigned> WidgetCalculator::CaculateData(WidgetInfoTemplate& wi, vector<DevData>& data, vector<unsigned>& store_detail_index)
{    
    vector<unsigned> ret;

    switch (wi.Setting.Func) {
        case 1: // Threshold
            ret = CaculateDataForTH(wi, data, store_detail_index);
            GenerateLabelForTHSetting(wi);
            break;
        case 2: // Percentage
            ret = CaculateDataForPercentage(wi, data, store_detail_index);
            break;
        case 3: // Boolean
            ret = CaculateDataForBoolean(wi, data, store_detail_index);
            break;
        case 4: // Numerical
            ret = CaculateDataForNumerical(wi, data, store_detail_index);
            break;
        case 5: // Google map, Open Street map, Gaode map
        case 6: // Vector map
        case 7: // Custom map
        default:
            break;
    }

    LOG_DEBUG << "[statis result]:";
    for (auto it = ret.begin(); it != ret.end(); ++it)
        LOG_DEBUG << *it;

    return ret;
}

vector<unsigned> WidgetCalculator::CaculateDataForTH(WidgetInfoTemplate& wi, const vector<DevData>& data, vector<unsigned>& store_detail_index)
{
    vector<unsigned> ret;
    unsigned overt_th_count = 0;
    unsigned lowert_th_count = 0;

    for (const auto& d : data) {
        if (d.value.type() != typeid(NULL)) {
            double data_tmp = GetDoubleFromAny(d.value);

            switch (wi.ThresholdSetting.func) {
                case 0:
                    if (data_tmp == wi.ThresholdSetting.value[0])
                        ProcessCounterForTHSetting(false, lowert_th_count, store_detail_index);
                    else
                        ProcessCounterForTHSetting(true, overt_th_count, store_detail_index);
                    break;
                case 1:
                    if (data_tmp < wi.ThresholdSetting.value[0])
                        ProcessCounterForTHSetting(false, lowert_th_count, store_detail_index);
                    else
                        ProcessCounterForTHSetting(true, overt_th_count, store_detail_index);
                    break;
                case 2:
                    if (data_tmp > wi.ThresholdSetting.value[0])
                        ProcessCounterForTHSetting(false, lowert_th_count, store_detail_index);
                    else
                        ProcessCounterForTHSetting(true, overt_th_count, store_detail_index);
                    break;
                case 3:
                    if (data_tmp != wi.ThresholdSetting.value[0])
                        ProcessCounterForTHSetting(false, lowert_th_count, store_detail_index);
                    else
                        ProcessCounterForTHSetting(true, overt_th_count, store_detail_index);
                    break;
                case 4:
                    if (data_tmp <= wi.ThresholdSetting.value[0])
                        ProcessCounterForTHSetting(false, lowert_th_count, store_detail_index);
                    else
                        ProcessCounterForTHSetting(true, overt_th_count, store_detail_index);
                    break;
                case 5:
                    if (data_tmp >= wi.ThresholdSetting.value[0])
                        ProcessCounterForTHSetting(false, lowert_th_count, store_detail_index);
                    else
                        ProcessCounterForTHSetting(true, overt_th_count, store_detail_index);
                    break;
                case 6:
                    if (data_tmp >= wi.ThresholdSetting.value[0] && data_tmp <= wi.ThresholdSetting.value[1])
                        ProcessCounterForTHSetting(false, lowert_th_count, store_detail_index);
                    else
                        ProcessCounterForTHSetting(true, overt_th_count, store_detail_index);
                    break;
                case 7:
                    if (data_tmp == wi.ThresholdSetting.value[0])
                        ProcessCounterForTHSetting(false, lowert_th_count, store_detail_index);
                    else
                        ProcessCounterForTHSetting(true, overt_th_count, store_detail_index);
                    break;
                case 8:
                    if (data_tmp > wi.ThresholdSetting.value[0] || data_tmp < wi.ThresholdSetting.value[1])
                        ProcessCounterForTHSetting(false, lowert_th_count, store_detail_index);
                    else
                        ProcessCounterForTHSetting(true, overt_th_count, store_detail_index);
                    break;
            }
        } else {
            store_detail_index.push_back(-1);
        }
    }

    ret.push_back(lowert_th_count);
    ret.push_back(overt_th_count);

    return ret;
}

void WidgetCalculator::ProcessCounterForTHSetting(bool is_overth, unsigned& counter, vector<unsigned>& store_detail_index)
{
    counter++;
    if (is_overth)
        store_detail_index.push_back(1);
    else
        store_detail_index.push_back(0);
}

void WidgetCalculator::GenerateLabelForTHSetting(WidgetInfoTemplate& wi)
{
    string lowert_label;
    string overt_label;

    switch (wi.ThresholdSetting.func) {
        case 0:
            overt_label = "≠" + to_string((int)round(wi.ThresholdSetting.value[0])) + " " + wi.Data.unit;
            lowert_label = "=" + to_string((int)round(wi.ThresholdSetting.value[0])) + " " + wi.Data.unit;
            break;
        case 1:
            overt_label = "≥" + to_string((int)round(wi.ThresholdSetting.value[0])) + " " + wi.Data.unit;
            lowert_label = "<" + to_string((int)round(wi.ThresholdSetting.value[0])) + " " + wi.Data.unit;
            break;
        case 2:
            overt_label = "≤" + to_string((int)round(wi.ThresholdSetting.value[0])) + " " + wi.Data.unit;
            lowert_label = ">" + to_string((int)round(wi.ThresholdSetting.value[0])) + " " + wi.Data.unit;
            break;
        case 3:
            overt_label = "=" + to_string((int)round(wi.ThresholdSetting.value[0])) + " " + wi.Data.unit;
            lowert_label = "≠" + to_string((int)round(wi.ThresholdSetting.value[0])) + " " + wi.Data.unit;
            break;
        case 4:
            overt_label = ">" + to_string((int)round(wi.ThresholdSetting.value[0])) + " " + wi.Data.unit;
            lowert_label = "≤" + to_string((int)round(wi.ThresholdSetting.value[0])) + " " + wi.Data.unit;
            break;
        case 5:
            overt_label = "<" + to_string((int)round(wi.ThresholdSetting.value[0])) + " " + wi.Data.unit;
            lowert_label = "≥" + to_string((int)round(wi.ThresholdSetting.value[0])) + " " + wi.Data.unit;
            break;
        case 6:
            overt_label = "besides " + to_string((int)round(wi.ThresholdSetting.value[0])) + "-" 
                + to_string((int)round(wi.ThresholdSetting.value[1])) + " " + wi.Data.unit;
            lowert_label = "between" + to_string((int)round(wi.ThresholdSetting.value[0])) + "-" 
                + to_string((int)round(wi.ThresholdSetting.value[1])) + " " + wi.Data.unit;
            break;
        case 7:
            overt_label = "Offline";
            lowert_label = "Online";
            break;
        case 8:
            overt_label = "between " + to_string((int)round(wi.ThresholdSetting.value[1])) + "-" 
                + to_string((int)round(wi.ThresholdSetting.value[0])) + " " + wi.Data.unit;
            lowert_label = "besides " + to_string((int)round(wi.ThresholdSetting.value[1])) + "-" 
                + to_string((int)round(wi.ThresholdSetting.value[0])) + " " + wi.Data.unit;
            break;
    }

    LOG_TRACE << "label[0]:" << lowert_label;
    LOG_TRACE << "label[1]:" << overt_label;

    wi.Setting.Label.push_back(lowert_label);
    wi.Setting.Label.push_back(overt_label);
}

vector<unsigned> WidgetCalculator::CaculateDataForPercentage(WidgetInfoTemplate& wi, vector<DevData>& data, vector<unsigned>& store_detail_index)
{
    vector<unsigned> ret(wi.Setting.Divider.Percentage.size() + 1);
    
    for (const auto& d : data) {
        if (IsAnyNullType(d.value))
            continue;

        double data_f = GetDoubleFromAny(d.value);

        for (size_t i = 0; i < wi.Setting.Divider.Percentage.size(); i++) {
            if (data_f >= wi.Setting.Divider.Percentage[i]) {
                ret[i] += 1;
                store_detail_index.push_back(i);
                break;
            } else if (i == (wi.Setting.Divider.Percentage.size() - 1)) {
                ret[i + 1] += 1;
                store_detail_index.push_back(i + 1);
            }
        }
    }

    return ret;
}

bool WidgetCalculator::IsAnyNullType(const boost::any& value)
{
    if (value.type() == typeid(NULL))
        return true;
    return false;
}

double WidgetCalculator::GetDoubleFromAny(const boost::any& value)
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

vector<DevData> WidgetCalculator::GetDevDataFromDB(const vector<DevInfo>& device_info_list, list<string> qpath_list)
{
    DataParser data_parser;
    vector<DevData> ret;
    DevData dev_data_item;
	
    if (qpath_list.size() == 0)
        return ret;
    
    string collection_prefix = qpath_list.front();
	transform(collection_prefix.begin(), collection_prefix.end(), collection_prefix.begin(), ::tolower);
	qpath_list.pop_front();

	int data_case = CheckDataCase(qpath_list);
	
	if (collection_prefix.compare("dynamic") == 0 || collection_prefix.compare("static") == 0) {
		for (auto dit = device_info_list.begin(); dit != device_info_list.end(); ++dit) {
			string collection_name = dit->dev_name + "-" + collection_prefix;
			string db_data_temp;
			int result = QueryLatestDataFromDB(collection_name, db_data_temp);
			
			if ((result != 1) || db_data_temp.empty())
                return ret;
            
            vector<string> db_data_list;
			Json::Value docobj;
			Json::Reader reader;

			if (!reader.parse(db_data_temp, docobj)) {
				LOG_ERROR << "Failed to parse raw data." << reader.getFormattedErrorMessages();
				return ret;
			}

			if (data_case == ParInfo) { // parition case
				auto findit = find(qpath_list.begin(), qpath_list.end(), "Capacity");
				if (findit != qpath_list.end()) {
					for (auto sit = dit->storage_list.begin(); sit != dit->storage_list.end(); ++sit) {
                        for (Json::ArrayIndex i = 0; i < docobj["Storage"].size(); i++) {
                            if (docobj["Storage"][i]["SN"].asString().compare(sit->first) == 0) {
                                for (int pid = 0; pid < sit->second; pid++) {
                                    int capacity = docobj["Storage"][i]["Par"]["ParInfo"][pid]["Capacity"].asInt();
                                    dev_data_item = SetDevData(dit->dev_name, capacity, docobj["time"].asInt());
                                    ret.push_back(dev_data_item);
                                }
                            }
                        }
					}
				}
				continue;
			}
			if (data_case == ParInfo)
				return ret;

			if (!qpath_list.front().compare("Storage")) {
                for (auto sit = dit->storage_list.begin(); sit != dit->storage_list.end(); ++sit) {
                    for (Json::ArrayIndex i = 0; i < docobj["Storage"].size(); i++) {
                        if (docobj["Storage"][i]["SN"].asString().compare(sit->first) == 0) {
                            ValueTimePair value_time_pair = data_parser.ParseValueFromDB(docobj["Storage"][i].toStyledString(), qpath_list);
                            dev_data_item = SetDevData(dit->dev_name, value_time_pair.first, value_time_pair.second);
                            ret.push_back(dev_data_item);
                            break;
                        }
                    }
                }
            } else {
                ValueTimePair value_time_pair = data_parser.ParseValueFromDB(db_data_temp, qpath_list);
				dev_data_item = SetDevData(dit->dev_name, value_time_pair.first, value_time_pair.second);
                ret.push_back(dev_data_item);
            }
        }
    }

	if (collection_prefix.compare("storageanalyzer") == 0) {
		for (auto dlit = device_info_list.begin(); dlit != device_info_list.end(); ++dlit) {
            for (auto sit = dlit->storage_list.begin(); sit != dlit->storage_list.end(); ++sit) {
				string SN = sit->first;
				string db_data_temp;
				QueryAnalyzerDataFromDB(SN, db_data_temp);

				if (db_data_temp.empty()) {
                    dev_data_item = SetDevData(SN, NULL, 0);
                    ret.push_back(dev_data_item);
					continue;
				}
                
                ValueTimePair value_time_pair = data_parser.ParseValueFromDB(db_data_temp, qpath_list);
				dev_data_item = SetDevData(SN, value_time_pair.first, value_time_pair.second);
                ret.push_back(dev_data_item);
			}
		}
	}

	if ((collection_prefix.compare("redis") == 0) && (qpath_list.size() > 0)) {
		int db_index;

        if (qpath_list.front().length() > 2)
            db_index = stoi(qpath_list.front().erase(0, 2)); // erase "DB"
        
        for (auto dlit = device_info_list.begin(); dlit != device_info_list.end(); ++dlit) {
            int value;
            if (db_index == 1) { // device status
                value = GetDevStatus(db_index, dlit->dev_name);
                dev_data_item = SetDevData(dlit->dev_name, value, static_cast<int>(time(nullptr)));
                ret.push_back(dev_data_item);
            } else if (db_index == 2) { // innoAge status
                for (auto iait = dlit->storage_list.begin(); iait != dlit->storage_list.end(); ++ iait) {
                    value = GetDevStatus(db_index, iait->first);
                    dev_data_item = SetDevData(iait->first, value, static_cast<int>(time(nullptr)));
                    ret.push_back(dev_data_item);
                }
            }
        }
	}

	return ret;
}

int WidgetCalculator::GetDevStatus(const int& db_index, const string& dev_name)
{
    int status = 0;
    
    RedisCommu redis_commu;
    
    if (redis_commu.Connect() == 0) {
        redis_commu.Select(db_index);

        string ext = "exists " + dev_name;
        string key = "get " + dev_name;
                
        redisReply *reply = redis_commu.Command(ext);
        if (reply->integer == 1) {
            reply = redis_commu.Command(key);
            if (!string(reply->str).compare("1"))
                status = 1;
            else
                status = 0;
        } else {
            LOG_WARN << dev_name << " : No this key";
            status = 0;
        }

        freeReplyObject(reply);
    }

    return status;
}

DevData WidgetCalculator::SetDevData(const string& dev_name, const boost::any& value, const int& time)
{
    DevData dev_data;
    dev_data.dev_name = dev_name;
    dev_data.value = value;
    dev_data.time = time;
    return dev_data;
}

void WidgetCalculator::PrintData(const vector<DevData>& data)
{
	LOG_TRACE << "[data]";
    
	for (auto dit = data.begin(); dit != data.end(); dit++) {
        string log;
		log += dit->dev_name + " ";
		if (dit->value.type() == typeid(int)) {
			log += to_string(boost::any_cast<int>(dit->value));
		} else if (dit->value.type() == typeid(double)) {
			log += to_string(boost::any_cast<double>(dit->value));
		} else if (dit->value.type() == typeid(pair<double, double>)) {	
			pair<double, double> pair_tmp = boost::any_cast<pair<double, double>>(dit->value);
			log += to_string(pair_tmp.first) + " " + to_string(pair_tmp.second);
		} else if (dit->value.type() == typeid(NULL)) {
			log += "NULL";
		}
        LOG_TRACE << log;
    }
}

void WidgetCalculator::PrintQpath(const list<string>& qpath)
{
    string log;
    for (const auto& item : qpath)
        log += item + " ";
    LOG_TRACE << log;
}

vector<unsigned> WidgetCalculator::CaculateDataForBoolean(const WidgetInfoTemplate& wi, const vector<DevData>& data, vector<unsigned>& store_detail_index)
{
    vector<unsigned> ret(2);
    
    int status = wi.Setting.Divider.Boolen.front();

    for (const auto& d : data) {
        auto value = boost::any_cast<int>(d.value);
        if (value == status) {
            ret[0]++;
            store_detail_index.push_back(0);
        } else {
            ret[1]++;
            store_detail_index.push_back(1);
        }
    }

    return ret;
}

vector<unsigned> WidgetCalculator::CaculateDataForNumerical(const WidgetInfoTemplate& wi, const vector<DevData>& data, vector<unsigned>& store_detail_index)
{
    vector<unsigned> ret(wi.Setting.Divider.Number.size() + 1);
    
    for (auto dit = data.begin(); dit != data.end(); dit++) {
        if (IsAnyNullType(dit->value)) {
            store_detail_index.push_back(-1);
            continue;
        }
        double value_f = GetDoubleFromAny(dit->value);

        for (size_t i = 0; i < wi.Setting.Divider.Number.size(); i++) {
            if (value_f >= wi.Setting.Divider.Number[i]) {
                if (i == 0) {
                    ret[0]++;
                    store_detail_index.push_back(0);
                } else {
                    ret[i]++;
                    store_detail_index.push_back(i);
                }
                break;
            } else if (i == (wi.Setting.Divider.Number.size() - 1)) {
                ret[i + 1]++;
                store_detail_index.push_back(i + 1);
            }
        }
    }

    return ret;
}

constexpr unsigned int string2int(const char* str, int h = 0)
{
    // DJB Hash function
    return !str[h] ? 5381 : (string2int(str, h+1)*33) ^ str[h];
}

void WidgetCalculator::SortStatisDataByChartType(WidgetInfoTemplate& wi, const vector<unsigned>& statis_data, WidgetData* wd)
{
    wd->id = wi.WidgetId;
	wd->name = wi.WidgetName;
	wd->width = wi.WidgetWidth;
	wd->type = wi.ChartType;
	wd->file_path = wi.Setting.FilePath;
    
    transform(wi.ChartType.begin(), wi.ChartType.end(), wi.ChartType.begin(), ::tolower);
    switch(string2int(wi.ChartType.c_str())) {
        case string2int("bar"):
        case string2int("doughnut"):
        case string2int("pie"):
        case string2int("text"):
        case string2int("line"):
			LOG_TRACE << "char typte: bar/doughnut/pie/text/line:";
            SetWidgetLabelValue(wi, statis_data, wd);
            break;
        case string2int("google map"):
		case string2int("open street map"):
		case string2int("gaode map"):
			LOG_TRACE << "char typte: google map/open street map/gaode map";
            SetMapWidget(wi, wd);
            break;
        case string2int("customized map"):
			LOG_TRACE << "customized map:";
            SetCustomMapWidget(wi, wd);
            break;
        case string2int("vector map"):
			LOG_TRACE << "vector map:";
            SetVectorMapWidget(wi, wd);
            break;
        default:
            break;
    }
}

void WidgetCalculator::SetWidgetLabelValue(WidgetInfoTemplate& wi, const vector<unsigned>& statis_data, WidgetData* wd)
{
    wd->data.label.assign(wi.Setting.Label.begin(), wi.Setting.Label.end());
    wd->data.value.assign(statis_data.begin(), statis_data.end());
}

void WidgetCalculator::SetMapWidget(const WidgetInfoTemplate& wi, WidgetData* wd)
{
    Json::Value raw;
    Json::Reader reader;
	Json::FastWriter fastWriter;
    
    for (auto it = wi.DeviceList.begin(); it != wi.DeviceList.end(); ++it) {
        string collection = it->name + "-static";
        string static_data;
        int result = QueryLatestDataFromDB(collection, static_data);

        WidgetData::MapItemTemplate map_item;

        if ((result == 1) && !static_data.empty()) {
            map_item.devices_info.name = it->name;
            map_item.devices_info.alias = it->alias;
            map_item.devices_info.owner = it->owner;
            
            reader.parse(static_data, raw);
            map_item.position.lat = raw["Sys"]["Latitude"].asDouble();
            map_item.position.lng = raw["Sys"]["Longitude"].asDouble();
            
            string event_str;
		    int have_event = QueryLatestEventByDevNameFromDB(it->name, event_str);
            if (have_event == 0) {
                map_item.devices_info.color = "green";
                map_item.devices_info.status = "NORMAL";
                map_item.devices_info.detail = "There is no event of this device.";
                map_item.devices_info.time = 0;
            } else if (have_event == 1) {
                Json::Value event;
				reader.parse(event_str, event);
                map_item.devices_info.color = "red";
                map_item.devices_info.status = "WARNING";
                map_item.devices_info.detail = fastWriter.write(event["Message"]);
                map_item.devices_info.detail.erase(remove(map_item.devices_info.detail.begin(),
				    map_item.devices_info.detail.end(), '\n'), map_item.devices_info.detail.end());
				map_item.devices_info.detail.erase(remove(map_item.devices_info.detail.begin(),
					map_item.devices_info.detail.end(), '"'), map_item.devices_info.detail.end());						
				map_item.devices_info.time = event["Time"].asInt();
            }
        } else {
            continue;
        }
        wd->map.value.push_back(map_item);
    }

    wd->map.center_lng = wi.Setting.Lng;
    wd->map.center_lat = wi.Setting.Lat;
}

void WidgetCalculator::SetCustomMapWidget(const WidgetInfoTemplate& wi, WidgetData* wd)
{
    Json::Value event_obj;
    Json::Reader reader;
    Json::FastWriter fastWriter;

    for (auto marker_it = wi.MarkerList.begin(); marker_it != wi.MarkerList.end(); ++marker_it) {
        WidgetData::PanelItemCustomizedMarkerTemplate marker;

        marker.x = marker_it->x;
        marker.y = marker_it->y;
        marker.name = marker_it->name;
        marker.is_normal = true;
        
        for (auto dev_it = marker_it->devices.begin(); dev_it != marker_it->devices.end(); ++dev_it) {
            WidgetData::DeviceInfoTemplate device;
            device.alias = dev_it->alias;
            device.name = dev_it->name;
            device.owner = dev_it->owner;
            
            string event;
            int have_enent = QueryLatestEventByDevNameFromDB(dev_it->name, event);
            if (have_enent == 0) {
                device.detail = "There is no event of this device.";
                device.status = "NORMAL";
                device.time = 0;
            } else if (have_enent == 1) {
                marker.is_normal = false;
                reader.parse(event, event_obj);
                device.detail = fastWriter.write(event_obj["Message"]);
                device.detail.erase(remove(device.detail.begin(), device.detail.end(), '\n'),device.detail.end());
                device.detail.erase(remove(device.detail.begin(),device.detail.end(), '"'), device.detail.end());	
                device.status = "WARNING";
                device.time = event_obj["Time"].asInt();
            }
            marker.devices_info.push_back(device);
        }
        wd->customized_map.push_back(marker);
    }
}

void WidgetCalculator::SetVectorMapWidget(const WidgetInfoTemplate& wi, WidgetData* wd)
{
    Json::Value raw;
    Json::Reader reader;
    vector<pair<double, double>> markers;
    vector<int> device_count;
    vector<int> event_count;
    
    for (auto dit = wi.DeviceList.begin(); dit != wi.DeviceList.end(); ++dit) {
        string collection = dit->name + "-static";
        string static_data;
        int result = QueryLatestDataFromDB(collection, static_data);
        bool found = false;

        if (result == 1 && !static_data.empty()) {
            reader.parse(static_data, raw);
            pair<double, double> pos = make_pair(raw["Sys"]["Latitude"].asDouble(), raw["Sys"]["Longitude"].asDouble());

            // get marker list
            int markers_index = 0;
            for (auto mit = markers.begin(); mit != markers.end(); ++mit) {
                if ((fabs(mit->first - pos.first) < 0.1 && fabs(mit->second - pos.second) < 0.1)) {
                    found = true;
                    markers_index = distance(markers.begin(), mit);
                    break;
                }
            }
            
            string event_str;
            int have_event = QueryLatestEventByDevNameFromDB(dit->name, event_str);
            if (!found) {
                markers.push_back(pos);
                device_count.push_back(1);
                if (have_event == 0)
                    event_count.push_back(0);
                else
                    event_count.push_back(1);
            } else {
                device_count.at(markers_index) += 1;
                if (have_event == 1)
                    event_count.at(markers_index) += 1;
            }
        }
    }

#ifdef DEBUG
    LOG_TRACE << "[markers]";
    for (auto mit = markers.begin(); mit != markers.end(); ++ mit)
        cout << distance(markers.begin(), mit) << ": " << mit->first << " " << mit->second << endl;
    LOG_TRACE << "[device_count]";
    for (auto dcit = device_count.begin(); dcit != device_count.end(); ++ dcit)
        cout << distance(device_count.begin(), dcit) << ": " << *dcit << endl;
    LOG_TRACE << "[event_count]";
    for (auto ecit = event_count.begin(); ecit != event_count.end(); ++ ecit)
        cout << distance(event_count.begin(), ecit) << ": " << *ecit << endl;
#endif
    
    wd->vectormap.mapIndex = wi.Setting.MapIndex;
    for (auto mit = markers.begin(); mit != markers.end(); ++mit) {
        WidgetData::PanelItemVectorMapTemplate::Markers marker_item;
        marker_item.latLng.push_back(mit->first);
        marker_item.latLng.push_back(mit->second);
        marker_item.style.r = 6;
        if (event_count[distance(markers.begin(), mit)] == 0)
            marker_item.style.fill = "green";
        else
            marker_item.style.fill = "red";
        wd->vectormap.markers.push_back(marker_item);
    }
    wd->vectormap.dev_cnt.assign(device_count.begin(), device_count.end());
    wd->vectormap.event_cnt.assign(event_count.begin(), event_count.end());
}

void WidgetCalculator::SetWidgetDetail(const WidgetInfoTemplate& wi, const vector<DevData>& data, 
    const vector<unsigned>& store_detail_index, WidgetData* wd)
{
    if (wi.Setting.Func >= 5) // There're no widget detail for map category
        return;

    /* Set label for widget detail */
    wd->detailWidget.item.push_back("Device Name");
    if (CheckDataCase(wi.Data.qpath_list) >= Storage) { // storage or partion case
        wd->detailWidget.item.push_back("Storage Serial Number");
    }
    if (wi.Setting.Divider.DenominatorId == 0) {
        if (wi.Data.unit.empty())
            wd->detailWidget.item.push_back(wi.Data.name);
        else
            wd->detailWidget.item.push_back(wi.Data.name + '(' + wi.Data.unit + ')');
    } else {
        if (wi.Data.unit.empty())
            wd->detailWidget.item.push_back(wi.Data.name + '/' + wi.Denominator.name);
        else
            wd->detailWidget.item.push_back(wi.Data.name + '(' + wi.Data.unit + ")/" + wi.Denominator.name + '(' + wi.Denominator.unit + ')');
    }
    wd->detailWidget.item.push_back("Owner");
    wd->detailWidget.item.push_back("Time");

    /* Insert recored */
    if (data.size() == 0)
        return;
    
    wd->detailWidget.record_array.resize(wi.Setting.Label.size());

    int data_index = 0;
    for (auto dit = wi.dev_info_list.begin(); dit != wi.dev_info_list.end(); ++dit) {
        int dev_index = distance(wi.dev_info_list.begin(), dit);
        if (CheckDataCase(wi.Data.qpath_list) == ParInfo) {
            for (auto sit = dit->storage_list.begin(); sit != dit->storage_list.end(); ++sit) {
                for (int par_index = 0; par_index < sit->second; par_index++) {
                    if (IsAnyNullType(data[data_index].value)) {
                        data_index++;
                        continue;
                    }

                    WidgetData::DetailWidgetTemplate::Record record;
                    record.dev_name = dit->dev_name;
                    record.alias = wi.DeviceList[dev_index].alias;
                    record.owner_name = wi.DeviceList[dev_index].owner;
                    record.storage_sn = sit->first + "[" + to_string(par_index) + "]";
                    if (record.alias.empty())
                        record.name = "<button name = \"device-link\" class = \"btn-link\" data-button = '{\"devName\":\"" 
                            + record.dev_name + "\"}'>" + record.dev_name + "</button>";
                    else
                        record.name = "<button name = \"device-link\" class = \"btn-link\" data-button = '{\"devName\":\"" 
                            + record.dev_name + "\"}'>" + record.alias + "</button>";
                   
                    if (data[data_index].value.type() == typeid(int)) {
                        record.value = to_string(boost::any_cast<int>(data[data_index].value));
                    } else if (data[data_index].value.type() == typeid(string)) {
                        record.value = boost::any_cast<std::string>(data[data_index].value);
                    } else if (data[data_index].value.type() == typeid(double)) {
                        stringstream stream;
                        stream << fixed << setprecision(2) << rounding(boost::any_cast<double>(data[data_index].value), 2);
                        record.value = stream.str();
                    }
                    record.time = data[data_index].time;
                    wd->detailWidget.record_array[store_detail_index[data_index]].push_back(record);
                    data_index++;
                }
            } 
        } else if ((CheckDataCase(wi.Data.qpath_list) == Storage) || (CheckDataCase(wi.Data.qpath_list) == InnoAge)) {
            for (auto sit = dit->storage_list.begin(); sit != dit->storage_list.end(); ++sit) {
                if (IsAnyNullType(data[data_index].value)) {
                    data_index++;
                    continue;
                }

                WidgetData::DetailWidgetTemplate::Record record;
                record.dev_name = dit->dev_name;
                record.alias = wi.DeviceList[dev_index].alias;
                record.owner_name = wi.DeviceList[dev_index].owner;
                record.storage_sn = sit->first;
                if (record.alias.empty())
                    record.name = "<button name = \"device-link\" class = \"btn-link\" data-button = '{\"devName\":\"" 
                        + record.dev_name + "\"}'>" + record.dev_name + "</button>";
                else
                    record.name = "<button name = \"device-link\" class = \"btn-link\" data-button = '{\"devName\":\"" 
                        + record.dev_name + "\"}'>" + record.alias + "</button>";
               
                if (data[data_index].value.type() == typeid(int)) {
                    record.value = to_string(boost::any_cast<int>(data[data_index].value));
                } else if (data[data_index].value.type() == typeid(string)) {
                    record.value = boost::any_cast<std::string>(data[data_index].value);
                } else if (data[data_index].value.type() == typeid(double)) {
                    stringstream stream;
                    stream << fixed << setprecision(2) << rounding(boost::any_cast<double>(data[data_index].value), 2);
                    record.value = stream.str();
                }

                if (find(wi.Data.qpath_list.begin(), wi.Data.qpath_list.end(), "Redis") != wi.Data.qpath_list.end()) {
                    if (record.value == "1")
                        record.value = "Online";
                    else if (record.value == "0")
                        record.value = "Offline";
                }
                
                record.time = data[data_index].time;
                wd->detailWidget.record_array[store_detail_index[data_index]].push_back(record);
                data_index++;
            }
        } else {
            if (IsAnyNullType(data[data_index].value)) {
                data_index++;
                continue;
            }

            WidgetData::DetailWidgetTemplate::Record record;
            record.dev_name = dit->dev_name;
            record.alias = wi.DeviceList[dev_index].alias;
            record.owner_name = wi.DeviceList[dev_index].owner;
            if (record.alias.empty())
                record.name = "<button name = \"device-link\" class = \"btn-link\" data-button = '{\"devName\":\"" 
                    + record.dev_name + "\"}'>" + record.dev_name + "</button>";
            else
                record.name = "<button name = \"device-link\" class = \"btn-link\" data-button = '{\"devName\":\"" 
                    + record.dev_name + "\"}'>" + record.alias + "</button>";
         
            if (data[data_index].value.type() == typeid(int)) {
                record.value = to_string(boost::any_cast<int>(data[data_index].value));
            } else if (data[data_index].value.type() == typeid(string)) {
                record.value = boost::any_cast<std::string>(data[data_index].value);
            } else if (data[data_index].value.type() == typeid(double)) {
                stringstream stream;
                stream << fixed << setprecision(2) << rounding(boost::any_cast<double>(data[data_index].value), 2);
                record.value = stream.str();
            }

            if (find(wi.Data.qpath_list.begin(), wi.Data.qpath_list.end(), "Redis") != wi.Data.qpath_list.end()) {
                if (record.value == "1")
                    record.value = "Online";
                else if (record.value == "0")
                    record.value = "Offline";
            }

            record.time = data[data_index].time;
            wd->detailWidget.record_array[store_detail_index[data_index]].push_back(record);
            data_index++;
        }
    }
}