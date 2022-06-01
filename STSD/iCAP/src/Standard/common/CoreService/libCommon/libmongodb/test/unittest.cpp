#include <string>
#include <vector>
#include <gtest/gtest.h>
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

namespace {
	TEST(MongoLibTest, InitializeMongoDB)
	{
		EXPECT_TRUE(InitializeMongoDB()); 
	}

	TEST(MongoLibTest, CappedCollectionTest)
	{
		EXPECT_FALSE(CheckCollExists(CAPPED_COLL_NAME));
		EXPECT_EQ(0, InsertDataToCappedColl(CAPPED_COLL_NAME, input_string));
		EXPECT_TRUE(CheckCollExists(CAPPED_COLL_NAME));
		EXPECT_EQ(1, GetDocCountInColl(CAPPED_COLL_NAME));

		string get_string;
		EXPECT_EQ(0, QueryLatestDataFromDB(CAPPED_COLL_NAME, get_string));
		Json::Value input, get;
		Json::Reader reader;
		reader.parse(input_string, input);
		reader.parse(get_string, get);
		EXPECT_STREQ(input["Innodisk"].asString().c_str(), get["Innodisk"].asString().c_str());
		
		EXPECT_EQ(0, InsertDataToCappedColl(CAPPED_COLL_NAME, input_string2));
		vector<string> get_vecstring;
		EXPECT_EQ(1, QueryDataFromDB(CAPPED_COLL_NAME, -1, 10, get_vecstring));
		reader.parse(input_string2, input);
		reader.parse(get_vecstring[0], get);
		EXPECT_STREQ(input["IPA"].asString().c_str(), get["IPA"].asString().c_str());
		
		EXPECT_TRUE(DropColl(CAPPED_COLL_NAME));
	}

	TEST(MongoLibTest, CollectionTest)
	{
		EXPECT_FALSE(CheckCollExists(COLL_NAME));
		EXPECT_EQ(0, InsertDataToDB(COLL_NAME, input_string));
		EXPECT_TRUE(CheckCollExists(COLL_NAME));
		EXPECT_EQ(1, GetDocCountInColl(COLL_NAME));

		string get_string;
		EXPECT_EQ(0, QueryLatestDataFromDB(COLL_NAME, get_string));
		Json::Value input, get;
		Json::Reader reader;
		reader.parse(input_string, input);
		reader.parse(get_string, get);
		EXPECT_STREQ(input["Innodisk"].asString().c_str(), get["Innodisk"].asString().c_str());

		EXPECT_EQ(0, InsertDataToDB(COLL_NAME, input_string2));
		vector<string> get_vecstring;
		EXPECT_EQ(2, QueryDataFromDB(COLL_NAME, 1, 10, get_vecstring));
		reader.parse(get_vecstring[0], get);
		EXPECT_STREQ(input["Innodisk"].asString().c_str(), get["Innodisk"].asString().c_str());
		reader.parse(input_string2, input);
		reader.parse(get_vecstring[1], get);
		EXPECT_STREQ(input["IPA"].asString().c_str(), get["IPA"].asString().c_str());

		EXPECT_TRUE(DropColl(COLL_NAME));
	}

	TEST(MongoLibTest, AnalyzerCollTest)
	{
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
		EXPECT_EQ(0, InsertDataToDB("StorageAnalyzer", jstr));
		EXPECT_TRUE(CheckCollExists(ANALYZER_COLL_NAME));

		const int time = 1560408193;
		const double health = 99.265306122449;
		const int data = 2432;
		EXPECT_EQ(0, InsertAnalyzerDataToDB(SN, time, health, data));
		
		string get_string;
		EXPECT_EQ(0, QueryAnalyzerDataFromDB(SN, get_string));
		Json::Value get;
		Json::Reader reader;
		reader.parse(get_string, get);
		EXPECT_STREQ(SN.c_str(), get["SN"].asString().c_str());
		EXPECT_EQ(capacity, get["Capacity"].asDouble());
		EXPECT_EQ(initialHealth, get["InitHealth"].asInt());
		EXPECT_EQ(initailTime, get["InitTime"].asInt());
		EXPECT_EQ(PECycle, get["PECycle"].asInt());
		EXPECT_TRUE(get.isMember("Lifespan"));
		EXPECT_EQ(time, get["Lifespan"][0]["time"].asInt());
		EXPECT_EQ(health, get["Lifespan"][0]["health"].asDouble());
		EXPECT_EQ(data, get["Lifespan"][0]["data"].asInt());

		EXPECT_EQ(0, DeleteAnalyzerDocFromDB(SN));
	}

	TEST(MongoLibTest, ScreenshotCollTest)
	{
		const string devName = "Device_xxxxxxxxx";
		const long long id = 1560408193;
		const string path = IMG_PATH + devName + "/" + devName + "_" + to_string(id) + ".jpg";
    	EXPECT_EQ(0, InsertScreenshotDataToDB(devName, id, path));
		EXPECT_TRUE(CheckCollExists(SCREENSHOT_COLL_NAME));

		string get_string;
		EXPECT_EQ(0, QueryScreenshotDataFromDB(devName, get_string));
		Json::Value get;
		Json::Reader reader;
		reader.parse(get_string, get);
		EXPECT_STREQ(devName.c_str(), get["Dev"].asString().c_str());
		EXPECT_TRUE(get.isMember("Images"));
		EXPECT_EQ(id, get["Images"][0]["Id"].asInt64());
		EXPECT_STREQ(path.c_str(), get["Images"][0]["Path"].asString().c_str());
		
		EXPECT_EQ(0, DeleteScreenshotDataFromDB(devName, id));
		EXPECT_EQ(0, QueryScreenshotDataFromDB(devName, get_string));
		reader.parse(get_string, get);
		EXPECT_EQ(0, get["Images"].size());

		EXPECT_EQ(0, DeleteScreenshotDocFromDB(devName));
	}
}

int main(int argc, char** argv)  
{
    testing::InitGoogleTest(&argc, argv); 
    return RUN_ALL_TESTS();
}