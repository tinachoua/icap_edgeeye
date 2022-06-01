#include <pthread.h>
#include <chrono>
#include <iostream>
#include <map>
#include <thread>
#include "CheckAlive.hpp"
#include "CheckStatus.hpp"
#include "CommandAgent.hpp"
#include "Logger.hpp"
#include "main.hpp"
#include "WebCommand.hpp"

using namespace std;

pthread_mutex_t alive_mutex;
map<string, Component*> DeviceAlive ;

void* AliveStatusListener(void* t)
{
	BOOST_LOG_NAMED_SCOPE(SCOPE_NAME);
	char* devName = (char*)t;
	string devName_str = string(devName);
	int count = 0;
	char *cstr = new char[devName_str.length() + 1];
    strcpy(cstr, devName_str.c_str());
	Status_insert_new_data(cstr);

	while (count < 1)
	{
		LOG_DEBUG << "Check device " << devName_str << " alive";

		Alive_clear_data_status(devName_str);
		this_thread::sleep_for(chrono::seconds(60));

		if (Alive_find_data(devName_str) == 1)
		{
			Status_insert_new_data(cstr);
		}
		else
		{
			break;
		}

		while (Status_find_data(devName_str))
		{
			usleep(100);
		}

		if (Alive_check_data(devName_str) == 1)
		{
			LOG_DEBUG << "Device " << devName_str << " is still alive, reset counter.";
			count = -1;
		}
		else
		{
			LOG_DEBUG << "Device " << devName_str << " was not response data over 1 minute.";
		}

		count++;
	}
	delete [] cstr;

	int response;
	if (Alive_find_data(devName_str) == 1)
	{
		if (WebCommand_UpdateDeviceStatus(devName_str, 0, response) != 0)
		{
			LOG_ERROR << "Failed to update device status.";
			return NULL;
		}
		else
		{
			if (response == 0)
			{
				LOG_DEBUG << devName << ", WebSocket response, Set offline successful.";
				CommandAgent_PublishDevStatusToNS(devName_str, 0);
				LOG_DEBUG << "Find Alive Data. Can remove data.";
				Alive_remove_data(devName_str);
			}
		}
	}

	return NULL;
}

Component* Alive_create_comp(char* devName)
{
	Component* comp_temp = new Component;
    comp_temp->DeviceName = new char[strlen(devName) + 1];
	if (comp_temp->DeviceName == NULL)
	{
		exit(0);
	}
	strcpy(comp_temp->DeviceName,devName);
	comp_temp->status = 0;
	pthread_create(&comp_temp->td, NULL, AliveStatusListener, (void*)comp_temp->DeviceName);
	return comp_temp;
}

void Alive_insert_new_data(char* devName)
{
	string devName_str = string(devName);
	map<string, Component*>::iterator iter;
	iter = DeviceAlive.find(devName_str);
	
	if (iter == DeviceAlive.end())
	{
		pthread_mutex_lock(&alive_mutex);
		Component* comp = Alive_create_comp(devName);
		DeviceAlive.insert(pair<string, Component*>(devName_str,comp));
		LOG_DEBUG << "Insert new device " << devName;
		pthread_mutex_unlock(&alive_mutex);
   	}
    else
	{
		LOG_DEBUG << "Device already in alive list.";
    }
}

void Alive_update_data_status(string devName)
{
	pthread_mutex_lock(&alive_mutex);
	map<string, Component*>::iterator iter;

	if (!devName.empty())
	{
		iter = DeviceAlive.find(devName);
		if (iter != DeviceAlive.end())
		{
			LOG_DEBUG << "Device " << devName << " response online.";
        	iter->second->status = 1; //update status to 1;
			pthread_mutex_unlock(&alive_mutex);
        }
		else
		{
	 		pthread_mutex_unlock(&alive_mutex);
		}
	}
}

void Alive_remove_data(string devName)
{
	pthread_mutex_lock(&alive_mutex);
	map<string, Component*>::iterator iter;
	
	if (!devName.empty())
	{
		iter = DeviceAlive.find(devName);
		if (iter->first != "" && iter->second->DeviceName != NULL)
		{
			DeviceAlive.erase(iter);
		}


		if (iter == DeviceAlive.end()) //find element
		{
			LOG_DEBUG <<"Remove device " << devName;
		}
		pthread_mutex_unlock(&alive_mutex);
	}
}

int Alive_find_data(string devName)
{
	int ret = 0;
	pthread_mutex_lock(&alive_mutex);
	map<string, Component*>::iterator iter;	
	
	if (!devName.empty())
	{
		iter = DeviceAlive.find(devName);
		if (iter != DeviceAlive.end())
		{
			LOG_DEBUG << "Device " << devName << " is in list.";
			ret = 1;
		}
	}
	
	pthread_mutex_unlock(&alive_mutex);
	return ret;
}

int Alive_check_data(string devName)
{
	int ret = 0;
	pthread_mutex_lock(&alive_mutex);
	map<string, Component*>::iterator iter;	

	if (!devName.empty())
	{
		iter = DeviceAlive.find(devName);
		if (iter != DeviceAlive.end())
		{
			LOG_DEBUG << "Find device " << devName << " in list.";
			pthread_mutex_unlock(&alive_mutex);
			return iter->second->status;
	    }
	}
	pthread_mutex_unlock(&alive_mutex);
	
	return ret;
}

void Alive_clear_data_status(string devName)
{
	pthread_mutex_lock(&alive_mutex);
	map<string, Component*>::iterator iter;
	
	if (!devName.empty())
	{
		iter = DeviceAlive.find(devName);
		if (iter != DeviceAlive.end())
		{
			iter->second->status = 0;
			LOG_DEBUG << "Clear device " << devName << " status.";
	    }
	}
	pthread_mutex_unlock(&alive_mutex);
}