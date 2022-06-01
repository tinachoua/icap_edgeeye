#include <iostream>
#include <string>
#include "WebCommand.hpp"

using namespace std;

int main(void)
{
    /* AuthAPI */
    cout << "WebCommand_GetToken" << endl;
    int result;
    string token;
    result = WebCommand_GetToken(token);
    cout << "  Result:" << result << ", token:" << token << endl;

    /* DeviceAPI*/
    string id = "Device00001";
    cout << "WebCommand_UpdateDeviceStatus, id:" << id << endl;
    int response;
    result = WebCommand_UpdateDeviceStatus(id, 0, response);
    cout << "  Result:" << result << ", response:" << response << endl;
    if (response != 0)
        cout << ". Failed to update device status to offline." << endl;

    cout << "WebCommand_GetDeviceStatus, id:" << id << endl;
    result = WebCommand_GetDeviceStatus(id, response);
    cout << "  Status:" << response << endl;

    cout << "WebCommand_GetDeviceList" << endl;
	string responseStr;
    WebCommand_GetDeviceList(responseStr);
    cout << "  Device list:" << responseStr << endl;

    cout << "WebCommand_GetInnoAgeList" << endl;
	responseStr.clear();
    WebCommand_GetInnoAgeList(responseStr);
    cout << "  innoAge list:" << responseStr << endl;

    cout << "WebCommand_UpdateDeviceAlias" << endl;
    string alias = "{\"Alias\":\"test\"}";
    WebCommand_UpdateDeviceAlias(id, alias);
    
    /* DashboardAPI */
    cout << "Webcommand_GetEmailSendor" << endl;
    responseStr.clear();
    Webcommand_GetEmailSendor(1, responseStr);
    cout << "  EmailSendor:" << responseStr << endl;
    cout << "Webcommand_GetWidgetsSetting" << endl;
    responseStr.clear();
    Webcommand_GetWidgetsSetting(responseStr);
    cout << "  Dashboard widget setting:" << responseStr << endl;
    cout << "Webcommand_GetHighPriorityWidgetsSetting" << endl;
    responseStr.clear();
    Webcommand_GetHighPriorityWidgetsSetting(responseStr);
    cout << "  NewWidgetSetting:" << responseStr << endl;
    cout << "Webcommand_GetTHSetting" << endl;
    responseStr.clear();
    Webcommand_GetTHSetting(responseStr);
    cout << "  Threshold setting:" << responseStr << endl;

    return 0;
}