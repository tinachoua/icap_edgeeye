#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <sys/wait.h>
#include <netinet/in.h>
#include <ctime>
#include <fcntl.h>       //File control defs
#include <errno.h>
#include <stdbool.h>
#include <iostream>
#include <fstream>
#include <string>
#include "hasp_api.h"     //key pro header
#include "Keypro_Read.h"

using namespace std;

#pragma pack(1) 
typedef struct {
	char Header[5];
	char Command[2];
	short Length;
	char CheckSum[5];
	char Payload[50];
}PACKAGE;
#pragma pack(0)

int CheckPackage(PACKAGE* inputBuffer)
{
    int Length = 4 + 1 + 2 + strlen(inputBuffer->Payload) + strlen(inputBuffer->CheckSum);
    unsigned int checksum = 0;
    char checkall[sizeof(inputBuffer->CheckSum)];
    char* buffer = (char*)malloc(sizeof(char) * 255);
    
	memset(buffer, 0, sizeof(char) * 255);
    strcpy(buffer, inputBuffer->Header);
    strcat(buffer, inputBuffer->Command);
    strcat(buffer, inputBuffer->Payload);
	
	for (unsigned int i = 0; i < (sizeof(char) * 255); i++)
    {
        checksum += *(buffer + i);
    }
    snprintf(checkall, sizeof(inputBuffer->CheckSum), "%X", checksum & 0x0000FFFF);

    if ((Length = inputBuffer->Length) && (strcmp(checkall, inputBuffer->CheckSum) == 0))
    {
		printf("%s: right package.\n", __func__);
        return 0;
    }
    else
    {
        printf("%s: wrong package,drop.\n", __func__);
        return -1;
    }
	
	if (buffer) {
		free(buffer);
		buffer = NULL;
	}
}

char* read_license() {
	string line;
	ifstream licensefile("./license.dat");

	if (licensefile.is_open())
	{
		getline(licensefile, line);
		licensefile.close();
	}
	else {
		cout << "ERROR: Cannot open license.dat" << endl;
		return NULL;
	}

	string count = line.substr(0, 2);
	size_t sz;
	char *license = (char*)malloc(sizeof(char) * 100);

	memcpy(license, line.data(), stoi(count, &sz) + 2);

	return license;
}

PACKAGE* GenerateStatusPackage(int status)
{
	PACKAGE* lpackage = (PACKAGE*)malloc(sizeof(lpackage) * 255);
	snprintf(lpackage->Header, sizeof(lpackage->Header), "INNO");
	snprintf(lpackage->Command, sizeof(lpackage->Command), "L");
    char* buffer = (char*)malloc(sizeof(char) * 255);
    memset(buffer, 0, 255);
    
	if (status == 0)
	{
#ifdef FAKE_DATA
		const char* fake_license = "24#12345678#100#1517817311";
		strcpy(lpackage->Payload, fake_license);
#elif STRESS
		const char* stress_license = "26#12345678#65536#1517817311";
		strcpy(lpackage->Payload, stress_license);
#else
		char *license = read_license();
		strcpy(lpackage->Payload, license);

		if (license) {
			free(license);
			license = NULL;
		}
#endif
		strcpy(buffer, "INNOL");
		strcat(buffer, lpackage->Payload);
    }
    
	if (status == -1)
    {
		lpackage->Payload[0] = '\0';
    }
    	
	unsigned int checksum = 0;
    
	for (int i = 0; i < 255; i++)
    {
		checksum += *(buffer + i);
    }
    
	snprintf(lpackage->CheckSum, sizeof(lpackage->CheckSum), "%X", checksum & 0x0000FFFF);
    lpackage->Length = 4 + 1 + 2 + strlen(lpackage->Payload) + strlen(lpackage->CheckSum);
#ifdef DEBUG
	printf("Send lPackage Header:%s\n", lpackage->Header);
    printf("Send lPackage Command:%s\n", lpackage->Command);
    printf("Send lPackage Length:%d\n", lpackage->Length);
    printf("Send lPackage CheckSum:%s\n", lpackage->CheckSum);
    printf("Send lPackage Payload:%s\n", lpackage->Payload);
#endif

	if (buffer) {
		free(buffer);
		buffer = NULL;
	}

    return lpackage;
}

PACKAGE* Statuspkg_gen(int status)
{
    PACKAGE* spackage=(PACKAGE*)malloc(sizeof(spackage) * 255);
    snprintf(spackage->Header, sizeof(spackage->Header), "INNO");
    snprintf(spackage->Command, sizeof(spackage->Command), "S");
    long int timestamp;
    timestamp = (long int)time(NULL);
    
	if (status == -1)
    {
        spackage->Payload[0] = '0';
    }
    if (status == 0)
    {
        spackage->Payload[0] = '1';
    }

    char timestamp_buff[20];
    sprintf(timestamp_buff,"%ld", timestamp);
    strcat(spackage->Payload, timestamp_buff);
    spackage->Length = 4 + 1 + 2 + strlen(spackage->Payload) + strlen(spackage->CheckSum);
    char* buffer = (char*)malloc(sizeof(char) * 255);
    memset(buffer, 0, 255);
    strcpy(buffer, "INNOS");
    strcat(buffer, spackage->Payload);
    unsigned int checksum = 0;
    for (int i = 0; i < 255; i++)
    {
		checksum += *(buffer + i);
    }
    snprintf(spackage->CheckSum, sizeof(spackage->CheckSum), "%X", checksum & 0x0000FFFF);
#ifdef DEBUG
    printf("Send spackage Header: %s\n", spackage->Header);
    printf("Send spackage Command: %s\n", spackage->Command);
    printf("Send spackage Length: %d\n", spackage->Length);
    printf("Send spackage CheckSum: %s\n", spackage->CheckSum);
    printf("Send spackage Payload: %s\n", spackage->Payload);
#endif
    
	if (buffer) {
		free(buffer);
		buffer = NULL;
	}

    return spackage;
}

int main()
{
    PACKAGE* inputBuffer = (PACKAGE*)malloc(sizeof(inputBuffer) * 255);
    PACKAGE* lpackage = (PACKAGE*)malloc(sizeof(lpackage) * 255);
    PACKAGE* spackage = (PACKAGE*)malloc(sizeof(spackage) * 255);
    int sockfd = 0, forClientSockfd = 0;
    int status;

    sockfd = socket(AF_INET , SOCK_STREAM , 0);
    if (sockfd == -1) {
        printf("Fail to create a socket.");
    }

    struct sockaddr_in serverInfo, clientInfo;
	socklen_t addrlen = sizeof(struct sockaddr_in);
    bzero(&serverInfo,sizeof(serverInfo));

    serverInfo.sin_family = PF_INET;
    serverInfo.sin_addr.s_addr = INADDR_ANY;
    // serverInfo.sin_addr.s_addr = inet_addr("127.0.0.1");
    serverInfo.sin_port = htons(8787);
    bind(sockfd, (struct sockaddr *)&serverInfo, sizeof(serverInfo));
	printf("after bind port:8787\n");
    listen(sockfd, 10);
	// forClientSockfd = accept(sockfd,(struct sockaddr*) &clientInfo, &addrlen);

	while ((forClientSockfd = accept(sockfd, (struct sockaddr*) &clientInfo, &addrlen)) >= 0)
	{
		memset(inputBuffer, 0, sizeof(inputBuffer) * 255);
		memset(lpackage, 0, sizeof(lpackage) * 255);
		memset(spackage, 0, sizeof(spackage) * 255);
		recv(forClientSockfd, inputBuffer, sizeof(inputBuffer) * 255, 0);

		pid_t pid;
		pid = fork();
		
		if (pid == -1)
		{
			close(forClientSockfd);
			break;
		}
        
		if (pid == 0)
		{
			sleep(1);
        	if (CheckPackage(inputBuffer) == 0)
        	{
#ifdef DEBUG
            	printf("Get package Header: %s\n", inputBuffer->Header);
            	printf("Get package Command: %s\n", inputBuffer->Command);
            	printf("Get package Length: %d\n", inputBuffer->Length);
            	printf("Get package CheckSum: %s\n", inputBuffer->CheckSum);
            	printf("Get package Payload: %s\n", inputBuffer->Payload);
#endif
				if (strcmp("L", inputBuffer->Command) == 0)
				{	
#if defined(STRESS) || defined(FAKE_DATA)
					int status = 0;
#else
					int status = check_keypro_status();
#endif
					if (status == -1)
                    {
						lpackage = GenerateStatusPackage(status);
                        send(forClientSockfd, lpackage, sizeof(lpackage) * 255, 0);
                        close(forClientSockfd);
						break;
                    }
				
					if (status == 0)
					{
#if defined(STRESS)|| defined(FAKE_DATA)
						lpackage = GenerateStatusPackage(status);
						send(forClientSockfd, lpackage, sizeof(lpackage) * 255, 0);
						close(forClientSockfd);
						break;
#else

						if (Keypro_readlicense() == 0)
						{
							lpackage = GenerateStatusPackage(status);
							send(forClientSockfd, lpackage, sizeof(lpackage) * 255, 0);
							close(forClientSockfd);
					   		break;
						}
						else
						{
							printf("There is no any license\n");
							break;
						}
#endif
					}
				}

				if (strcmp("S", inputBuffer->Command) == 0)
				{
					int status =check_keypro_status();
					if (status == -1)
					{
						spackage = GenerateStatusPackage(status);
						send(forClientSockfd, spackage, sizeof(spackage) * 255, 0);
						close(forClientSockfd);
						break;
					}
					if (status == 0)
					{
						printf("Key is detected.\n");
						spackage = GenerateStatusPackage(status);
						send(forClientSockfd, spackage, sizeof(spackage) * 255, 0);
						close(forClientSockfd);
						break;
                    }
				}
			}
		}
		else
		{
        	waitpid(pid, &status, WNOHANG);
    		if (WIFEXITED(status)) 
        	{
            	WEXITSTATUS(status);
        	}
        	else if (WIFSIGNALED(status))
        	{
            	WTERMSIG(status);
        	}
//        	break;
		}
	} // while

	if (inputBuffer) {
		free(inputBuffer);
		inputBuffer = NULL;
	}
	if (lpackage) {
		free(lpackage);
		lpackage = NULL;
	}
	if (spackage) {
		free(spackage);
		spackage = NULL;
	}

    return 0;
}

