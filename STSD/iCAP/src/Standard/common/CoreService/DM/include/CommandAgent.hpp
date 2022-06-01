#ifndef DM_COMMANDAGENT_H_
#define DM_COMMANDAGENT_H_

#include <string>

void CommandAgent_SendCheckStatus(const std::string& devID);
void CommandAgent_SendOK(const std::string& devID);
void CommandAgent_SendFail(const std::string& devID);
void CommandAgent_SendStart(const std::string& devID);
void CommandAgent_SendOffline(const std::string& devID);
void CommandAgent_PublishDevStatusToNS(const std::string& id, const int& status);

#endif // DM_COMMANDAGENT_H_