#include <iostream>
#include "Logger.hpp"

int main(void)
{
    BOOST_LOG_NAMED_SCOPE("EXAMPLE");
    LOG_TRACE << "trace";
    LOG_DEBUG << "debug";
    LOG_INFO << "info";
    LOG_WARN << "warning";
    LOG_ERROR << "error";
    LOG_FATAL << "fatal";

    return 0;
}
