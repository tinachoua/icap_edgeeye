#ifndef IAM_INNOAGESTATUS_H_
#define IAM_INNOAGESTATUS_H_

#include <string>

void InitInnoAgeStatus();
void WriteAllInnoAgeStatus(const std::string& innoage_list);
void WriteInnoAgeStatusFromWebAPI(const std::string& sn);
void WriteInnoAgeStatusFromMqtt(const std::string& sn, const std::string& status);
void NotifyInnoAgeStatusToNS(const std::string& id, const std::string& status);

#endif // IAM_INNOAGESTATUS_H_