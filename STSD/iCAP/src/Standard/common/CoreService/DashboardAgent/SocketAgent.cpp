#include <netinet/in.h> // sockaddr_in
#include <jsoncpp/json/json.h>
#include <sys/select.h>
#include <sys/socket.h>
#include <atomic>
#include <chrono>
#include <thread>
#include "Logger.hpp"
#include "main.hpp"
#include "SocketAgent.hpp"
#include "WidgetProcessor.hpp"

using namespace std;

#define PORT    			8787
#define PACKAGE_HEADER_LEN 	14 // Header + Length + Checksum
#define CMD_SCREENSHOT		0x01
#define CMD_DELETE_IMG		0x02
#define CMD_DATE			0x03
#define CMD_NEW_WIDGET		0x04
#define CMD_UPDATE_TH		0x05
#define CMD_UPDATE_SMTP		0x06
#define CMD_REPORT			0x07
#define CMD_DB				0x08

#pragma pack(1) 
struct PACKAGE {
	char Header[5];
	char Command[2];
	short Length;
	char CheckSum[5];
	char Payload[100];

	PACKAGE() : Length(0) {}
};
#pragma pack(0)

extern atomic<bool> running;

int CheckPackage(PACKAGE* input_buffer);
PACKAGE* GenerateStatusPackage(int status);

int CheckNewSettings(void)
{ 	
	BOOST_LOG_NAMED_SCOPE(SCOPE_NAME);
	/*
	 * non-blocking socket
	 */
	struct sockaddr_in server_addr;
	socklen_t len;
    fd_set active_fd_set;
    int sock_fd, max_fd;
    int flag = 1;

	memset(&server_addr, 0, sizeof(server_addr));
    server_addr.sin_family = AF_INET;
    server_addr.sin_addr.s_addr = INADDR_ANY;
    server_addr.sin_port = htons(PORT);
    len = sizeof(struct sockaddr_in);

	// Create endpoint
    if ((sock_fd = socket(PF_INET, SOCK_STREAM, 0)) == -1) {
		LOG_ERROR << "Fail to create socket.";
        return -1;
    }

	// Set socket option
    if (setsockopt(sock_fd, SOL_SOCKET, SO_REUSEADDR, &flag, sizeof(int)) < 0) {
		LOG_ERROR << "Fail to set socket option.";
        return -1;
    }	
	
	// Bind
    if (bind(sock_fd, (struct sockaddr *)&server_addr, sizeof(server_addr)) < 0) {
        LOG_ERROR << "Fail to bind socket address.";
        return -1;
    }

	// Listen
    if (listen(sock_fd, 10) == -1) {
		LOG_ERROR << "Fail to listen socket.";
        return -1;
    }
 
	FD_ZERO(&active_fd_set);
    FD_SET(sock_fd, &active_fd_set);
    max_fd = sock_fd;

	WidgetProcessor widget_processor;
	do {
		int ret;
        struct timeval tv = {2, 0}; // 2 seconds
 
        // Copy fd set
        fd_set read_fds = active_fd_set;
        ret = select(max_fd + 1, &read_fds, NULL, NULL, &tv);
        if (ret == -1) {
			LOG_ERROR << "socket select error";
            return -1;
        } else if (ret == 0) {
			// select timeout
			continue;
		} else {
            /* Service all sockets */
			for (int i = 0; i < FD_SETSIZE; i++) {
				if (FD_ISSET(i, &read_fds)) {
					if (i == sock_fd) {
                        // Accept
                    	int new_fd = accept(sock_fd, (struct sockaddr *)&server_addr, &len);
						if (new_fd != -1) {
							LOG_DEBUG << "Accpet, new fd:";
                            // Add to fd set
                            FD_SET(new_fd, &active_fd_set);
                            if (new_fd > max_fd)
								max_fd = new_fd;
                        }
                    } else {
                        /* Data arriving on an already-connected socket */
                        // Receive
						PACKAGE input_package;
                        int recv_len = recv(i, (void*)&input_package, sizeof(input_package), 0);
                        if (recv_len == -1) {
							LOG_ERROR << "Receive error";
                        } else if (recv_len == 0) {
							LOG_ERROR << "Client disconnect";
                        } else {
                            LOG_DEBUG << "Get new widget setting command.";
							int result = CheckPackage(&input_package);
					
							PACKAGE *spackage;
							if (result > 0)
								spackage = GenerateStatusPackage(1);
							else
								spackage = GenerateStatusPackage(0);
							
							int write_len = send(i, spackage, sizeof(PACKAGE), 0);
							delete spackage;

							if (write_len != sizeof(PACKAGE)) {
								LOG_ERROR << "Fail to write status package.";
							} else {
								LOG_DEBUG << "Successful writing status package.";
								if (result == CMD_NEW_WIDGET)
									widget_processor.CalculateWidgets(true);
							}
						}
 
                        // Clean up
                        close(i);
                        FD_CLR(i, &active_fd_set);
                    }
                }
            }
        }
	} while (running.load(memory_order_relaxed));

	close(sock_fd);

	return 0;
}

int CheckPackage(PACKAGE* input_buffer)
{
	LOG_DEBUG << "Get package Header:" << input_buffer->Header;
	LOG_DEBUG << "Get package Command:" << input_buffer->Command;
	LOG_DEBUG << "Get package Length:" << input_buffer->Length;
	LOG_DEBUG << "Get package CheckSum:" << input_buffer->CheckSum;
	LOG_DEBUG << "Get package Payload:" << input_buffer->Payload;

    int length = 4 + 1 + 2 + strlen(input_buffer->Payload) + strlen(input_buffer->CheckSum);
    unsigned int checksum = 0;
    char checkall[sizeof(input_buffer->CheckSum)];
	char buffer[sizeof(PACKAGE)];
    
	memset(&buffer, 0, sizeof(buffer));
    strcpy(buffer, input_buffer->Header);
    strcat(buffer, input_buffer->Command);
    strcat(buffer, input_buffer->Payload);
	
	for (unsigned int i = 0; i < sizeof(buffer); i++)
        checksum += buffer[i];
     snprintf(checkall, sizeof(input_buffer->CheckSum), "%X", checksum & 0x0000FFFF);
    
	if ((length != input_buffer->Length) || (strcmp(checkall, input_buffer->CheckSum) != 0)) {
		LOG_ERROR << "Wrong package, drop...";
        return -1;
	}

	if ((strcmp(input_buffer->Header, "INNO") != 0) || (strcmp(input_buffer->Command, "D") != 0)) {
		LOG_ERROR << "Wrong header or command, drop...";
        return -1;
	}

	if (strcmp(input_buffer->Payload, "WIDGET/ADD") == 0) {
		return CMD_NEW_WIDGET;
	} else {
		LOG_ERROR << "Wrong payload, drop...";
        return -1;
	}
	
	return 0;
}

PACKAGE* GenerateStatusPackage(int status)
{
    PACKAGE *spackage = new PACKAGE;
    strncpy(spackage->Header, "INNO", sizeof(spackage->Header));
    strncpy(spackage->Command, "D", sizeof(spackage->Command));
    strcpy(spackage->Payload, to_string(status).c_str());
    
	char buffer[sizeof(PACKAGE)];
    memset(buffer, 0, sizeof(buffer));
    strcpy(buffer, "INNOD");
    strcat(buffer, spackage->Payload);
	unsigned int checksum = 0;
    for (size_t i = 0; i < sizeof(buffer); i++)
		checksum += buffer[i];
    snprintf(spackage->CheckSum, sizeof(spackage->CheckSum), "%X", checksum & 0x0000FFFF);

	spackage->Length = 4 + 1 + 2 + strlen(spackage->Payload) + strlen(spackage->CheckSum);
    
	LOG_DEBUG << "Send spackage Header:" << spackage->Header;
	LOG_DEBUG << "Send spackage Command:" << spackage->Command;
	LOG_DEBUG << "Send spackage Length:" << spackage->Length;
	LOG_DEBUG << "Send spackage CheckSum:" << spackage->CheckSum;
	LOG_DEBUG << "Send spackage Payload:" << spackage->Payload;
    
    return spackage;
}