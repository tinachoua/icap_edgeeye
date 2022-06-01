#ifndef RH_SOCKETAGENT_H_
#define RH_SOCKETAGENT_H_

#pragma pack(1) 
struct PACKAGE_IN {
	char header[5]; 		// "INNO"
	unsigned int length;	// size of (header + checksum + payload + length)
	char checksum[5];		// checksum of (header + payload)
	char payload[100];

	PACKAGE_IN() : length(0) {}
};
#pragma pack(0)

#pragma pack(1) 
struct PACKAGE_OUT {
	char header[5]; 			// "INNO"
	unsigned int length; 		// size of (header + checksum + payload + length)
	char checksum[5]; 			// checksum of (header + payload)
	std::string payload;

	PACKAGE_OUT() : length(0) {}
};
#pragma pack(0)

int CommandListener(void);

#endif // RH_SOCKETAGENT_H_