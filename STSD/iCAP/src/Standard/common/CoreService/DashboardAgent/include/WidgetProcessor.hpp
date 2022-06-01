#ifndef DA_WIDGETPROCESSOR_H_
#define DA_WIDGETPROCESSOR_H_

#include <list>
#include <map>
#include <vector>

struct DevInfo
{
	std::string dev_name;
	std::map<std::string, int> storage_list; // first:S/N, second:partition count
};

struct SettingItemTemplate
{
	struct SettingDividerTemplate
	{
		std::vector<int> Percentage;
		int DenominatorId;
		std::vector<bool> Boolen;
		std::vector<double> Number;
		std::string DataName;
		std::string Unit;
	};
	
	std::vector<std::string> Label;
	int Func;
	double Lng;
	double Lat;
	int MapIndex;
	std::string FilePath;
	SettingDividerTemplate Divider;
};

struct WidgetInfoTemplate
{
	struct DataTemplate
	{
		std::string name;
		std::list<std::string> qpath_list;
		std::string unit;
	};
	struct ThresholdSettingTemplate
	{
		int func;
		std::vector<double> value;
	};
	struct DeviceTemplate
	{
		std::string name;
		std::string alias;
		std::string owner;
	};
	struct MarkerTemplate
	{
		int x;
		int y;
		std::string name;
		std::vector<DeviceTemplate> devices;
	};

	int WidgetId;
	std::string WidgetName;
	int WidgetWidth;
	std::string ChartType;
	DataTemplate Data;
	DataTemplate Denominator;
	SettingItemTemplate Setting;
	ThresholdSettingTemplate ThresholdSetting;
	std::vector<DeviceTemplate> DeviceList;
	std::vector<MarkerTemplate> MarkerList;
	std::vector<DevInfo> dev_info_list;
};

class WidgetProcessor
{
public:
    void CalculateWidgets(bool is_high_priority);

private:
    std::string GetWidgetsSettingFromWeb();
    std::string GetHighPriorityWidgetsSettingFromWeb();
    std::vector<WidgetInfoTemplate>* ParseWidgetsSetting(const std::string& setting);
    int Getqpath(const std::string& db_path, std::list<std::string>& qpath_list);
    void StartCalculationThreads(std::vector<WidgetInfoTemplate>* widgets_info_ptr);
};

#endif // DA_WIDGETPROCESSOR_H_