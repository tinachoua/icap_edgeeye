#include <atomic>
#include <csignal>
#include <thread>
#include "Commander.hpp"
#include "Configurer.hpp"
#include "DeviceCommand.hpp"
#include "InnoAgeStatus.hpp"
#include "Logger.hpp"
#include "main.hpp"
#include "MQTTAgent.hpp"

using namespace std;

const string mqtt_default_url = "tcp://172.30.0.101:18883";
const int Qos = 1;
const vector<mqttsub_rule> sub_rule_list = { make_pair("innoageSphere/statusRecv/#", Qos) };
extern const char *mqtt_client_id;

const IP_PORT_SET ip_port_set = {
	{"172.30.0.101", "18883"},	// innoage MQTT
    {"172.30.0.5", "6379"},		// Redis
    {"172.30.0.4", "1883"},		// MQTT
    {"172.30.0.6", "50000"},	// auth api
    {"172.30.0.7", "52000"},	// device api
	{"172.30.0.100", "8161"}	// innoage api
};

int main(void)
{
	BOOST_LOG_NAMED_SCOPE(SCOPE_NAME);
	LOG_INFO << "Start iCAP_CoreService_InnoAgeManager program.";

	sigset_t signals;
	if (sigemptyset(&signals) != 0
            || sigaddset(&signals, SIGTERM) != 0
            || sigaddset(&signals, SIGINT) != 0
            || sigaddset(&signals, SIGHUP) != 0
            || pthread_sigmask(SIG_BLOCK, &signals, nullptr) != 0) {
        LOG_FATAL << "Install signal handler failed";
        return 1;
    }

	LOG_INFO << "Please press Ctrl-C to exit.";

	if (!CheckPort(ip_port_set)) {
		LOG_WARN << "The dependent containers are not ready yet.";
		return -1;
	}
	
	InitInnoAgeStatus();

	mqtt_client_id = ("IAM-" + to_string(time(NULL))).c_str();;
	Udefined_connect_option opts;
	opts.username = "innoage";
	opts.password = "B673AEBC6D65E7F42CFABFC7E01C02D0";
	MQTTAgent_AddSubTopics(opts, sub_rule_list);
	
	string mqtt_server_url;
	Configurer config;
	string mqtt_ip_from_setting = config.GetInnoAgeMqttBrokerUrl();
	
	if (!mqtt_ip_from_setting.empty()) 
		mqtt_server_url = mqtt_ip_from_setting;
	else 
		mqtt_server_url = mqtt_default_url;
	
	LOG_INFO << "IP of innoAge MQTT broker:" << mqtt_server_url;

	if (MQTTAgent_Start(opts, mqtt_server_url, MqttMessageReceiver) == 0) {
		LOG_INFO << "Connect to innoAge service gateway successfully.";
		
		int signal = 0;
		int status = sigwait(&signals, &signal);

		if (status == 0)
			LOG_INFO << "Received signal " << signal;
		else
			LOG_ERROR << "sigwait returns " << status;

		MQTTAgent_Stop();
		LOG_INFO << "Disconnect from iCAP service gateway successfully.";
	} else {
		LOG_FATAL << "Connect to innoAge service gateway failed.";
		LOG_FATAL << "Please check the innoAge service gateway was still working.";
	}
	
	MQTTAgent_Destroy();

	LOG_INFO << "End iCAP_CoreService_InnoAgeManager successfully.";

	return 0;
}