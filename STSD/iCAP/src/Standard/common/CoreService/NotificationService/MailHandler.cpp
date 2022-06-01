
#include <jsoncpp/json/json.h>
#include "base64.hpp"
#include "CSmtp.hpp"
#include "main.hpp"
#include "Logger.hpp"
#include "MailHandler.hpp"
#include "Nosqlcommand.hpp"

using namespace std;

MailHandler::MailHandler()
{
	BOOST_LOG_NAMED_SCOPE(SCOPE_NAME);
}

MailHandler::~MailHandler()
{
}

void MailHandler::SetMailSendor(const string& mailsendor_str)
{
	MailSendor ms;

	if (mailsendor_str.empty())
        LOG_DEBUG << "No mail sendor setting.";
	else
		ms = ParseMailSendor(mailsendor_str);

	UpdateMailSendor(ms);
}

MailSendor MailHandler::ParseMailSendor(const string& mailsendor_str)
{
	MailSendor ms;

	Json::Value ms_object;
	Json::Reader reader;
	if (!reader.parse(mailsendor_str, ms_object)) {
		LOG_ERROR << "Failed to parse mail sendor settings." << reader.getFormattedErrorMessages();
		return ms;
	}

	ms.smtp_addr = ms_object["SMTPAddress"].asString();
	ms.port = ms_object["PortNumber"].asInt();
	ms.email_form = ms_object["EmailFrom"].asString();
	ms.password = base64_decode(ms_object["Password"].asString());
	ms.interval = ms_object["ResendInterval"].asDouble();
	ms.enable_ssl = ms_object["EnableSSL"].asBool();
	ms.enable_tls = ms_object["EnableTLS"].asBool();
	ms.enable_mail = ms_object["Enable"].asBool();

	LOG_DEBUG << "[MailServerSetting]:";
	LOG_DEBUG << "  SMTPAddress:" << ms.smtp_addr;
	LOG_DEBUG << "  PortNumber:" << ms.port;
	LOG_DEBUG << "  EmailFrom:" << ms.email_form;
	LOG_DEBUG << "  Password:" << ms.password;
	LOG_DEBUG << "  ResendInterval:" << ms.interval;
	LOG_DEBUG << "  EnableSSL:" << ms.enable_ssl;
	LOG_DEBUG << "  EnableTLS:" << ms.enable_tls;
	LOG_DEBUG << "  Enable:" << ms.enable_mail;

	return ms;
}

void MailHandler::UpdateMailSendor(const MailSendor& ms)
{
	lock_guard<mutex> lock(ms_mutex_);
	mail_sendor_ = ms;
}

bool MailHandler::CheckResend(const string& message)
{
	string event_str;
	
	if (QueryLatestEventByMsgFromDB(message, event_str)) {
		Json::Value event_obj;
		Json::Reader reader;
		reader.parse(event_str, event_obj);

		if (event_obj["Checked"].asBool()) {
			LOG_DEBUG << "This event has been checked!";
			RemoveMailRecord(message);
			return false;
		} 
		
		time_t current_time = time(nullptr);
		
		if (IsFoundMailRecord(message)) {
			time_t last_sendtime = mail_records_[message];
			double current_interval = difftime(current_time, last_sendtime);
			LOG_DEBUG << "Resend interval:" << mail_sendor_.interval;
			LOG_DEBUG << "Current interval:" << current_interval;
			
			if (current_interval >= mail_sendor_.interval) {
				UpdateMailRecord(message, current_time);
				return true;
			} else {
				return false;
			}
		} else {
			InsertMailRecord(message, current_time);
			return true;
		}
	}

	return false; // no event in EventLog
}

bool MailHandler::IsFoundMailRecord(const string& message)
{
	if (mail_records_.find(message) != mail_records_.end())
		return true;
	return false;
}

void MailHandler::InsertMailRecord(const string& message, const time_t time)
{
	lock_guard<mutex> lock(mail_records_mutex_);
	mail_records_.insert({ message, time });
}
void MailHandler::UpdateMailRecord(const string& message, const time_t time)
{
	lock_guard<mutex> lock(mail_records_mutex_);
	auto it = mail_records_.find(message);
	if (it != mail_records_.end())
		it->second = time;
}

void MailHandler::RemoveMailRecord(const string& message)
{
	lock_guard<mutex> lock(mail_records_mutex_);
	auto it = mail_records_.find(message);
	if (it != mail_records_.end())
		mail_records_.erase(it);
}

void MailHandler::SendEmail(const vector<string>& mail_list, const string& message)
{
	try {
		CSmtp mail;

		mail.SetSMTPServer(mail_sendor_.smtp_addr.c_str(), mail_sendor_.port, true);
		if (mail_sendor_.enable_ssl)
			mail.SetSecurityType(USE_SSL);
		else if (mail_sendor_.enable_tls)
			mail.SetSecurityType(USE_TLS);
		else
			mail.SetSecurityType(NO_SECURITY);
		mail.SetLogin(mail_sendor_.email_form.c_str());
		mail.SetPassword(mail_sendor_.password.c_str());
		mail.SetSenderName("iCAP Server");
  		mail.SetSenderMail(mail_sendor_.email_form.c_str());
  		mail.SetSubject("The Event Message");
		for (const auto& addr : mail_list)
			mail.AddRecipient(addr.c_str());
  		mail.AddMsgLine(message.c_str());
		mail.Send();
		LOG_INFO << "Mail was send successfully.";
	} catch (ECSmtp e) {
		LOG_WARN << e.GetErrorText().c_str();
	}		
}