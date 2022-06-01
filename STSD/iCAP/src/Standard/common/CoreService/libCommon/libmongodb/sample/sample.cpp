#include <iostream>
#include <string>
#include <vector>
#include <jsoncpp/json/json.h>
#include "Nosqlcommand.hpp"

using namespace std;

#define CAPPED_COLL_NAME		"test_capped_coll"
#define COLL_NAME				"test_coll"
#define ANALYZER_COLL_NAME		"StorageAnalyzer"
#define SCREENSHOT_COLL_NAME	"Screenshot"
#define IMG_PATH    			"/var/iCAP/Images/screenshot/"

const string input_string = "{\"Innodisk\":\"Hello Innodisk\"}";
const string input_string2 = "{\"IPA\":\"Hello IPA\"}";
const string input_string3 = "{\"iCAP\":\"Hello iCAP\"}";

int main(void)
{
	cout << "Initialize Mongo database." << endl;
	InitializeMongoDB();

	cout << "Check if the collection " << COLL_NAME << " exists. ";
	cout << "Result:" << CheckCollExists(COLL_NAME) << endl;

	cout << "Insert data to capped collction " << CAPPED_COLL_NAME << "." << endl;
	InsertDataToCappedColl(CAPPED_COLL_NAME, input_string);

	cout << "Get documnet count in collection " << CAPPED_COLL_NAME << "." << endl;
	int doc_count = GetDocCountInColl(CAPPED_COLL_NAME);
	cout << "documnet count:" << doc_count << endl;

	cout << "Insert data to collection " << COLL_NAME << "." << endl;
	InsertDataToDB(COLL_NAME, input_string);
	// Insert more data to test_coll for showing query multiple data.
	InsertDataToDB(COLL_NAME, input_string2);
	InsertDataToDB(COLL_NAME, input_string3);

	cout << "Query the latest data form collection " << COLL_NAME << ". ";
	string get_string;
	QueryLatestDataFromDB(COLL_NAME, get_string);
	cout << "Data:" << endl << "  " << get_string << endl;

	cout << "Query all data form collection " << COLL_NAME << ". ";
	vector<string> get_vecstr;
	QueryDataFromDB(COLL_NAME, 1, 0, get_vecstr);
	cout << "Data:" << endl;
	for (auto it = get_vecstr.begin(); it != get_vecstr.end(); ++it)
		cout << "  " << *it << endl;
	
	cout << "Query the oldest two data form collection " << COLL_NAME << ". Data:" << endl;
	get_vecstr.clear();
	QueryDataFromDB(COLL_NAME, 1, 2, get_vecstr);
	for (auto it = get_vecstr.begin(); it != get_vecstr.end(); ++it)
		cout << "  " << *it << endl;

	cout << "Query the latest two data form collection " << COLL_NAME << ". Data:" << endl;
	get_vecstr.clear();
	QueryDataFromDB(COLL_NAME, -1, 2, get_vecstr);
	for (auto it = get_vecstr.begin(); it != get_vecstr.end(); ++it)
		cout << "  " << *it << endl;
	
	cout << "Drop collections " << CAPPED_COLL_NAME << " and " <<  COLL_NAME << endl;
	DropColl(CAPPED_COLL_NAME);
	DropColl(COLL_NAME);

	cout << "Operate collection StorageAnalyzer." << endl;
	cout << "Insert Initialized data of storage." << endl;
	const string SN = "01234567";
	const double capacity = 238.467908;
	const int initialHealth = 100;
	const int initailTime = 1505260800;
	const int PECycle = 3000;
	Json::Value analyzer_obj;
	analyzer_obj["SN"] = SN;
	analyzer_obj["Capacity"] = capacity;
	analyzer_obj["InitHealth"] = initialHealth;
	analyzer_obj["InitTime"] = initailTime;
	analyzer_obj["PECycle"] = PECycle;
	analyzer_obj["Lifespan"] = Json::Value(Json::arrayValue);
	Json::FastWriter writer;
	string jstr = writer.write(analyzer_obj);
	InsertDataToDB("StorageAnalyzer", jstr);
	cout << "Insert lifespan date of storage." << endl;
	const int time = 1560408193;
	const double health = 99.265306122449;
	const int data = 2432;
	InsertAnalyzerDataToDB(SN, time, health, data);
	cout << "Query data from collection StorageAnalyzer. Data:" << endl;
	QueryAnalyzerDataFromDB(SN, get_string);
	cout << "  " << get_string << endl;
	cout << "Delete data from collection StorageAnalyzer." << endl;
	DeleteAnalyzerDocFromDB(SN);

	cout << "Operate collection Screenshot." << endl;
	cout << "Insert image date." << endl;
	const string devName = "Device_xxxxxxxxx";
	const long long id = 1560408193;
	const string path = IMG_PATH + devName + "/" + devName + "_" + to_string(id) + ".jpg";
	InsertScreenshotDataToDB(devName, id, path);
	cout << "Query image data from collection Screenshot. Data:" << endl;
	QueryScreenshotDataFromDB(devName, get_string);
	cout << "  " << get_string << endl;
	cout << "Delete image data from collection Screenshot." << endl;
	DeleteScreenshotDataFromDB(devName, id);

	return 0;
}