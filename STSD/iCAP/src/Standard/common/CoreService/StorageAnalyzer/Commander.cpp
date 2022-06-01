#include <cstdio>
#include <iostream>
#include <memory>
#include <stdexcept>
#include <string>
#include <array>
#include "Commander.hpp"
#include "Logger.hpp"

std::string exec(const char* cmd)
{
    std::array<char, 128> buffer;
    std::string result;
    std::unique_ptr<FILE, decltype(&pclose)> pipe(popen(cmd, "r"), pclose);
    
    if (!pipe)
        throw std::runtime_error("popen() failed!");
    
    while (fgets(buffer.data(), buffer.size(), pipe.get()) != nullptr)
        result += buffer.data();

    return result;
}

bool CheckPort(const IP_PORT_SET& ip_port, const int& timeout)
{
    try {
        for (auto it = ip_port.begin(); it != ip_port.end(); ++it) {
            std::string cmd = "nc -z -v -w " + std::to_string(timeout) + " " + it->first + " " + it->second + " 2>&1";
            std::string result = exec(cmd.c_str());
            LOG_INFO << "Check port " << it->first << ":" << it->second << ", result:" << result;
            
            std::size_t found = result.find("succeeded!");
            if (found == std::string::npos)
                return false;
        }
    } catch (...) {
        LOG_ERROR << "Check port failed!";
        return false;
    }
    return true;
}