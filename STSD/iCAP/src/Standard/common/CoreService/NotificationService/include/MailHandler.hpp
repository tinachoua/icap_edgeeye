#ifndef NS_MAILHANDLER_H_
#define NS_MAILHANDLER_H_

#include <ctime>
#include <map>
#include <mutex>
#include <string>
#include <vector>

struct MailSendor
{
    std::string smtp_addr;
    int port;
    std::string email_form;
    std::string password;
    double interval;
    bool enable_ssl;
    bool enable_tls;
    bool enable_mail = false;
};

typedef std::map<std::string, time_t> MailRecords;

class MailHandler
{
public:
    MailHandler();
    ~MailHandler();
    void SetMailSendor(const std::string& mailsendor_str);
    bool CheckResend(const std::string& message);
    void SendEmail(const std::vector<std::string>& mail_list, const std::string& message);

private:
    std::mutex ms_mutex_;
    std::mutex mail_records_mutex_;
    MailSendor mail_sendor_;
    MailRecords mail_records_;

    MailSendor ParseMailSendor(const std::string& mailsendor_str);
    bool IsFoundMailRecord(const std::string& message);
    void UpdateMailSendor(const MailSendor& ms);
    void InsertMailRecord(const std::string& message, const time_t time);
    void UpdateMailRecord(const std::string& message, const time_t time);
    void RemoveMailRecord(const std::string& message);
};

#endif // NS_MAILHANDLER_H_