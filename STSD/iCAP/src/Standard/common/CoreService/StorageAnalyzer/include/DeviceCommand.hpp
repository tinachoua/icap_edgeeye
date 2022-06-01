#ifndef SA_DEVICECOMMAND_H_
#define SA_DEVICECOMMAND_H_

#include <string>

void DeviceCommand_InitialCheck(void);
void MqttMessageReceiver(const std::string& topic, const std::string& message);

#endif // SA_DEVICECOMMAND_H_