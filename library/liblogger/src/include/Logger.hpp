#ifndef _LOGGER_H_
#define _LOGGER_H_

#pragma once

#include <boost/log/attributes/named_scope.hpp>
#include <boost/log/core.hpp>
#include <boost/log/expressions.hpp>
#include <boost/log/sources/global_logger_storage.hpp>
#include <boost/log/support/date_time.hpp>
#include <boost/log/support/exception.hpp>
#include <boost/log/trivial.hpp>
#include <boost/log/utility/manipulators/add_value.hpp>
#include <boost/log/utility/setup.hpp>
#include <boost/shared_ptr.hpp>


#define ANSI_CLEAR      "\x1B[0;00m"
#define ANSI_RED        "\x1B[1;31m"
#define ANSI_GREEN      "\x1B[0;32m"
#define ANSI_YELLOW     "\x1B[1;33m"
#define ANSI_BLUE       "\x1B[0;34m"
#define ANSI_MAGENTA    "\x1B[0;35m"
#define ANSI_CYAN       "\x1B[0;36m"
#define ANSI_RED_BG     "\x1B[1;41m"

#define LOG_TRACE  BOOST_LOG_SEV(my_logger::get(), boost::log::trivial::trace) \
  << boost::log::add_value("Line", __LINE__) \
  << boost::log::add_value("File", __FILE__)
#define LOG_DEBUG  BOOST_LOG_SEV(my_logger::get(), boost::log::trivial::debug) \
  << boost::log::add_value("Line", __LINE__) \
  << boost::log::add_value("File", __FILE__)
#define LOG_INFO  BOOST_LOG_SEV(my_logger::get(), boost::log::trivial::info) \
  << boost::log::add_value("Line", __LINE__) \
  << boost::log::add_value("File", __FILE__)
#define LOG_WARN  BOOST_LOG_SEV(my_logger::get(), boost::log::trivial::warning) \
  << boost::log::add_value("Line", __LINE__) \
  << boost::log::add_value("File", __FILE__)
#define LOG_ERROR BOOST_LOG_SEV(my_logger::get(), boost::log::trivial::error) \
  << boost::log::add_value("Line", __LINE__) \
  << boost::log::add_value("File", __FILE__)
#define LOG_FATAL BOOST_LOG_SEV(my_logger::get(), boost::log::trivial::fatal) \
  << boost::log::add_value("Line", __LINE__) \
  << boost::log::add_value("File", __FILE__)

//Narrow-char thread-safe logger.
typedef boost::log::sources::severity_logger_mt<boost::log::trivial::severity_level> logger_t;

//declares a global logger with a custom initialization
BOOST_LOG_GLOBAL_LOGGER(my_logger, logger_t)

#endif // End of _LOGGER_H_