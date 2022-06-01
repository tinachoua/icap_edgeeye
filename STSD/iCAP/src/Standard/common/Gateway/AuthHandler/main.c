#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <signal.h>
#include <unistd.h>
#include "main.h"
#include "JsonParser.h"
#include "hiredis.h"
#include "async.h"
#include "adapters/libevent.h"

static char onExit = 0;
redisAsyncContext *c;

void gotExitCmd(int sig)
{
	redisAsyncDisconnect(c);
	onExit = 1;
}

int ReloadPWD()
{
	char* buf = (char*)malloc(sizeof(char) * 1024);
	char* tok;
	int k;
	FILE* fp;
	
	fp = popen("ps -a | grep -i mosquitto | awk {'print $1'}", "r");

	if (fp == NULL) {
		fprintf(stderr, "[%d]popen fail!\n", __LINE__);
		return -1;
	}

	k = -1;	

	do {
		*(buf+(++k)) = fgetc(fp);
	} while(*(buf+k) != 0xFFFFFFFF);

	*(buf+k) = '\0';
	pclose(fp);

	tok = strtok(buf, " ");

	char* cmd = (char*)malloc(sizeof(char) * 100);
	sprintf(cmd, "kill -HUP %s", tok);
	printf("cmd: %s\n", cmd);
	fp = popen(cmd, "r");
	
	if (fp == NULL) {
		fprintf(stderr, "[%d]popen fail!\n", __LINE__);
		return -1;
	}
	
	pclose(fp);
	free(cmd);
	free(buf);
	return 0;
}

void subCallback(redisAsyncContext *c, void *r, void *priv)
{
    redisReply *reply = r;
    struct json_object *jobj, *account_obj, *pwd_obj;
	char* cmd;
	FILE* fp;
	
	if (reply == NULL) return;
    
	if (reply->type == REDIS_REPLY_ARRAY && reply->elements == 3) {
        if (strcmp(reply->element[0]->str, "subscribe") != 0 ) {
            printf("Received[%s] channel %s: %s\n",
				(char*)priv,
				reply->element[1]->str,
				reply->element[2]->str);
			
			jobj = json_tokener_parse(reply->element[2]->str);
			account_obj = get_json_object(jobj, "account");
			pwd_obj = get_json_object(jobj, "pwd");
			
			cmd = (char*)malloc(sizeof(char) * 200);
			memset(cmd, '\0', sizeof(char) * 200);
			
			sprintf(cmd,
				"mosquitto_passwd -D /etc/mosquitto/passwd %s",
				json_object_get_string(account_obj));
			fp = popen(cmd, "r");
			pclose(fp);
			
			memset(cmd, '\0', sizeof(char) * 200);
			sprintf(cmd,
				"mosquitto_passwd -b /etc/mosquitto/passwd %s %s",
				json_object_get_string(account_obj),
				json_object_get_string(pwd_obj));
			fp = popen(cmd, "r");
			
			pclose(fp);
			free(cmd);
			
			ReloadPWD();	
        }
    }
}

void connectCallback(const redisAsyncContext *c, int status)
{
    if (status != REDIS_OK) {
	    printf("Connection error: %s\n", c->errstr);
        return;
    }
    printf("Connected...\n");
}

void disconnectCallback(const redisAsyncContext *c, int status)
{
    if (status != REDIS_OK) {
        printf("Disconnection error: %s\n", c->errstr);
        return;
    }
	onExit = 1;
	printf("Disconnected...\n");
}

int main (int argc, char **argv)
{
	FILE* fp;

    signal(SIGPIPE, SIG_IGN);
	signal(SIGINT, gotExitCmd);
    struct event_base *base = event_base_new();
        
	printf("Start to connection\n");
	c = redisAsyncConnect("172.30.0.5", 6379);

	fp = popen("rm -r /etc/mosquitto/passwd", "r");
	pclose(fp);
	fp = popen("touch /etc/mosquitto/passwd","r");
	pclose(fp);
	fp = popen("mosquitto_passwd -b /etc/mosquitto/passwd admin AH0MBwnqi3O-9Dxlt7ZxGHBGsZC5TnEA","r");
	pclose(fp);

	if (ReloadPWD() != 0)
		return -1;

    redisLibeventAttach(c,base);
	redisAsyncSetConnectCallback(c,connectCallback);
	redisAsyncSetDisconnectCallback(c,disconnectCallback);
	redisAsyncCommand(c, subCallback, (char*) "sub", "SUBSCRIBE Cmd");

	while (onExit == 0) {
		event_base_dispatch(base);
		if (onExit == 0) {
			printf("try to reconnect\n");
			c = redisAsyncConnect("172.30.0.5", 6379);
        	redisLibeventAttach(c,base);
			redisAsyncSetConnectCallback(c,connectCallback);
			redisAsyncSetDisconnectCallback(c,disconnectCallback);
			redisAsyncCommand(c, subCallback, (char*) "sub", "SUBSCRIBE Cmd");
		}
		usleep(1000000L);
	}
	
	return 0;
}
