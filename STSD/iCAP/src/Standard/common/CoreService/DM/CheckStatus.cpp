#include <pthread.h>
#include <chrono>
#include <iostream>
#include <thread>
#include "CheckAlive.hpp"
#include "CheckStatus.hpp"
#include "CommandAgent.hpp"
#include "Logger.hpp"
#include "main.hpp"
#include "WebCommand.hpp"

using namespace std;

pthread_mutex_t status_mutex;
map<string,Component*> DeviceStatus;

void* StatusListener(void* t)
{	
	BOOST_LOG_NAMED_SCOPE(SCOPE_NAME);
	char* devName = (char*)t;
	string devName_str = string(devName);
	int count = 0;

	while (count < 3)
	{	
		LOG_DEBUG << "Check device " << devName_str << " status";
		CommandAgent_SendCheckStatus(devName_str);
		this_thread::sleep_for(chrono::seconds(1));
		
		if (Status_check_data(devName_str) == 1)
		{
			LOG_DEBUG << "Device " << devName_str <<" online, remove data.";
			break;
		}
		else
		{
			LOG_DEBUG << "Device " << devName_str <<" offline, count = " << count;
		}		
		count++;
	}

	if (count >= 3)
	{
		int response;
		if (WebCommand_UpdateDeviceStatus(devName_str, 0, response) != 0)
		{
			LOG_INFO << devName_str <<":WebSocket response, Device not found.";
		}
	
		if (response == 0)
		{
			LOG_DEBUG << devName_str << ":WebSocket response, Set offline successful.";
			CommandAgent_PublishDevStatusToNS(devName_str, 0);
			Alive_clear_data_status(devName_str);
		}
	}
	else
	{
		Alive_update_data_status(devName_str);
	}

	Status_remove_data(devName_str);
	
	return NULL;
}

Component* Status_create_comp(const char* devName)
{
	Component* comp_temp = new Component();
	comp_temp->DeviceName = new char[strlen(devName) + 1];
	if (comp_temp->DeviceName == NULL)
	{ 
		exit(0);
	}
	strcpy(comp_temp->DeviceName, devName);
	comp_temp->status = 0;
	pthread_create(&comp_temp->td, NULL, StatusListener, (void*)comp_temp->DeviceName);
	return comp_temp;
}

void Status_insert_new_data(const std::string& devName)
{
	pthread_mutex_lock(&status_mutex);
	map<string, Component*>::iterator iter;
	iter = DeviceStatus.find(devName);
	if (iter == DeviceStatus.end())
	{
		Component* comp = Status_create_comp(devName.c_str());
        DeviceStatus.insert(pair<string, Component*>(devName, comp));
		LOG_DEBUG << "[Status insert] Insert new device " << devName;
	}
	pthread_mutex_unlock(&status_mutex);
}

int Status_update_data_status(string devName)
{	
	map<string, Component*>::iterator iter;
	pthread_mutex_lock(&status_mutex);

	if (!devName.empty())
	{
		iter = DeviceStatus.find(devName);
		if (iter != DeviceStatus.end())
		{
			LOG_DEBUG << "Device " << devName << " response online.";
        	iter->second->status = 1; //update status to 1;
        }
		else
		{
			pthread_mutex_unlock(&status_mutex);
        	return 0;
		}
	}

	pthread_mutex_unlock(&status_mutex);
    return 1;
}

void Status_remove_data(string devName)
{
	map<string, Component*>::iterator iter;
    pthread_mutex_lock(&status_mutex);
	
	if (!devName.empty())
	{
		iter = DeviceStatus.find(devName);
		if (iter->first != "" && iter->second->DeviceName != NULL)
		{
			delete iter->second;
		}
		DeviceStatus.erase(iter);
		LOG_DEBUG << "Remove device " << devName;
	}

	pthread_mutex_unlock(&status_mutex);
}

int Status_find_data(string devName)
{
	int ret = 0;
    map<string, Component*>::iterator iter;
	pthread_mutex_lock(&status_mutex);
	
	if (!devName.empty())
	{
		iter = DeviceStatus.find(devName);
		if (iter != DeviceStatus.end())
		{
			ret = 1;
		}
	}
	pthread_mutex_unlock(&status_mutex);
	
	return ret;
}

int Status_check_data(string devName)
{
	int ret=0;
    map<string,Component*>::iterator iter;
	pthread_mutex_lock(&status_mutex);
	
	if (!devName.empty())
	{
		iter = DeviceStatus.find(devName);
		if (iter != DeviceStatus.end())
		{
			LOG_DEBUG << "Find device " << devName << " in list.";
			pthread_mutex_unlock(&status_mutex);	
			return iter->second->status;
	    }
		else
		{
			LOG_DEBUG << "Can't Find device " << devName << ".";	
		}
	}
	pthread_mutex_unlock(&status_mutex);
	
	return ret;
}