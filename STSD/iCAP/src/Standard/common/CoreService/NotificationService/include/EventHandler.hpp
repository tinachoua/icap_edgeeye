#ifndef NS_EVENTHANDLER_H_
#define NS_EVENTHANDLER_H_

#include <MailHandler.hpp>
#include <ThresholdAgent.hpp>

class EventHandler
{
public:
    EventHandler();
    ~EventHandler();
    int GetTHSetting();
    int GetMailSendor();
    void ProcessDevStatusEvent(const std::string& msg);
    void ProcessRawEvent(const std::string& dev_name, const std::string& raw);

private:
    THAgent th_agent_;
    MailHandler mail_handler_;

    int IsNewEvent(const std::string& dev_name, const std::string& message);
    void CreateEvent(const std::string& dev_name, const std::string& message);
};

#endif // NS_EVENTHANDLER_H_