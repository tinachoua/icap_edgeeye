#include <netinet/in.h> // sockaddr_in
#include <jsoncpp/json/json.h>
#include <cstring>
#include <errno.h>
#include <sys/select.h>
#include <sys/socket.h>
#include <atomic>
#include <fstream>
#include <future>
#include <list>
#include <mutex>
#include <thread>
#include "Logger.hpp"
#include "main.hpp"
#include "MQTTAgent.hpp"
#include "Nosqlcommand.hpp"
#include "SocketAgent.hpp"

using namespace std;

#define PORT    			8787
#define PACKAGE_HEADER_LEN	14 // Header + Length + Checksum
#define CMD_SCREENSHOT		0x01
#define CMD_DELETE_IMG		0x02
#define CMD_DATE			0x03
#define CMD_NEW_WIDGET		0x04
#define CMD_UPDATE_TH		0x05
#define CMD_UPDATE_SMTP		0x06
#define CMD_REPORT			0x07
#define CMD_DB				0x08

extern atomic<bool> running;
mutex PromiseListMutex;
map<string, promise<IMG_DATA_PAIR>*> PromiseList;

int CheckPackage(PACKAGE_IN* input_buffer);
string GetDevNamefromPackage(PACKAGE_IN* input_buffer);
int ProcessDeleteImgCmd(PACKAGE_IN* input_buffer);
PACKAGE_OUT* GenerateScreenshotResponsePackage(const string& data, const long long& id);
PACKAGE_OUT* GenerateStatusResponsePackage(const int& response_status);
int SendAll(int s, const char *buf, size_t *len);

int CommandListener(void)
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
							LOG_DEBUG << "Accpet, new fd:" << new_fd;

                            // Add to fd set
                            FD_SET(new_fd, &active_fd_set);
                            if (new_fd > max_fd)
								max_fd = new_fd;
                        }
                    } else {
                        /* Data arriving on an already-connected socket */
                        // Receive
						PACKAGE_IN input_package;
                        int recv_len = recv(i, (void*)&input_package, sizeof(input_package), 0);
                        if (recv_len == -1) {
							LOG_ERROR << "Receive error";
                        } else if (recv_len == 0) {
							LOG_ERROR << "Client disconnect";
                        } else {
                            LOG_DEBUG << "Get new command from web service.";
							
							int command_type = CheckPackage(&input_package);
							if (command_type == CMD_SCREENSHOT) {
								promise<IMG_DATA_PAIR> promise_obj;
								future<IMG_DATA_PAIR> future_obj = promise_obj.get_future();
								string dev_name = GetDevNamefromPackage(&input_package);
								PromiseListMutex.lock();
								PromiseList.insert(pair<string, promise<IMG_DATA_PAIR>*>(dev_name, &promise_obj));
								PromiseListMutex.unlock();

								Json::Value payload_obj;
								payload_obj["Cmd"] = "ScreenShot";
								payload_obj["ID"] = dev_name;
								MQTTAgent_PublishData("Remote", payload_obj.toStyledString().c_str(), 1);
								future_status status;
								string img_str;
								long long id;
								do {
									status = future_obj.wait_for(std::chrono::seconds(5));
									if (status == future_status::deferred) {
										LOG_DEBUG << "deferred";
									} else if (status == std::future_status::timeout) {
										LOG_DEBUG << "timeout";
										break;
									} else if (status == std::future_status::ready) {
										LOG_DEBUG << "ready!";
										auto img_data = future_obj.get();
										img_str = img_data.first;
										id = img_data.second;
									}
								} while (status != std::future_status::ready); 

								PromiseListMutex.lock();
								PromiseList.erase(dev_name);
								PromiseListMutex.unlock();

								PACKAGE_OUT *output_package = GenerateScreenshotResponsePackage(img_str, id);

								size_t write_len = send(i, output_package, PACKAGE_HEADER_LEN, 0);

								if (write_len != PACKAGE_HEADER_LEN) {
									LOG_ERROR << "Fail to write response header.";
								} else {
									write_len = output_package->payload.length();
									int result = SendAll(i, output_package->payload.c_str(), &write_len);
									if (result < 0 || write_len != output_package->payload.length()) {
										LOG_ERROR << "Fail to write response payload, result=" << result 
											<< ", write_len=" << write_len;
									}
								}
								delete output_package;
							} else if (command_type == CMD_DELETE_IMG) {
								int status = ProcessDeleteImgCmd(&input_package);
								
								PACKAGE_OUT *output_package = GenerateStatusResponsePackage(status);
								
								size_t write_len = send(i, output_package, PACKAGE_HEADER_LEN, 0);

								if (write_len != PACKAGE_HEADER_LEN) {
									LOG_ERROR << "Fail to write response header.";
								} else {
									write_len = output_package->payload.length();
									int result = SendAll(i, output_package->payload.c_str(), &write_len);
									if (result < 0 || write_len != output_package->payload.length()) {
										LOG_ERROR << "Fail to write response payload, result=" << result 
											<< ", write_len=" << write_len;
									}
								}
								delete output_package;
							}
						}
 
                        // Clean up
                        close(i);
                        FD_CLR(i, &active_fd_set);
                    }
                }
            }
        }
		this_thread::sleep_for(chrono::milliseconds(500));
	} while (running.load(memory_order_relaxed));

	close(sock_fd);

	return 0;
}

int CheckPackage(PACKAGE_IN* input_buffer)
{
	LOG_DEBUG << "Get package header:" << input_buffer->header;
	LOG_DEBUG << "Get package length:" << input_buffer->length;
	LOG_DEBUG << "Get package checksum:" << input_buffer->checksum;
	LOG_DEBUG << "Get package payload:" << input_buffer->payload;

	// check header
	if (strcmp(input_buffer->header, "INNO") != 0) {
		LOG_ERROR << "Wrong package header, drop...";
        return -1;
	}

    // check length
	unsigned int length = sizeof(input_buffer->header) + sizeof(input_buffer->checksum) + sizeof(input_buffer->payload)
							+ sizeof(input_buffer->length);
	if (length != input_buffer->length) {
		LOG_ERROR << "The expected package length is " << length << ".";
		LOG_ERROR << "Wrong package length, drop...";
        return -1;
	}
    
	// check checksum
	unsigned int checksum = 0;
	for (size_t i = 0; i < strlen(input_buffer->header); i++)
        checksum += input_buffer->header[i];
	for (size_t i = 0; i < strlen(input_buffer->payload); i++)
        checksum += input_buffer->payload[i];
	char checkall[sizeof(input_buffer->checksum)];
    snprintf(checkall, sizeof(checkall), "%X", checksum & 0x0000FFFF);
	if (strcmp(checkall, input_buffer->checksum) != 0) {
		LOG_ERROR << "Wrong package checksum, drop...";
        return -1;
	}

	Json::Value payload;
	Json::Reader reader;
	if (!reader.parse((string)input_buffer->payload, payload)) {
		LOG_ERROR << "Failed to parse package payload." << reader.getFormattedErrorMessages();
		return -1;
	}

	return payload["Cmd"].asInt(); // return type of command
}

string GetDevNamefromPackage(PACKAGE_IN* input_buffer)
{
	Json::Value payload;
	Json::Reader reader;
	if (!reader.parse((string)input_buffer->payload, payload)) {
		LOG_ERROR << "Failed to parse screenshot command." << reader.getFormattedErrorMessages();
		exit(-1);
	}

	return payload["DevName"].asString();
}

int ProcessDeleteImgCmd(PACKAGE_IN* input_buffer)
{
	Json::Value payload, doc_obj;
	Json::Reader reader;
	if (!reader.parse((string)input_buffer->payload, payload)) {
		LOG_ERROR << "Failed to parse delete image command." << reader.getFormattedErrorMessages();
		return 0;
	}

	// 1. Query Screenshot collection and get document
	string db_data;
	if ((QueryScreenshotDataFromDB(payload["DevName"].asString(), db_data) != 0) || db_data.empty()) {
		LOG_WARN << "No " << payload["DevName"].asString() << "in Screenshot DB.";
		return 0;
	}

	if (!reader.parse(db_data, doc_obj)) {
		LOG_ERROR << "Failed to parse image data." << reader.getFormattedErrorMessages();
		return 0;
	}
	
	for (Json::Value::ArrayIndex i = 0; i < payload["ImgId"].size(); i++) {
		for (Json::Value::ArrayIndex j = 0; j < doc_obj["Images"].size(); j++) {
			if (payload["ImgId"][i].asInt64() == doc_obj["Images"][j]["Id"].asInt64()) {
				// 2. Get path from DB
				string path = doc_obj["Images"][j]["Path"].asString();

				bool failed = !ifstream(path.c_str());
    			if (failed) { 
					LOG_ERROR << "Error opening deleted file"; 
					return 2;
				} else {
					// 3. Delete local image
					if (remove(path.c_str()) != 0)
						return 0;
				}
				// 4. Delete image object form DB
				if (DeleteScreenshotDataFromDB(payload["DevName"].asString(), doc_obj["Images"][j]["Id"].asInt64()) != 0)
					return 0;
			}
		}
	}
	
	return 1;
}

PACKAGE_OUT* GenerateScreenshotResponsePackage(const string& data, const long long& id)
{
    PACKAGE_OUT *output_package = new PACKAGE_OUT;

	strncpy(output_package->header, "INNO", sizeof(output_package->header));
	
	Json::Value payload_obj;
	payload_obj["Image"] = data;
	payload_obj["Time"] = static_cast<long long>(time(nullptr));
	payload_obj["Id"] = id;
	output_package->payload = payload_obj.toStyledString();

	unsigned int checksum = 0;
    for (size_t i = 0; i < strlen(output_package->header); i++)
		checksum += output_package->header[i];
	for (size_t i = 0; i < output_package->payload.length(); i++)
		checksum += output_package->payload[i];
    snprintf(output_package->checksum, sizeof(output_package->checksum), "%X", checksum);
	output_package->checksum[sizeof(output_package->checksum) - 1] = '\0';

	output_package->length = sizeof(output_package->header) + sizeof(output_package->checksum)
							+ output_package->payload.length() + sizeof(output_package->length);
    
	LOG_DEBUG << "Send output_package header:" << output_package->header;
	LOG_DEBUG << "Send output_package length:" << dec << output_package->length;
	LOG_DEBUG << "Send output_package checksum:" << output_package->checksum;
	LOG_DEBUG << "Send output_package payload:" << output_package->payload;
    
    return output_package;
}

PACKAGE_OUT* GenerateStatusResponsePackage(const int& response_status)
{
    PACKAGE_OUT *output_package = new PACKAGE_OUT;

	strncpy(output_package->header, "INNO", sizeof(output_package->header));
	
	Json::Value payload_obj;
	payload_obj["Status"] = response_status;
	output_package->payload = payload_obj.toStyledString();

	unsigned int checksum = 0;
    for (size_t i = 0; i < strlen(output_package->header); i++)
		checksum += output_package->header[i];
	for (size_t i = 0; i < output_package->payload.length(); i++)
		checksum += output_package->payload[i];
    snprintf(output_package->checksum, sizeof(output_package->checksum), "%X", checksum);
	output_package->checksum[sizeof(output_package->checksum) - 1] = '\0';

	output_package->length = sizeof(output_package->header) + sizeof(output_package->checksum)
							+ output_package->payload.length() + sizeof(output_package->length);
    
	LOG_DEBUG << "Send output_package header:" << output_package->header;
	LOG_DEBUG << "Send output_package length:" << dec << output_package->length;
	LOG_DEBUG << "Send output_package checksum:" << output_package->checksum;
	LOG_DEBUG << "Send output_package payload:" << output_package->payload;
    
    return output_package;
}

int SendAll(int s, const char *buf, size_t *len)
{
	size_t total = 0; // how many bytes we've sent
	size_t bytesleft = *len; // how many we have left to send
	int bytes_sent = -1;
	while (total < *len) {
		bytes_sent = send(s, buf + total, bytesleft, 0);
		if (bytes_sent == -1) {
			LOG_ERROR << "Failed to send data, error:" << strerror(errno);
			break;
		}
		total += bytes_sent;
		bytesleft -= bytes_sent;
	}
	*len = total; // return number actually sent here
	return bytes_sent == -1 ? -1 : 0; // return -1 on failure, 0 on success
}