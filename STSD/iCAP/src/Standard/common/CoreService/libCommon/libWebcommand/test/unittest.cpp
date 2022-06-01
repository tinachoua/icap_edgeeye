#include <string>
#include <gtest/gtest.h>
#include <jsoncpp/json/json.h>
#include "WebCommand.hpp"

using namespace std;

namespace {
	TEST(WebcommandLibTest, AuthenticationAPITest)
	{
		string token;
		EXPECT_EQ(0, WebCommand_GetToken(token));
		EXPECT_STRNE("", token.c_str());
	}

	TEST(WebcommandLibTest, DeviceAPITest)
	{
		string id = "Device00001";
		string alias = "{\"Alias\":\"test\"}";
		int status;
		int response;
		EXPECT_EQ(0, WebCommand_UpdateDeviceStatus(id, 0, response)); // set offline
		EXPECT_EQ(0, response);
		EXPECT_EQ(0, WebCommand_GetDeviceStatus(id, status));
		EXPECT_EQ(0, status);
		EXPECT_EQ(0, WebCommand_UpdateDeviceStatus(id, 1, response)); // set online
		EXPECT_EQ(1, response);
		EXPECT_EQ(0, WebCommand_GetDeviceStatus(id, status));
		EXPECT_EQ(1, status);
		string device_list;
		EXPECT_EQ(0, WebCommand_GetDeviceList(device_list));
		EXPECT_STRNE("", device_list.c_str());
		EXPECT_EQ(0, WebCommand_UpdateDeviceAlias(id, alias));
		string innoage_list;
		EXPECT_EQ(0, WebCommand_GetInnoAgeList(innoage_list));
		EXPECT_STRNE("", device_list.c_str());
	}

	TEST(WebcommandLibTest, DashboardAPITest)
	{
		string response;
		EXPECT_EQ(0, Webcommand_GetEmailSendor(1, response));
		EXPECT_EQ(0, Webcommand_GetWidgetsSetting(response));
		EXPECT_EQ(0, Webcommand_GetHighPriorityWidgetsSetting(response));
		EXPECT_EQ(0, Webcommand_GetTHSetting(response));
	}

	TEST(WebcommandLibTest, InnoAgeAPITest)
	{
		string response;
		EXPECT_EQ(0, WebCommand_GetInnoAgeStatus("B0011905300270085", response));
		EXPECT_STREQ("1", response.c_str());
	}
}

int main(int argc, char** argv)  
{
    testing::InitGoogleTest(&argc, argv); 
    return RUN_ALL_TESTS();
}