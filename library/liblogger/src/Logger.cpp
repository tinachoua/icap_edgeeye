#include <algorithm>
#include <ctime>
#include <fstream>
#include <iostream>
#include <string>
#include <boost/predef.h>
#include "Logger.hpp"

using namespace std;

namespace attrs     = boost::log::attributes;
namespace expr      = boost::log::expressions;
namespace keywords  = boost::log::keywords;
namespace logging   = boost::log;
namespace sinks     = boost::log::sinks;
namespace trivial   = boost::log::trivial;

#ifdef PATH
    string log_dir = PATH;
#else
    string log_dir = "iCAP";
#endif

bool is_enable_log()
{
    string setting_path;

    if (BOOST_OS_WINDOWS)
        setting_path = "c:\\Program Files\\" + log_dir + "_Server\\setting.env";
    else
        setting_path = "/var/" + log_dir + "/setting.env";

    

    std::ifstream cFile(setting_path);
    string disable_log = "true";
    
    if (cFile.is_open()) {
        string line;
        while (std::getline(cFile, line)) {
            line.erase(std::remove(line.begin(), line.end(), ' '),
               line.end());
            if(line[0] == '#' || line.empty())
                continue;
            auto delimiterPos = line.find("=");
            auto name = line.substr(0, delimiterPos);
            auto value = line.substr(delimiterPos + 1);
            if (name == "DISABLE_LOG")
                disable_log = value;
        }
    } else {
        cerr << "Couldn't open config file for reading." << endl;
        return false;
    }

    if (disable_log == "true")
        return false;
    
    return true;
}

void consoleColorFormatter(logging::record_view const& rec, logging::formatting_ostream& strm)
{
    auto now = time(nullptr);
    auto* gmtm = gmtime(&now);
    stringstream sstream;
    sstream << put_time(gmtm, "%c");
    strm << "[" << sstream.str() << "] ";
    const auto thid_vt = logging::extract<attrs::current_thread_id::value_type>("ThreadID", rec).get();
    strm << "(" << thid_vt << ") ";

    const auto sl = logging::extract<trivial::severity_level>("Severity", rec).get();
    switch (sl) 
    {
        case trivial::trace:
            strm << ANSI_CLEAR << "[" << sl << "]   ";
            break;
        case trivial::debug:
            strm << ANSI_BLUE << "[" << sl << "]   ";
            break;
        case trivial::info:
            strm << ANSI_GREEN << "[" << sl << "]    ";
            break;
        case trivial::warning:
            strm << ANSI_YELLOW << "[" << sl << "] ";
            break;
        case trivial::error:
            strm << ANSI_RED << "[" << sl << "]   ";
            break;
        case trivial::fatal:
            strm << ANSI_RED_BG << "[" << sl << "]   ";
            break;
        default:
            break;
    }
    
    const auto scope_vt = logging::extract<attrs::named_scope::value_type>("Scope", rec);
    if (!scope_vt.empty()) 
    {
        for (auto& scope : scope_vt.get()) {
            strm << "[" << scope.scope_name << "]";
        }
    } 
    
    logging::value_ref<string> fullpath = logging::extract<string>("File", rec);
    strm << "(" << boost::filesystem::path(fullpath.get()).filename().string() << ":";
    strm << logging::extract<int>("Line", rec) << ") ";
    strm << rec[expr::smessage] << ANSI_CLEAR;
}

void textFileFormat(logging::record_view const& rec, logging::formatting_ostream& strm)
{
    // if (is_enable_log())
    //     logging::core::get()->set_logging_enabled(true);
    // else
    //     logging::core::get()->set_logging_enabled(false);

    auto now = time(nullptr);
    auto* gmtm = gmtime(&now);
    stringstream sstream;
    sstream << put_time(gmtm, "%c");
    strm << "[" << sstream.str() << "] ";

    const auto sl = logging::extract<trivial::severity_level>("Severity", rec).get();
    switch (sl) 
    {
        case trivial::trace:
            strm << "[" << sl << "]   ";
            break;
        case trivial::debug:
            strm << "[" << sl << "]   ";
            break;
        case trivial::info:
            strm << "[" << sl << "]    ";
            break;
        case trivial::warning:
            strm << "[" << sl << "] ";
            break;
        case trivial::error:
            strm << "[" << sl << "]   ";
            break;
        case trivial::fatal:
            strm << "[" << sl << "]   ";
            break;
        default:
            break;
    }
    
    const auto scope_vt = logging::extract<attrs::named_scope::value_type>("Scope", rec);
    if (!scope_vt.empty()) 
    {
        for (auto& scope : scope_vt.get()) {
            strm << "[" << scope.scope_name << "]";
        }
    }

    strm << rec[expr::smessage];
}

//Defines a global logger initialization routine
BOOST_LOG_GLOBAL_LOGGER_INIT(my_logger, logger_t)
{    
    logger_t lg;

    if (is_enable_log())
        logging::core::get()->set_logging_enabled(true);
    else
        logging::core::get()->set_logging_enabled(false);

    logging::add_common_attributes();
    logging::core::get()->add_global_attribute("Scope", attrs::named_scope());
    logging::core::get()->set_filter
    (
#ifdef DEBUG
        logging::trivial::severity >= logging::trivial::debug
#else
        logging::trivial::severity >= logging::trivial::info
#endif
    );

#ifdef DEBUG
    /* console sink */
    using boost::shared_ptr;
    typedef sinks::text_ostream_backend Backend;
    typedef sinks::synchronous_sink<Backend> Sink;

    shared_ptr<Sink> sink = logging::add_console_log(std::clog);
    sink->set_formatter(&consoleColorFormatter);
#endif

    /* file sink */
    string file_name;
    if (BOOST_OS_WINDOWS) {
        file_name = "c:\\Program Files\\" + log_dir + "_Server\\Log\\core_%Y%m%d.log";
    } else {
        file_name = "/var/" + log_dir + "/Log/core_%Y%m%d.log";
    }

    auto fsSink = logging::add_file_log(
        keywords::file_name = file_name,
        keywords::time_based_rotation = sinks::file::rotation_at_time_point(0, 0, 0),
        keywords::open_mode = std::ios_base::app);
    fsSink->set_formatter(&textFileFormat);
    fsSink->locked_backend()->auto_flush(true);

    return lg;
}