#include <cstring>
#include <ctime>
#include <list>
#include <sstream> // std::ostringstream
#include <curlpp/cURLpp.hpp>
#include <curlpp/Easy.hpp>
#include <curlpp/Exception.hpp>
#include <curlpp/Infos.hpp>
#include <curlpp/Options.hpp>
#include <jsoncpp/json/json.h>
#include "HttpStatusCodes.hpp"
#include "Logger.hpp"
#include "WebCommand.hpp"

using namespace std;

const char *auth_api_addr = "http://172.30.0.6:50000";
const char *dashboard_api_addr = "http://172.30.0.8:51000";
const char *device_api_addr = "http://172.30.0.7:52000";
const char *innoage_api_addr = "http://172.30.0.100:8161";

void PrintReplyStatus(const string& func_name, int http_status_code)
{
	if (HttpStatus::isError(http_status_code))
		LOG_ERROR << func_name << ":" << http_status_code << " " << HttpStatus::reasonPhrase(http_status_code);
	else
		LOG_DEBUG << func_name << ":" << http_status_code << " " << HttpStatus::reasonPhrase(http_status_code);
}

int WebCommand_GetToken(std::string& token)
{
	try {
		curlpp::Cleanup cleaner;
		curlpp::Easy request;

		// URL
		string url = string(auth_api_addr) + "/AuthenticationAPI/Login";
		request.setOpt(new curlpp::options::Url(url));
		// custom request
		request.setOpt(new curlpp::options::CustomRequest{"GET"});
		// header
		list<string> headers;
		headers.push_back("cache-control: no-cache");
		headers.push_back("content-type: application/x-www-form-urlencoded");
		headers.push_back("username:admin");
		headers.push_back("password:admin");
		request.setOpt(new curlpp::options::HttpHeader(headers));
		//user agent
		request.setOpt(new curlpp::options::UserAgent("libcurl-agent/1.0"));
		// get response
		ostringstream os;
		request.setOpt(new curlpp::options::WriteStream(&os));

#ifdef DEBUG
		// more information
		request.setOpt(new curlpp::options::Verbose(true));
#endif
		request.perform();

		int https_status_code = static_cast<int>(curlpp::infos::ResponseCode::get(request));
		if (https_status_code == HttpStatus::toInt(HttpStatus::Code::OK)) {
			Json::Value response_content;
			Json::Reader reader;
			reader.parse(os.str(), response_content);
			token = response_content["Token"].asString();
		}

		PrintReplyStatus("WebCommand_GetToken", https_status_code);
		if (HttpStatus::isError(https_status_code))
			return -1;
	} catch (curlpp::RuntimeError& e) {
		LOG_ERROR << e.what();
		return -1;
	} catch (curlpp::LogicError& e) {       
		LOG_ERROR << e.what();
		return -1;
	}

	return 0;
}

int WebCommand_GetDeviceStatus(const std::string& dev_id, int& status)
{
	try {
		string token;
		if (WebCommand_GetToken(token) != 0) {
			LOG_ERROR << "Web socket authentication fail.";
			return -1;
		}

		curlpp::Cleanup cleaner;
		curlpp::Easy request;

		// URL
		string url = string(device_api_addr) + "/StatusAPI/Get";
		request.setOpt(new curlpp::options::Url(url));
		// custom request
		request.setOpt(new curlpp::options::CustomRequest{"GET"});
		// header
		list<string> headers;
		headers.push_back("cache-control: no-cache");
		headers.push_back("content-type: application/x-www-form-urlencoded");
		headers.push_back("token:" + token);
		headers.push_back("device:" + dev_id);
		request.setOpt(new curlpp::options::HttpHeader(headers));
		// user agent
		request.setOpt(new curlpp::options::UserAgent("libcurl-agent/1.0"));
		// get response
		ostringstream os;
		request.setOpt(new curlpp::options::WriteStream(&os));
		
#ifdef DEBUG
		// more information
		request.setOpt(new curlpp::options::Verbose(true));
#endif
		request.perform();
		
		int https_status_code = static_cast<int>(curlpp::infos::ResponseCode::get(request));
		if (https_status_code == HttpStatus::toInt(HttpStatus::Code::OK)) {
			Json::Value response_content;
			Json::Reader reader;
			reader.parse(os.str(), response_content);
			status = response_content["Response"].asInt();
		} else if (https_status_code == HttpStatus::toInt(HttpStatus::Code::NotFound)) {
			status = 2;
		}

		PrintReplyStatus("WebCommand_GetDeviceStatus", https_status_code);
		if (HttpStatus::isError(https_status_code))
			return -1;
	} catch (curlpp::RuntimeError& e) {
		LOG_ERROR << e.what();
		return -1;
	} catch (curlpp::LogicError& e) {       
		LOG_ERROR << e.what();
		return -1;
	}
	
	return 0;
}

int WebCommand_GetInnoAgeStatus(const std::string& setting_url, const std::string& sn, int& status)
{
	try {
		curlpp::Cleanup cleaner;
		curlpp::Easy request;

		// URL
		string url;
		if (setting_url.empty())
			url = string(innoage_api_addr) + "/devices/status/" + sn;
		else
			url = setting_url + "/devices/status/" + sn;
		
		request.setOpt(new curlpp::options::Url(url));
		// custom request
		request.setOpt(new curlpp::options::CustomRequest{"GET"});
		// header
		list<string> headers;
		headers.push_back("cache-control: no-cache");
		headers.push_back("content-type: application/x-www-form-urlencoded");
		request.setOpt(new curlpp::options::HttpHeader(headers));
		// user agent
		request.setOpt(new curlpp::options::UserAgent("libcurl-agent/1.0"));
		// set timeout
		request.setOpt(new curlpp::options::Timeout(5L));
		// get response
		ostringstream os;
		request.setOpt(new curlpp::options::WriteStream(&os));
		
#ifdef DEBUG
		// more information
		request.setOpt(new curlpp::options::Verbose(true));
#endif
		request.perform();
		
		int https_status_code = static_cast<int>(curlpp::infos::ResponseCode::get(request));
		if (https_status_code == HttpStatus::toInt(HttpStatus::Code::OK)) {
			Json::Value response_content;
			Json::Reader reader;
			reader.parse(os.str(), response_content);
			status = response_content["Payload"]["Data"]["Status"].asInt();
		}

		PrintReplyStatus("WebCommand_GetInnoAgeStatus", https_status_code);
		if (HttpStatus::isError(https_status_code))
			return -1;
	} catch (curlpp::RuntimeError& e) {
		LOG_ERROR << e.what();
		return -1;
	} catch (curlpp::LogicError& e) {       
		LOG_ERROR << e.what();
		return -1;
	}
	
	return 0;
}

int WebCommand_UpdateDeviceStatus(const std::string& dev_id, const int& status, int& ret)
{
	try {
		string token;
		if (WebCommand_GetToken(token) != 0) {
			LOG_ERROR << "Web socket authentication fail.";
			return -1;
		}

		curlpp::Cleanup cleaner;
		curlpp::Easy request;

		// URL
		string url = string(device_api_addr) + "/StatusAPI/Update";
		request.setOpt(new curlpp::options::Url(url));
		// custom request
		request.setOpt(new curlpp::options::CustomRequest{"PUT"});
		// header
		list<string> headers;
		headers.push_back("cache-control: no-cache");
		headers.push_back("content-type: application/json");
		headers.push_back("token:" + token);
		request.setOpt(new curlpp::options::HttpHeader(headers));
		// body
		Json::Value body;
		body["DeviceName"] = dev_id;
		body["Status"] = status;
		string body_str = body.toStyledString();
		request.setOpt(new curlpp::options::PostFields(body_str));
    	request.setOpt(new curlpp::options::PostFieldSize(body_str.length()));
		// user agent
		request.setOpt(new curlpp::options::UserAgent("libcurl-agent/1.0"));
		// get response
		ostringstream os;
		request.setOpt(new curlpp::options::WriteStream(&os));

#ifdef DEBUG
		// more information
		request.setOpt(new curlpp::options::Verbose(true));
#endif
		request.perform();

		int https_status_code = static_cast<int>(curlpp::infos::ResponseCode::get(request));
		if (https_status_code == HttpStatus::toInt(HttpStatus::Code::Accepted)) {
			Json::Value response_content;
			Json::Reader reader;
			reader.parse(os.str(), response_content);
			ret = response_content["Response"].asInt();
		}

		PrintReplyStatus("WebCommand_UpdateDeviceStatus", https_status_code);
		if (HttpStatus::isError(https_status_code))
			return -1;
	} catch (curlpp::RuntimeError& e) {
		LOG_ERROR << e.what();
		return -1;
	} catch (curlpp::LogicError& e) {       
		LOG_ERROR << e.what();
		return -1;
	}
	
	return 0;
}

int WebCommand_GetDeviceList(std::string& ret)
{
	try {
		string token;
		if (WebCommand_GetToken(token) != 0) {
			LOG_ERROR << "Web socket authentication fail.";
			return -1;
		}

		curlpp::Cleanup cleaner;
		curlpp::Easy request;

		// URL
		string url = string(device_api_addr) + "/StatusAPI/GetList";
		request.setOpt(new curlpp::options::Url(url));
		// custom request
		request.setOpt(new curlpp::options::CustomRequest{"GET"});
		// header
		list<string> headers;
		headers.push_back("cache-control: no-cache");
		headers.push_back("content-type: application/x-www-form-urlencoded");
		headers.push_back("token:" + token);
		request.setOpt(new curlpp::options::HttpHeader(headers));
		// user agent
		request.setOpt(new curlpp::options::UserAgent("libcurl-agent/1.0"));
		// get response
		ostringstream os;
		request.setOpt(new curlpp::options::WriteStream(&os));
		
#ifdef DEBUG
		// more information
		request.setOpt(new curlpp::options::Verbose(true));
#endif
		request.perform();

		int https_status_code = static_cast<int>(curlpp::infos::ResponseCode::get(request));
		if (https_status_code == HttpStatus::toInt(HttpStatus::Code::OK))
			ret = os.str();

		PrintReplyStatus("WebCommand_GetDeviceList", https_status_code);
		if (HttpStatus::isError(https_status_code))
			return -1;
	}
	catch (curlpp::RuntimeError& e) {
		LOG_ERROR << e.what();
		return -1;
	} catch (curlpp::LogicError& e) {       
		LOG_ERROR << e.what();
		return -1;
	}
	
	return 0;
}

int WebCommand_GetInnoAgeList(std::string& ret)
{
	try {
		string token;
		if (WebCommand_GetToken(token) != 0) {
			LOG_ERROR << "Web socket authentication fail.";
			return -1;
		}

		curlpp::Cleanup cleaner;
		curlpp::Easy request;

		// URL
		string url = string(device_api_addr) + "/DeviceInfoAPI/InnoAgelist";
		request.setOpt(new curlpp::options::Url(url));
		// custom request
		request.setOpt(new curlpp::options::CustomRequest{"GET"});
		// header
		list<string> headers;
		headers.push_back("cache-control: no-cache");
		headers.push_back("content-type: application/x-www-form-urlencoded");
		headers.push_back("token:" + token);
		request.setOpt(new curlpp::options::HttpHeader(headers));
		// user agent
		request.setOpt(new curlpp::options::UserAgent("libcurl-agent/1.0"));
		// get response
		ostringstream os;
		request.setOpt(new curlpp::options::WriteStream(&os));
		
#ifdef DEBUG
		// more information
		request.setOpt(new curlpp::options::Verbose(true));
#endif
		request.perform();

		int https_status_code = static_cast<int>(curlpp::infos::ResponseCode::get(request));
		if (https_status_code == HttpStatus::toInt(HttpStatus::Code::OK))
			ret = os.str();

		PrintReplyStatus("WebCommand_GetInnoAgeList", https_status_code);
		if (HttpStatus::isError(https_status_code))
			return -1;
	}
	catch (curlpp::RuntimeError& e) {
		LOG_ERROR << e.what();
		return -1;
	} catch (curlpp::LogicError& e) {       
		LOG_ERROR << e.what();
		return -1;
	}
	
	return 0;
}

int Webcommand_GetEmailSendor(int company_id, std::string& ret)
{
	try {
		string token;
		if (WebCommand_GetToken(token) != 0) {
			LOG_ERROR << "Web socket authentication fail.";
			return -1;
		}

		curlpp::Cleanup cleaner;
		curlpp::Easy request;

		// URL
		string url = string(dashboard_api_addr) + "/EventAPI/SMTP/Setting";
		request.setOpt(new curlpp::options::Url(url));
		// custom request
		request.setOpt(new curlpp::options::CustomRequest{"GET"});
		// header
		list<string> headers;
		headers.push_back("cache-control: no-cache");
		headers.push_back("content-type: application/x-www-form-urlencoded");
		headers.push_back("CompanyId:" + to_string(company_id));
		headers.push_back("token:" + token);
		request.setOpt(new curlpp::options::HttpHeader(headers));
		// user agent
		request.setOpt(new curlpp::options::UserAgent("libcurl-agent/1.0"));
		// get response
		ostringstream os;
		request.setOpt(new curlpp::options::WriteStream(&os));
		
#ifdef DEBUG
		// more information
		request.setOpt(new curlpp::options::Verbose(true));
#endif
		request.perform();

		int https_status_code = static_cast<int>(curlpp::infos::ResponseCode::get(request));
		if (https_status_code == HttpStatus::toInt(HttpStatus::Code::OK))
			ret = os.str();

		PrintReplyStatus("Webcommand_GetEmailSendor", https_status_code);
		if (HttpStatus::isError(https_status_code))
			return -1;
	} catch (curlpp::RuntimeError& e) {
		LOG_ERROR << e.what();
		return -1;
	} catch (curlpp::LogicError& e) {       
		LOG_ERROR << e.what();
		return -1;
	}
	
	return 0;
}

int Webcommand_GetWidgetsSetting(string& ret)
{
	try {
		string token;
		if (WebCommand_GetToken(token) != 0) {
			LOG_ERROR << "Web socket authentication fail.";
			return -1;
		}

		curlpp::Cleanup cleaner;
		curlpp::Easy request;

		// URL
		string url = string(dashboard_api_addr) + "/DashboardAPI/Panel/Setting";
		request.setOpt(new curlpp::options::Url(url));
		// custom request
		request.setOpt(new curlpp::options::CustomRequest{"GET"});
		// header
		list<string> headers;
		headers.push_back("cache-control: no-cache");
		headers.push_back("content-type: application/x-www-form-urlencoded");
		headers.push_back("token:" + token);
		request.setOpt(new curlpp::options::HttpHeader(headers));
		// user agent
		request.setOpt(new curlpp::options::UserAgent("libcurl-agent/1.0"));
		// get response
		ostringstream os;
		request.setOpt(new curlpp::options::WriteStream(&os));
		
#ifdef DEBUG
		// more information
		request.setOpt(new curlpp::options::Verbose(true));
#endif
		request.perform();

		int https_status_code = static_cast<int>(curlpp::infos::ResponseCode::get(request));
		if (https_status_code == HttpStatus::toInt(HttpStatus::Code::OK))
			ret = os.str();

		PrintReplyStatus("Webcommand_GetWidgetsSetting", https_status_code);
		if (HttpStatus::isError(https_status_code))
			return -1;
	} catch (curlpp::RuntimeError& e) {
		LOG_ERROR << e.what();
		return -1;
	} catch (curlpp::LogicError& e) {       
		LOG_ERROR << e.what();
		return -1;
	}
	
	return 0;
}

int Webcommand_GetHighPriorityWidgetsSetting(string& ret)
{
	try {
		string token;
		if (WebCommand_GetToken(token) != 0) {
			LOG_ERROR << "Web socket authentication fail.";
			return -1;
		}

		curlpp::Cleanup cleaner;
		curlpp::Easy request;

		// URL
		string url = string(dashboard_api_addr) + "/DashboardAPI/NewPanel/Setting";
		request.setOpt(new curlpp::options::Url(url));
		// custom request
		request.setOpt(new curlpp::options::CustomRequest{"GET"});
		// header
		list<string> headers;
		headers.push_back("cache-control: no-cache");
		headers.push_back("content-type: application/x-www-form-urlencoded");
		headers.push_back("token:" + token);
		request.setOpt(new curlpp::options::HttpHeader(headers));
		// user agent
		request.setOpt(new curlpp::options::UserAgent("libcurl-agent/1.0"));
		// get response
		ostringstream os;
		request.setOpt(new curlpp::options::WriteStream(&os));
		
#ifdef DEBUG
		// more information
		request.setOpt(new curlpp::options::Verbose(true));
#endif
		request.perform();

		int https_status_code = static_cast<int>(curlpp::infos::ResponseCode::get(request));
		if (https_status_code == HttpStatus::toInt(HttpStatus::Code::OK))
			ret = os.str();

		PrintReplyStatus("Webcommand_GetHighPriorityWidgetsSetting", https_status_code);
		if (HttpStatus::isError(https_status_code))
			return -1;
	} catch (curlpp::RuntimeError& e) {
		LOG_ERROR << e.what();
		return -1;
	} catch (curlpp::LogicError& e) {       
		LOG_ERROR << e.what();
		return -1;
	}
	
	return 0;
}

int Webcommand_GetTHSetting(std::string& ret)
{
	try {
		string token;
		if (WebCommand_GetToken(token) != 0) {
			LOG_ERROR << "Web socket authentication fail.";
			return -1;
		}

		curlpp::Cleanup cleaner;
		curlpp::Easy request;

		// URL
		string url = string(dashboard_api_addr) + "/SettingAPI/Threshold/Setting";
		request.setOpt(new curlpp::options::Url(url));
		// custom request
		request.setOpt(new curlpp::options::CustomRequest{"GET"});
		// header
		list<string> headers;
		headers.push_back("cache-control: no-cache");
		headers.push_back("content-type: application/x-www-form-urlencoded");
		headers.push_back("token:" + token);
		request.setOpt(new curlpp::options::HttpHeader(headers));
		// user agent
		request.setOpt(new curlpp::options::UserAgent("libcurl-agent/1.0"));
		// get response
		ostringstream os;
		request.setOpt(new curlpp::options::WriteStream(&os));
		
#ifdef DEBUG
		// more information
		request.setOpt(new curlpp::options::Verbose(true));
#endif
		request.perform();

		int https_status_code = static_cast<int>(curlpp::infos::ResponseCode::get(request));
		if (https_status_code == HttpStatus::toInt(HttpStatus::Code::OK))
			ret = os.str();

		PrintReplyStatus("Webcommand_GetTHSetting", https_status_code);
		if (HttpStatus::isError(https_status_code))
			return -1;
	} catch (curlpp::RuntimeError& e) {
		LOG_ERROR << e.what();
		return -1;
	} catch (curlpp::LogicError& e) {       
		LOG_ERROR << e.what();
		return -1;
	}
	
	return 0;
}

int Webcommand_GetDBSetting(std::string& ret)
{
	try {
		string token;
		if (WebCommand_GetToken(token) != 0) {
			LOG_ERROR << "Web socket authentication fail.";
			return -1;
		}

		curlpp::Cleanup cleaner;
		curlpp::Easy request;

		// URL
		string url = string(dashboard_api_addr) + "/SettingAPI/RawData/Expiry-Date";
		request.setOpt(new curlpp::options::Url(url));
		// custom request
		request.setOpt(new curlpp::options::CustomRequest{"GET"});
		// header
		list<string> headers;
		headers.push_back("cache-control: no-cache");
		headers.push_back("content-type: application/x-www-form-urlencoded");
		headers.push_back("token:" + token);
		request.setOpt(new curlpp::options::HttpHeader(headers));
		// user agent
		request.setOpt(new curlpp::options::UserAgent("libcurl-agent/1.0"));
		// get response
		ostringstream os;
		request.setOpt(new curlpp::options::WriteStream(&os));
		
#ifdef DEBUG
		// more information
		request.setOpt(new curlpp::options::Verbose(true));
#endif
		request.perform();

		int https_status_code = static_cast<int>(curlpp::infos::ResponseCode::get(request));
		if (https_status_code == HttpStatus::toInt(HttpStatus::Code::OK))
			ret = os.str();

		PrintReplyStatus("Webcommand_GetDBSetting", https_status_code);
		if (HttpStatus::isError(https_status_code))
			return -1;
	} catch (curlpp::RuntimeError& e) {
		LOG_ERROR << e.what();
		return -1;
	} catch (curlpp::LogicError& e) {       
		LOG_ERROR << e.what();
		return -1;
	}
	
	return 0;
}

int WebCommand_UpdateDeviceAlias(const std::string& device_name, const std::string& alias)
{
	try {
		string token;
		if (WebCommand_GetToken(token) != 0) {
			LOG_ERROR << "Web socket authentication fail.";
			return -1;
		}

		curlpp::Cleanup cleaner;
		curlpp::Easy request;

		// URL
		string url = string(device_api_addr) + "/DeviceAPI/" + device_name + "/Alias";
		request.setOpt(new curlpp::options::Url(url));
		// custom request
		request.setOpt(new curlpp::options::CustomRequest{"PUT"});
		// header
		list<string> headers;
		headers.push_back("cache-control: no-cache");
		headers.push_back("content-type: application/json");
		headers.push_back("token:" + token);
		request.setOpt(new curlpp::options::HttpHeader(headers));
		// body
		request.setOpt(new curlpp::options::PostFields(alias));
      	request.setOpt(new curlpp::options::PostFieldSize(alias.length()));
		// user agent
		request.setOpt(new curlpp::options::UserAgent("libcurl-agent/1.0"));
		// get response
		ostringstream os;
		request.setOpt(new curlpp::options::WriteStream(&os));

#ifdef DEBUG
		// more information
		request.setOpt(new curlpp::options::Verbose(true));
#endif
		request.perform();

		int https_status_code = static_cast<int>(curlpp::infos::ResponseCode::get(request));
	
		PrintReplyStatus("WebCommand_UpdateDeviceAlias", https_status_code);
		if (HttpStatus::isError(https_status_code))
			return -1;
	} catch (curlpp::RuntimeError& e) {
		LOG_ERROR << e.what();
		return -1;
	} catch (curlpp::LogicError& e) {       
		LOG_ERROR << e.what();
		return -1;
	}
	
	return 0;
}