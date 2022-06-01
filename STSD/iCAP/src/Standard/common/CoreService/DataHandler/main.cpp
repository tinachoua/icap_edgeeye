#include <atomic>
#include <chrono>
#include <csignal>
#include <iostream>
#include <list>
#include <thread>
#include <utility>
#include <vector>
#include "Commander.hpp"
#include "DeviceCommand.hpp"
#include "Logger.hpp"
#include "main.hpp"
#include "MQTTAgent.hpp"
#include "Nosqlcommand.hpp"
#include "SocketAgent.hpp"

using namespace std;

#define UNUSED(x) (void)(x)

atomic<bool> is_exit { false }, running { true };
const string mqtt_server_url = "tcp://172.30.0.4:1883";
const int Qos = 1;
const vector<mqttsub_rule> sub_rule_list = { make_pair("RawData", Qos), 
											 make_pair("ActiveMsg", Qos) };
extern const char *mqtt_client_id;

const IP_PORT_SET ip_port_set = {
    {"172.30.0.2", "27017"},	// MongoDB
    {"172.30.0.4", "1883"},		// MQTT
	{"172.30.0.6", "50000"},	// auth api
	{"172.30.0.7", "52000"}		// device api
};

void gotExitCmd(int sig);

int main(void)
{
	BOOST_LOG_NAMED_SCOPE(SCOPE_NAME);
	signal(SIGINT, gotExitCmd);
	LOG_INFO << "Start iCAP_CoreService_DataHandler program.";
	LOG_INFO << "Please press Ctrl-C to exit.";

	if (!CheckPort(ip_port_set)) {
		LOG_WARN << "The dependent containers are not ready yet.";
		return -1;
	}
	
	mqtt_client_id = ("DH-" + to_string(time(NULL))).c_str();;
	Udefined_connect_option opts;
	opts.username = "admin";
	opts.password = "AH0MBwnqi3O-9Dxlt7ZxGHBGsZC5TnEA";
	MQTTAgent_AddSubTopics(opts, sub_rule_list);

	if (MQTTAgent_Start(opts, mqtt_server_url, MqttMessageReceiver) == 0) {
		LOG_INFO << "Connect to iCAP service gateway successfully.";

		if (InitializeMongoDB()) {
			LOG_INFO << "Initializing the Data DB successfully.";
		} else {
			LOG_FATAL << "Initializing the Data DB failed.";
			return -1;
		}

		// Create a socket thread to check whether the screenshot command
		thread cmd_listener_thread(CommandListener);

		while (!is_exit.load(memory_order_relaxed))
			this_thread::sleep_for(chrono::seconds(3));

		running.store(false, memory_order_relaxed);
		cmd_listener_thread.join();
		
		MQTTAgent_Stop();
		LOG_INFO << "Disconnect from iCAP service gateway successfully.";
	} else {
		LOG_FATAL << "Connect to iCAP service gateway failed.";
		LOG_FATAL << "Please check the iCAP service gateway was still working.";
	}

	MQTTAgent_Destroy();

	LOG_INFO << "End iCAP_CoreService_DataHandler successfully.";

	return 0;
}

void gotExitCmd(int sig)
{
	UNUSED(sig);
	is_exit.store(true);
}