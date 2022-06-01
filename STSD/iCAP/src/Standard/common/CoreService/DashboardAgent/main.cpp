#include <atomic>
#include <chrono>
#include <csignal>
#include <future>
#include <iostream>
#include <thread>
#include <utility>
#include "Commander.hpp"
#include "WidgetProcessor.hpp"
#include "Logger.hpp"
#include "main.hpp" 
#include "Nosqlcommand.hpp"
#include "SocketAgent.hpp"

using namespace std;

#define UNUSED(x) (void)(x)

atomic<bool> is_exit { false }, running { true };

const IP_PORT_SET ip_port_set = {
    {"172.30.0.2", "27017"},	// MongoDB
    {"172.30.0.5", "6379"},		// Redis
    {"172.30.0.6", "50000"},	// auth api
    {"172.30.0.8", "51000"}		// dashboard api
};

void gotExitCmd(int sig);

int main(void)
{	
	BOOST_LOG_NAMED_SCOPE(SCOPE_NAME);
	signal(SIGINT, gotExitCmd);
	LOG_INFO << "Start iCAP_CoreService_DashboardAgent program.";
	LOG_INFO << "Please press Ctrl-C to exit.";

	if (!CheckPort(ip_port_set)) {
		LOG_WARN << "The dependent containers are not ready yet.";
		return -1;
	}

	if (InitializeMongoDB()) {
		LOG_INFO << "Initializing the Data DB successfully.";
	} else {
		LOG_FATAL << "Initializing the Data DB failed.";
		return -1;
	}

	thread check_thread(CheckNewSettings); // Create a socket thread to check if there are new dashboard settings

	WidgetProcessor widget_processor;
	do {
		widget_processor.CalculateWidgets(false);
		this_thread::sleep_for(chrono::seconds(1));
	} while (!is_exit.load(memory_order_relaxed));
	
	running.store(false);
	check_thread.join();

	LOG_INFO << "End iCAP_CoreService_DashboardAgent successfully.";

	return 0;
}

void gotExitCmd(int sig)
{
	UNUSED(sig);
	is_exit.store(true);
}