#ifndef DM_DEVICECOMMAND_H_
#define DM_DEVICECOMMAND_H_

#include <string>

void DeviceCommand_InitialCheck();
void MqttMessageReceiver(const std::string& topic, const std::string& message);
int GetFakeDevice();

#endif // DM_DEVICECOMMAND_H_