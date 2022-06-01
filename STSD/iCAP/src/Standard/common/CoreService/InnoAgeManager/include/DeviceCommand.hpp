#ifndef IAM_DEVICECOMMAND_H_
#define IAM_DEVICECOMMAND_H_

#include <string>

void MqttMessageReceiver(const std::string& topic, const std::string& message);

#endif // IAM_DEVICECOMMAND_H_