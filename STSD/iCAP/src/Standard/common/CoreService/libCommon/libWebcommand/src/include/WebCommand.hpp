#ifndef LIBCOMMON_LIBWEBCOMMAND_WEBCOMMAND_H_
#define LIBCOMMON_LIBWEBCOMMAND_WEBCOMMAND_H_

#include <string>

typedef struct emailset
{
	const char* user;
	char* password;
	char* smtpaddr;
	int enableSSL=0;
	char *emp_list[50] = {0};
} EmailSet;
extern EmailSet email;

int WebCommand_UpdateDeviceStatus(const std::string& dev_id, const int& status, int& ret);
int WebCommand_UpdateDeviceAlias(const std::string& device_name, const std::string& alias);
int WebCommand_GetToken(std::string& token);
int WebCommand_GetDeviceStatus(const std::string& dev_id, int& status);
int WebCommand_GetInnoAgeStatus(const std::string& setting_url, const std::string& sn, int& status);
int WebCommand_GetDeviceList(std::string& ret);
int WebCommand_GetInnoAgeList(std::string& ret);
int Webcommand_GetEmailSendor(int company_id, std::string& ret);
int Webcommand_GetWidgetsSetting(std::string& ret);
int Webcommand_GetHighPriorityWidgetsSetting(std::string& ret);
int Webcommand_GetTHSetting(std::string& ret);
int Webcommand_GetDBSetting(std::string& ret);

#endif // LIBCOMMON_LIBWEBCOMMAND_WEBCOMMAND_H_