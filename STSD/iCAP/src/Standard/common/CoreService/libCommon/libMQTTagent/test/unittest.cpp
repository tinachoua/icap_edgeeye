#include <iostream>
#include <string>
#include <utility>      // std::pair
#include <vector>
#include <gtest/gtest.h>
#include "MQTTAgent.hpp"

using namespace std;

extern const char *mqtt_client_id;
const char* topic = "TEST_TOPIC";
const int qos = 1;

namespace {
	void DeviceCommand_Handler(const std::string& topic, const std::string& message)
	{
		(void)topic;
		(void)(message);
	}

	TEST(MQTTAgentLibTest, StartMQTT)
	{
		mqtt_client_id = ("test-" + to_string(time(NULL))).c_str();;
		mqttsub_rule sub_rule = make_pair(topic, qos);
		Udefined_connect_option opts;
		opts.username = "admin";
		opts.password = "AH0MBwnqi3O-9Dxlt7ZxGHBGsZC5TnEA";
		vector<mqttsub_rule> sub_rule_list;
		sub_rule_list.push_back(sub_rule);
		MQTTAgent_AddSubTopics(opts, sub_rule_list);
		EXPECT_EQ(1, opts.sub_rule.size());
		string url = "tcp://172.30.0.4:1883";
		EXPECT_EQ(0, MQTTAgent_Start(opts, url, DeviceCommand_Handler));
	}

	TEST(MQTTAgentLibTest, AddSubscribedTopic)
	{
		EXPECT_EQ(0, MQTTAgent_PublishData(topic, "test payload.", 1));
	}
	
	TEST(MQTTAgentLibTest, PublishDataToBroker)
	{
		EXPECT_EQ(0, MQTTAgent_PublishData(topic, "test payload.", 1));
	}

	TEST(MQTTAgentLibTest, EndMQTT)
	{
		EXPECT_EQ(0, MQTTAgent_Stop());
		EXPECT_EQ(0, MQTTAgent_Destroy());
	}
}

int main(int argc, char** argv)  
{
    testing::InitGoogleTest(&argc, argv); 
    return RUN_ALL_TESTS();
}