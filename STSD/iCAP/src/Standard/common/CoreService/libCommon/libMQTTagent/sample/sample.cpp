#include <chrono>
#include <csignal>
#include <iostream>
#include <string>
#include <thread>
#include <vector>
#include <jsoncpp/json/json.h>
#include "MQTTAgent.hpp"

using namespace std;

const string server_url = "tcp://172.30.0.4:1883";
const char* CLIENTID = "SAMPLE";
const char* topic = "SAMPLE_TOPIC";
const int Qos = 1;
const mqttsub_rule sub_rule = make_pair(topic, Qos);
extern const char *mqtt_client_id;
static char onExit = 0;

void gotExitCmd(int sig)
{
	(void)sig;
	onExit = 1;
}

void MqttMessageReceiver(const std::string& topic, const std::string& message)
{
	(void)topic;
	(void)(message);
}

int main(void)
{   
	mqtt_client_id = ("sample-" + to_string(time(NULL))).c_str();;
	Udefined_connect_option opts;
	opts.username = "admin";
	opts.password = "AH0MBwnqi3O-9Dxlt7ZxGHBGsZC5TnEA";
	vector<mqttsub_rule> sub_rule_list;
	sub_rule_list.push_back(sub_rule);
	MQTTAgent_AddSubTopics(opts, sub_rule_list);
	
	if (MQTTAgent_Start(opts, server_url, MqttMessageReceiver) == 0) {
		signal(SIGINT, gotExitCmd);

		int count = 1;
		while (!onExit) {
			string payload = "Publish data, count=" + to_string(count);
			MQTTAgent_PublishData("SAMPLE_TOPIC", payload.c_str(), Qos);
			this_thread::sleep_for(chrono::seconds(3));
			count++;
			break;
		}

		MQTTAgent_Stop();
	}

	MQTTAgent_Destroy();
	
	return 0;
}
