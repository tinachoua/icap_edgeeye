#ifndef DA_WIDGETCALCULATOR_H_
#define DA_WIDGETCALCULATOR_H_

#include "WidgetProcessor.hpp"

struct DevData
{
    std::string dev_name; // device_name or storage S/N
    boost::any value;
    int time;
};

struct WidgetData
{
	struct PanelItemDataTemplate
	{
		std::vector<std::string> label;
		std::vector<unsigned> value;
	};
	struct DeviceInfoTemplate
	{
		std::string name;
		std::string alias;
		std::string color;
		std::string status;
		std::string owner;
		std::string detail;
		long long time;
	};
	struct MapItemTemplate
	{
		struct Pos
		{
			double lat;
			double lng;
		};
	
		DeviceInfoTemplate devices_info;
		Pos position;
	};
	struct PanelItemMapTemplate
	{
		double center_lng;
		double center_lat;
		std::vector<MapItemTemplate> value;
	};

	struct PanelItemVectorMapTemplate
	{
		struct Markers
		{
			struct Style
			{
				int r;
				std::string fill;
			};

			std::vector<double> latLng;
			Style style;
		};

		int mapIndex;
		std::vector<Markers> markers;
		std::vector<int> dev_cnt;
		std::vector<int> event_cnt;
	};
	
	struct DetailWidgetTemplate
	{
		struct Record
		{
			std::string name;
			std::string dev_name;
			std::string storage_sn;
			std::string value;
			std::string owner_name;
			std::string alias;
			long long time;
		};
		
		std::vector<std::string> item;
		std::vector<std::vector<Record>> record_array;
	};

	struct PanelItemCustomizedMarkerTemplate
	{
		int x;
		int y;
		bool is_normal; // true: no event; false: have events
		std::string name;
		std::vector<DeviceInfoTemplate> devices_info;
	};
	
	int id;
	std::string name;
	int width;
	std::string type;
	std::string file_path;
	PanelItemDataTemplate data;
	PanelItemMapTemplate map; //google map and open street map
	PanelItemVectorMapTemplate vectormap;
	std::vector<PanelItemCustomizedMarkerTemplate> customized_map;
	DetailWidgetTemplate detailWidget;
};

class WidgetCalculator
{
public:
    WidgetCalculator();
    ~WidgetCalculator();
    static void CalculateWidget(WidgetInfoTemplate& widget_info);

private:
    static void GetDeviceInfoList(WidgetInfoTemplate& widget_info);
    static int Getqpath(const std::string& db_path, std::list<std::string>& qpath_list);
    static int CheckDataCase(const std::list<std::string>& qpath_list);
    static std::map<std::string, int> GetStorageList(const std::string& dev_name, const bool& check_is_innoage = false);
    static WidgetData* GetWidgetData(WidgetInfoTemplate wi);
	static std::vector<DevData> GetData(WidgetInfoTemplate wi);
    static std::vector<DevData> GetDevDataFromDB(const std::vector<DevInfo>& device_info_list,
		std::list<std::string> qpath_list);
	static int GetDevStatus(const int& db_index, const std::string& dev_name);
    static DevData SetDevData(const std::string& dev_name, const boost::any& value, const int& time);
	static void PrintData(const std::vector<DevData>& data);
	static void PrintQpath(const std::list<std::string>& qpath);
    static std::vector<unsigned> CaculateData(WidgetInfoTemplate& wi, std::vector<DevData>& data,
		std::vector<unsigned>& store_detail_index);
    static std::vector<unsigned> CaculateDataForTH(WidgetInfoTemplate& wi, const std::vector<DevData>& data,
		std::vector<unsigned>& store_detail_index);
    static void ProcessCounterForTHSetting(bool is_overth, unsigned& counter, std::vector<unsigned>& store_detail_index);
    static void GenerateLabelForTHSetting(WidgetInfoTemplate& wi);
    static std::vector<unsigned> CaculateDataForPercentage(WidgetInfoTemplate& wi, std::vector<DevData>& data,
		std::vector<unsigned>& store_detail_index);
	static bool IsAnyNullType(const boost::any& value);
	static double GetDoubleFromAny(const boost::any& value);
	static std::vector<unsigned> CaculateDataForBoolean(const WidgetInfoTemplate& wi, const std::vector<DevData>& data,
		std::vector<unsigned>& store_detail_index);
	static std::vector<unsigned> CaculateDataForNumerical(const WidgetInfoTemplate& wi, const std::vector<DevData>& data,
		std::vector<unsigned>& store_detail_index);
	static void SortStatisDataByChartType(WidgetInfoTemplate& wi, const std::vector<unsigned>& statis_data,
		WidgetData* wd);
	static void SetWidgetLabelValue(WidgetInfoTemplate& wi, const std::vector<unsigned>& statis_data, WidgetData* wd);
	static void SetMapWidget(const WidgetInfoTemplate& wi, WidgetData* wd);
	static void SetCustomMapWidget(const WidgetInfoTemplate& wi, WidgetData* wd);
	static void SetVectorMapWidget(const WidgetInfoTemplate& wi, WidgetData* wd);
	static void SetWidgetDetail(const WidgetInfoTemplate& wi, const std::vector<DevData>& data, 
		const std::vector<unsigned>& store_detail_index, WidgetData* wd);
};

#endif // DA_WIDGETCALCULATOR_H_