#include <algorithm>
#include <chrono>
#include <cstring>
#include <iostream>
#include <list>
#include <sstream>
#include <thread>
#include <boost/log/attributes/named_scope.hpp>
#include "Crypto.hpp"
#include "Logger.hpp"
#include "MQTTAgent.hpp"

using namespace std;

#define UNUSED(x) (void)(x)
#define TIMEOUT	10000L

volatile MQTTAsync_token deliveredtoken;

static int disc_finished = 0;
static int finished = 0;
static int subscribed = 0;
static MQTTAsync client;
vector<mqttsub_rule> sub_rule;
const char *mqtt_client_id;
static devicecmd_handler DEVCMD_HANDLER;

Crypto crypto;

std::string hexStr(unsigned char *data, int len)
{
	stringstream ss;
	ss << hex;
	for (int i = 0; i < len; ++i)
		ss << std::setw(2) << std::setfill('0') << (int)data[i];
	return ss.str();
}

int onMessageArrive(void *context, char *topicName, int topicLen, MQTTAsync_message *message)
{
	UNUSED(context);
	UNUSED(topicLen);

	// string hexString = hexStr(static_cast<unsigned char*>(message->payload), message->payloadlen);
	// LOG_INFO << "hex string:" << hexString;

	const char* payloadptr = (const char*)message->payload;
	string tempstr = "";
	for (int i = 0; i < message->payloadlen; i++) {
		tempstr += *payloadptr++;
	}
	
#ifndef NO_ENCYPTION
	secure_string ctext(tempstr.data(), message->payloadlen);
 	secure_string rtext;
	if (crypto.aes_decrypt(ctext, rtext) != 0)
	{
		LOG_WARN << "MQTT payload decryption failed. Drop the payload!";
		return -1; 
	}

	LOG_DEBUG << "---Recv raw---";
	LOG_DEBUG << "Topic:" << topicName;
	LOG_DEBUG << rtext;
	LOG_DEBUG << "--------------";
	LOG_DEBUG << "Length of payload before decryption:" << message->payloadlen;
	LOG_DEBUG << "--------------";
	LOG_DEBUG << "Length of payload after decryption:" << rtext.length();
	LOG_DEBUG << "---- Done ----";

	DEVCMD_HANDLER(topicName, rtext.data());
#else
	LOG_DEBUG << "---Recv raw---";
	LOG_DEBUG << "Topic:" << topicName;
	LOG_DEBUG << tempstr;
	LOG_DEBUG << "--------------";
	LOG_DEBUG << "Length of payload:" << tempstr.length();
	LOG_DEBUG << "---- Done ----";
	
	DEVCMD_HANDLER(topicName, tempstr);
#endif

    MQTTAsync_free(topicName);
	MQTTAsync_freeMessage(&message);
	
	return 1;
}

void onSubscribe(void* context, MQTTAsync_successData* response)
{
	UNUSED(context);
	UNUSED(response);

	LOG_DEBUG << "Subscribe succeeded";

	subscribed = 1;
}

void onSubscribeFailure(void* context, MQTTAsync_failureData* response)
{
	UNUSED(context);
	int rc = response ? response->code : 0;
	LOG_DEBUG << "Connect failed, rc " << rc;
	finished = 1;
}

void onConnect(void* context, MQTTAsync_successData* response)
{
	UNUSED(response);
	MQTTAsync client = (MQTTAsync)context;
	MQTTAsync_responseOptions opts = MQTTAsync_responseOptions_initializer;
	int rc;
	size_t rule_size = sub_rule.size();
	int qoss[rule_size];
	char* topics[rule_size];

	LOG_DEBUG << "Subscribing for client " << mqtt_client_id;

	for (size_t i = 0; i < rule_size; i++)
	{
		topics[i] = (char*)(sub_rule[i].first.c_str());
		qoss[i] = sub_rule[i].second;
		LOG_DEBUG << "Topic:" << topics[i] << ", Qos:" << qoss[i];
	}

	opts.onSuccess = onSubscribe;
	opts.onFailure = onSubscribeFailure;

	deliveredtoken = 0;
	if ((rc = MQTTAsync_subscribeMany(client, rule_size, topics, qoss, &opts)))
	{
		LOG_ERROR << "Failed to start subscribe, return code " << rc;
	}
}

void onConnectFailure(void* context, MQTTAsync_failureData* response)
{
	UNUSED(context);

	int rc = response ? response->code : 0;
	LOG_ERROR << "Connect failed, rc " << rc << endl;
	finished = 1;
}

void onConnectLost(void* context, char* cause)
{
	MQTTAsync client = (MQTTAsync)context;
	MQTTAsync_connectOptions conn_opts = MQTTAsync_connectOptions_initializer;
	int rc;

	LOG_WARN << "Connection lost" << "cause: " << cause << ". Reconnection";

	conn_opts.keepAliveInterval = 20;
	conn_opts.cleansession = 1;
	if ((rc = MQTTAsync_connect(client, &conn_opts)) != MQTTASYNC_SUCCESS)
	{
		LOG_ERROR << "Failed to start connect, return code " << rc;
		finished = 1;
	}
}

void onDisconnect(void* context, MQTTAsync_successData* response)
{
	UNUSED(context);
	UNUSED(response);

	LOG_DEBUG << "Successful disconnection";
	disc_finished = 1;
}

void onSend(void* context, MQTTAsync_successData* response)
{
	UNUSED(context);
	UNUSED(response);
}

void MQTTAgent_AddSubTopics(Udefined_connect_option &opts, const vector<mqttsub_rule> &sub_rule)
{
	for (auto &it : sub_rule)
		opts.sub_rule.push_back(it);
}

int MQTTAgent_PublishData(const char* topic, const char* payload, int qos)
{
	MQTTAsync_responseOptions opts = MQTTAsync_responseOptions_initializer;
	MQTTAsync_message pubmsg = MQTTAsync_message_initializer;
	int rc;
	int Qos = qos;
	opts.onSuccess = onSend;
	opts.context = client;
	
#ifndef NO_ENCYPTION
	secure_string ctext;
	
	if (crypto.aes_encrypt(payload, ctext) != 0)
	{
		LOG_ERROR << "MQTT payload encryption failed.";
		return -1;
	}

	unsigned char send[ctext.length()];
   	std::copy(ctext.begin(), ctext.end(), send);
	
	//printBytesAsHex(send, sizeof(send), "MQTTAgent_PublishData");

	pubmsg.payload = static_cast<void*>(send);
	pubmsg.payloadlen = sizeof(send);

	LOG_DEBUG << "---Send raw---";
	LOG_DEBUG << "Topic:" << topic;
	LOG_DEBUG << payload;
	LOG_DEBUG << "--------------";
	LOG_DEBUG << "Length of payload before encryption:" << strlen(payload);
	LOG_DEBUG << "--------------";
	LOG_DEBUG << "Length of payload after encryption:" << pubmsg.payloadlen;
	LOG_DEBUG << "---- Done ----";
#else
	string send = payload;
	pubmsg.payload = static_cast<void*>(&send[0]);
	pubmsg.payloadlen = send.length();

	LOG_DEBUG << "---Send raw---";
	LOG_DEBUG << "Topic:" << topic; 
	LOG_DEBUG << send;
	LOG_DEBUG << "--------------";
	LOG_DEBUG << "Length of payload:" << send.length();
	LOG_DEBUG << "---- Done ----";
#endif

	if (Qos > 2 || Qos< 0) {
		LOG_ERROR << "Qos have to be 0~2";
		return -2;
	}
    
	if (topic == NULL) {
        LOG_ERROR << "You have to subscribe a topic!";
        return -3;
    }

	pubmsg.qos = Qos;
	pubmsg.retained = 0;
	deliveredtoken = 0;

	if ((rc = MQTTAsync_sendMessage(client, topic, &pubmsg, &opts)) != MQTTASYNC_SUCCESS)
		LOG_WARN << "Failed to start sendMessage, return code:" << rc;

    return rc;
}

int MQTTAgent_Start(Udefined_connect_option opts, const string& server_uri, devicecmd_handler temp)
{
	const char* username=opts.username;
	const char* password=opts.password;

	MQTTAsync_connectOptions conn_opts = MQTTAsync_connectOptions_initializer;
	int rc;
	
	MQTTAsync_create(&client, server_uri.c_str(), mqtt_client_id, MQTTCLIENT_PERSISTENCE_NONE, NULL);
	sub_rule = opts.sub_rule;
	
	MQTTAsync_setCallbacks(client, NULL, onConnectLost, onMessageArrive, NULL);        
	conn_opts.keepAliveInterval = 20;
	conn_opts.cleansession = 1;
	conn_opts.onSuccess = onConnect;
	conn_opts.onFailure = onConnectFailure;
	conn_opts.context = client;

	DEVCMD_HANDLER = temp;
	if (username != NULL) {
		conn_opts.username = username;
		conn_opts.password = password;
	}

	if ((rc = MQTTAsync_connect(client, &conn_opts)) != MQTTASYNC_SUCCESS)
		return -1;

	while (!subscribed && !finished)
		this_thread::sleep_for(chrono::microseconds(10));

	if (finished)
		return -2;

	return 0;
}

int MQTTAgent_Stop()
{
	MQTTAsync_disconnectOptions disc_opts = MQTTAsync_disconnectOptions_initializer;
	disc_opts.onSuccess = onDisconnect;

	int rc;
	if ((rc = MQTTAsync_disconnect(client, &disc_opts)) != MQTTASYNC_SUCCESS)
	{
		LOG_DEBUG << "Failed to start disconnect, return code " << rc;
	}

	while (!disc_finished)
	{
		this_thread::sleep_for(chrono::microseconds(10));
	}

	MQTTAgent_Destroy();
	return rc;
}

int MQTTAgent_Destroy()
{
	MQTTAsync_destroy(&client);
	sub_rule.clear();
	return 0;
}