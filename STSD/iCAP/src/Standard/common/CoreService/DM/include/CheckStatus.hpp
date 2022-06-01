#ifndef DM_CHECKSTATUS_H_
#define DM_CHECKSTATUS_H_

#include <map>
#include <string>

typedef struct component {
	pthread_t td;
	int status;
	char* DeviceName;
} Component;

extern std::map<std::string, Component*> DeviceStatus; // Devname, thread+Status

void Status_insert_new_data(const std::string& devName);
int Status_find_data(std::string devName);
int Status_update_data_status(std::string devName);
void Status_remove_data(std::string devName);
int Status_check_data(std::string devName);
void NonBlockingWait(int seconds);

#endif // DM_CHECKSTATUS_H_