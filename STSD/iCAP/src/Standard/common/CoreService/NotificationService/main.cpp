#include <atomic>
#include <csignal>
#include <thread>
#include "Commander.hpp"
#include "DeviceCommand.hpp"
#include "Logger.hpp"
#include "main.hpp"
#include "MQTTAgent.hpp"
#include "Nosqlcommand.hpp"
#include "RedisSubscriber.hpp"
#include "SocketAgent.hpp"

using namespace std;

#define UNUSED(x) (void)(x)

atomic<bool> is_exit { false }, running { true };
const string mqtt_server_url = "tcp://172.30.0.4:1883";
const int Qos = 1;
const vector<mqttsub_rule> sub_rule_list = { make_pair("RawData", Qos) };
extern const char *mqtt_client_id;

const IP_PORT_SET ip_port_set = {
    {"172.30.0.2", "27017"},	// MongoDB
    {"172.30.0.5", "6379"},		// Redis
    {"172.30.0.4", "1883"},		// MQTT
    {"172.30.0.6", "50000"},	// auth api
    {"172.30.0.7", "52000"},	// device api
	{"172.30.0.8", "51000"}		// dashboard api
};

void gotExitCmd(int sig);

int main(void)
{
	BOOST_LOG_NAMED_SCOPE(SCOPE_NAME);
	signal(SIGPIPE, SIG_IGN);
	signal(SIGINT, gotExitCmd);
	LOG_INFO << "Start iCAP_CoreService_NotificationService program.";
	LOG_INFO << "Please press Ctrl-C to exit.";

	if (!CheckPort(ip_port_set)) {
		LOG_WARN << "The dependent containers are not ready yet.";
		return -1;
	}

	mqtt_client_id = ("NS-" + to_string(time(NULL))).c_str();;
	Udefined_connect_option opts;
	opts.username = "admin";
	opts.password = "AH0MBwnqi3O-9Dxlt7ZxGHBGsZC5TnEA";
	MQTTAgent_AddSubTopics(opts, sub_rule_list);

	if (MQTTAgent_Start(opts, mqtt_server_url, MqttMessageReceiver) == 0) {
		LOG_INFO << "Connect to iCAP service gateway successfully.";

		if (InitializeMongoDB())
			LOG_INFO << "Initializing the Data DB successfully.";
		else
			LOG_FATAL << "Initializing the Data DB failed.";

		// Create a socket thread to check if there are the settings change.
		thread setting_thread(SettingsListener);

		/* If disconnected from the Redis server, try to reconnect. */
		do {
			RedisSubscriber();
			this_thread::sleep_for(chrono::seconds(1));
		} while(!is_exit.load(memory_order_relaxed));
		
		running.store(false, memory_order_relaxed);
		setting_thread.join();

		MQTTAgent_Stop();
		LOG_INFO << "Disconnect from iCAP service gateway successfully.";
	} else {
		LOG_FATAL << "Connect to iCAP service gateway failed.";
		LOG_FATAL << "Please check the iCAP service gateway was still working.";
	}

	MQTTAgent_Destroy();
	
	LOG_INFO << "End iCAP_CoreService_NotificationService successfully.";

	return 0;
}

void gotExitCmd(int sig)
{
	UNUSED(sig);
	RedisDisconnect();
	is_exit.store(true);
}