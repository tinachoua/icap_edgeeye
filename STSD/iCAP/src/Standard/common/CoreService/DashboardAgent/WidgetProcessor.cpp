#include <thread>
#include <jsoncpp/json/json.h>
#include "Logger.hpp"
#include "WebCommand.hpp"
#include "WidgetCalculator.hpp"
#include "WidgetProcessor.hpp"

using namespace std;

void WidgetProcessor::CalculateWidgets(bool is_high_priority)
{
	string setting_str;
	if (is_high_priority)
		setting_str = GetHighPriorityWidgetsSettingFromWeb();
	else
		setting_str = GetWidgetsSettingFromWeb();
	
	if (setting_str.empty())
		return;
	
	LOG_DEBUG << setting_str;
	
	vector<WidgetInfoTemplate> *widgets_info_ptr = ParseWidgetsSetting(setting_str);
#ifdef DEBUG
	LOG_TRACE << "[WidgetSetting]";
	for (auto widget_it = (*widgets_info_ptr).begin(); widget_it != (*widgets_info_ptr).end(); ++widget_it) {
		cout << (int)distance((*widgets_info_ptr).begin(), widget_it) << endl;
		cout << "+--WidgetId:" << widget_it->WidgetId << endl;
		cout << "+--WidgetName:" << widget_it->WidgetName << endl;
		cout << "+--WidgetWidth:" << widget_it->WidgetWidth << endl;
		cout << "+--ChartType:" << widget_it->ChartType << endl;
		cout << "+--Data:" << endl;
		cout << "   +--Name:" << widget_it->Data.name << endl;
		cout << "   +--Path:";
		for (auto it = widget_it->Data.qpath_list.begin(); it != widget_it->Data.qpath_list.end(); ++it)
			cout << *it << ' ';
		cout << endl;
		cout << "   +--Unit:" << widget_it->Data.unit << endl;
		cout << "+--Denominator:" << endl;
		cout << "   +--Name:" << widget_it->Denominator.name << endl;
		cout << "   +--Path:";
		for (auto it = widget_it->Denominator.qpath_list.begin(); it != widget_it->Denominator.qpath_list.end(); ++it)
			cout << *it << ' ';
		cout << endl;
		cout << "   +--Unit:" << widget_it->Denominator.unit << endl;
		cout << "+--Setting:" << endl;
		cout << "   +--Label" << endl;
		for (auto lbit = widget_it->Setting.Label.begin(); lbit != widget_it->Setting.Label.end(); ++lbit)
			cout << "      " << distance(widget_it->Setting.Label.begin(), lbit) << ':' << *lbit << endl;
		cout << "   +--Func:" << widget_it->Setting.Func << endl;
		cout << "   +--Lng:" << widget_it->Setting.Lng << endl;
		cout << "   +--Lat:" << widget_it->Setting.Lat << endl;
		cout << "   +--MapIndex:" << widget_it->Setting.MapIndex << endl;
		cout << "   +--FilePath:" << widget_it->Setting.FilePath << endl;
		cout << "   +--Divider" << endl;
		cout << "      +--Percentage" << endl;
		for (auto ptit = widget_it->Setting.Divider.Percentage.begin(); ptit != widget_it->Setting.Divider.Percentage.end(); ++ptit)
			cout << "         " << distance(widget_it->Setting.Divider.Percentage.begin(), ptit) << ':' << *ptit << endl;	
		cout << "       --DenominatorId:" << widget_it->Setting.Divider.DenominatorId << endl;
		cout << "      +--Boolean" << endl;
		for (auto bit = widget_it->Setting.Divider.Boolen.begin(); bit != widget_it->Setting.Divider.Boolen.end(); ++bit)
			cout << "         " << distance(widget_it->Setting.Divider.Boolen.begin(), bit) << ':' << *bit << endl;
		cout << "      +--Number" << endl;
		for (auto nbit = widget_it->Setting.Divider.Number.begin(); nbit != widget_it->Setting.Divider.Number.end(); ++nbit)
			cout << "         " << distance(widget_it->Setting.Divider.Number.begin(), nbit) << ':' << *nbit << endl;
		cout << "      +--DataName:" << widget_it->Setting.Divider.DataName << endl;
		cout << "      +--Unit:" << widget_it->Setting.Divider.Unit << endl;
		cout << "+--ThresholdSetting" << endl;
		cout << "   Fun:" << widget_it->ThresholdSetting.func << endl;
		cout << "   Value" << endl;
		for (auto vit = widget_it->ThresholdSetting.value.begin(); vit != widget_it->ThresholdSetting.value.end(); ++vit)
			cout << "   "  << distance(widget_it->ThresholdSetting.value.begin(), vit) << ':' << *vit << endl;
		cout << "+--DeviceList" << endl;
		for (auto dlit = widget_it->DeviceList.begin(); dlit != widget_it->DeviceList.end(); ++dlit) {
			cout << "   " << distance(widget_it->DeviceList.begin(), dlit) << ':' << endl;
			cout << "   " << "Name:" << dlit->name << endl;
			cout << "   " << "Alias:" << dlit->alias << endl;
			cout << "   " << "Owner:" << dlit->owner << endl;
		}
		cout << "+--MarkerList" << endl;
		for (auto mlit = widget_it->MarkerList.begin(); mlit != widget_it->MarkerList.end(); ++mlit) {
			cout << "   +--" << distance(widget_it->MarkerList.begin(), mlit) << endl;
			cout << "      X:" << mlit->x << endl;
			cout << "      Y:" << mlit->y << endl;
			cout << "      Name:" << mlit->name << endl;
			cout << "      Devices" << endl;
			for (auto dit = mlit->devices.begin(); dit != mlit->devices.end(); ++dit) {
				cout << "      +--" << distance(mlit->devices.begin(), dit) << endl;
				cout << "         Name:" << dit->name << endl;
				cout << "         Alias:" << dit->alias << endl;
				cout << "         Owner:" << dit->owner << endl;
 			}
		}
		cout << "************************************************" << endl;
	}
#endif
	
	StartCalculationThreads(widgets_info_ptr);
	delete widgets_info_ptr;
}

string WidgetProcessor::GetWidgetsSettingFromWeb()
{
	string ret;
	if (Webcommand_GetWidgetsSetting(ret) != 0)
		LOG_ERROR << "Get widgets setting faild.";
	
	return ret;
}

string WidgetProcessor::GetHighPriorityWidgetsSettingFromWeb()
{
	string ret;
	if (Webcommand_GetHighPriorityWidgetsSetting(ret) != 0)
		LOG_ERROR << "Get high priority widgets setting faild.";
	
	return ret;
}

vector<WidgetInfoTemplate>* WidgetProcessor::ParseWidgetsSetting(const string& setting)
{
	Json::Value setting_obj;
	Json::Reader reader;
	if (!reader.parse(setting, setting_obj)) {
		LOG_ERROR  << "Failed to parse configuration" << endl << reader.getFormattedErrorMessages();
		return NULL;
	}
	
	setting_obj = setting_obj["WidgetSetting"];
	vector<WidgetInfoTemplate>* ret_ptr = new vector<WidgetInfoTemplate>;
	for (Json::Value::ArrayIndex widget_index = 0; widget_index < setting_obj.size(); widget_index++) {
		WidgetInfoTemplate widget_info_obj;
		widget_info_obj.WidgetId = setting_obj[widget_index]["WidgetId"].asInt();
		widget_info_obj.WidgetName = setting_obj[widget_index]["WidgetName"].asString();
		widget_info_obj.WidgetWidth = setting_obj[widget_index]["WidgetWidth"].asInt();
		widget_info_obj.ChartType = setting_obj[widget_index]["ChartType"].asString();
		
		widget_info_obj.Data.name = setting_obj[widget_index]["Data"]["Name"].asString();
		list<string> qpath_list;
		Getqpath(setting_obj[widget_index]["Data"]["Path"].asString(), qpath_list);
		widget_info_obj.Data.qpath_list.assign(qpath_list.begin(), qpath_list.end());
		widget_info_obj.Data.unit = setting_obj[widget_index]["Data"]["Unit"].asString();
		
		qpath_list.clear();
		widget_info_obj.Denominator.name = setting_obj[widget_index]["Denominator"]["Name"].asString();
		Getqpath(setting_obj[widget_index]["Denominator"]["Path"].asString(), qpath_list);
		widget_info_obj.Denominator.qpath_list.assign(qpath_list.begin(), qpath_list.end());
		widget_info_obj.Denominator.unit = setting_obj[widget_index]["Denominator"]["Unit"].asString();
		
		for (Json::Value::ArrayIndex lbid = 0; lbid < setting_obj[widget_index]["Setting"]["Label"].size(); lbid++)
			widget_info_obj.Setting.Label.push_back(setting_obj[widget_index]["Setting"]["Label"][lbid].asString());
		widget_info_obj.Setting.Func = setting_obj[widget_index]["Setting"]["Func"].asInt();
		widget_info_obj.Setting.Lng = setting_obj[widget_index]["Setting"]["Lng"].asDouble();
		widget_info_obj.Setting.Lat = setting_obj[widget_index]["Setting"]["Lat"].asDouble();
		widget_info_obj.Setting.MapIndex = setting_obj[widget_index]["Setting"]["MapIndex"].asInt();
		widget_info_obj.Setting.FilePath = setting_obj[widget_index]["Setting"]["FilePath"].asString();
		for (Json::Value::ArrayIndex ptid = 0; ptid < setting_obj[widget_index]["Setting"]["Divider"]["Percentage"].size(); ptid++)
			widget_info_obj.Setting.Divider.Percentage.push_back(setting_obj[widget_index]["Setting"]["Divider"]["Percentage"][ptid].asInt());
		widget_info_obj.Setting.Divider.DenominatorId = setting_obj[widget_index]["Setting"]["Divider"]["DenominatorId"].asInt();
		for (Json::Value::ArrayIndex blid = 0; blid < setting_obj[widget_index]["Setting"]["Divider"]["Boolean"].size(); blid++)
			widget_info_obj.Setting.Divider.Boolen.push_back(setting_obj[widget_index]["Setting"]["Divider"]["Boolean"][blid].asBool());
		for (Json::Value::ArrayIndex nbid = 0; nbid < setting_obj[widget_index]["Setting"]["Divider"]["Number"].size(); nbid++)
			widget_info_obj.Setting.Divider.Number.push_back(setting_obj[widget_index]["Setting"]["Divider"]["Number"][nbid].asDouble());
		widget_info_obj.Setting.Divider.DataName = setting_obj[widget_index]["Setting"]["Divider"]["DataName"].asString();
		widget_info_obj.Setting.Divider.Unit = setting_obj[widget_index]["Setting"]["Divider"]["Unit"].asString();
		
		widget_info_obj.ThresholdSetting.func = setting_obj[widget_index]["ThresholdSetting"]["Func"].asInt();
		for (Json::Value::ArrayIndex vid = 0; vid < setting_obj[widget_index]["ThresholdSetting"]["Value"].size(); vid++)
			widget_info_obj.ThresholdSetting.value.push_back(setting_obj[widget_index]["ThresholdSetting"]["Value"][vid].asDouble());
		for (Json::Value::ArrayIndex dlid = 0; dlid < setting_obj[widget_index]["Devices"].size(); dlid++) {
			WidgetInfoTemplate::DeviceTemplate device;
			device.name = setting_obj[widget_index]["Devices"][dlid]["Name"].asString();
			device.alias = setting_obj[widget_index]["Devices"][dlid]["Alias"].asString();
			device.owner = setting_obj[widget_index]["Devices"][dlid]["Owner"].asString();
			widget_info_obj.DeviceList.push_back(device);
		}
		for (Json::Value::ArrayIndex mkid = 0; mkid < setting_obj[widget_index]["Markers"].size(); mkid++) {
			WidgetInfoTemplate::MarkerTemplate marker;
			marker.x = setting_obj[widget_index]["Markers"][mkid]["X"].asInt();
			marker.y = setting_obj[widget_index]["Markers"][mkid]["Y"].asInt();
			marker.name = setting_obj[widget_index]["Markers"][mkid]["Name"].asString();
			for (Json::Value::ArrayIndex devid = 0; devid < setting_obj[widget_index]["Markers"][mkid]["Devices"].size(); devid++) {
				WidgetInfoTemplate::DeviceTemplate device;
				device.name = setting_obj[widget_index]["Markers"][mkid]["Devices"][devid]["Name"].asString();
				device.alias = setting_obj[widget_index]["Markers"][mkid]["Devices"][devid]["Alias"].asString();
				device.owner = setting_obj[widget_index]["Markers"][mkid]["Devices"][devid]["Owner"].asString();
				marker.devices.push_back(device);
			}
			widget_info_obj.MarkerList.push_back(marker);
		}
		ret_ptr->push_back(widget_info_obj);
	}
	
	return ret_ptr;
}

int WidgetProcessor::Getqpath(const string& db_path, list<string>& qpath_list) //(Mongodb collection, documents path, number of data you want to get)
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

void WidgetProcessor::StartCalculationThreads(vector<WidgetInfoTemplate>* widgets_info_ptr)
{
	const int thread_count = widgets_info_ptr->size();
	thread calculation_threads[thread_count];

	for (auto it = (*widgets_info_ptr).begin(); it != (*widgets_info_ptr).end(); ++it) {
		int widget_index = distance((*widgets_info_ptr).begin(), it);
		if (widget_index >= thread_count)	break;
		calculation_threads[widget_index] = thread(&WidgetCalculator::CalculateWidget, ref((*widgets_info_ptr)[widget_index]));
		this_thread::sleep_for(chrono::seconds(1));
	}

	for (int i = 0; i < thread_count; ++i)
		calculation_threads[i].join();
}