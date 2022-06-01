#ifndef DH_DEVICECOMMAND_H_
#define DH_DEVICECOMMAND_H_

#include <string>

void MqttMessageReceiver(const std::string& topic, const std::string& message);

#endif // DH_DEVICECOMMAND_H_