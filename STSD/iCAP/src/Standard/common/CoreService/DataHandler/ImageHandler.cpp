#include <cstdlib>
#include <ctime>
#include <fstream>
#include <iostream>
#include <map>
#include <jsoncpp/json/json.h>
#include "base64.hpp"
#include "ImageHandler.hpp"
#include "Logger.hpp"
#include "Nosqlcommand.hpp"

using namespace std;

#define IMG_PATH    "/var/iCAP/Images/screenshot/"

int SaveScreenshotImg(const string& dev_name, const string& base64_str,
                        long long *timestamp, long long *id)
{
    string command = (string)"mkdir -p " + IMG_PATH + dev_name;

    const int dir_err = system(command.c_str());
    if (dir_err == -1) {
        LOG_ERROR << "Creating screenshot directory failed.";
        exit(-1);
    }

    *timestamp = static_cast<long long>(time(nullptr));
    string::size_type sz = 0; // alias of size_t
	*id = stoll(dev_name.substr(7,5), &sz, 10) + *timestamp;
    string file_path = IMG_PATH + dev_name + "/" + dev_name + "_" + to_string(*timestamp) + ".jpg";
    
    vector<BYTE> decodedData = base64_decode(base64_str);
    ofstream outfile(file_path, ios::out | ios::binary); 
    outfile.write((const char*)decodedData.data(), decodedData.size());
    outfile.close();

    return 0;
}

string ConvertImagetoBase64(const char* buffer, unsigned int size)
{
    return base64_encode(reinterpret_cast<const unsigned char*>(buffer), size);
}

int AddImagePathToDB(const string& dev_name, const long long& timestamp, const long long& id)
{
    Json::Value root, img_obj;
    string path = IMG_PATH + dev_name + "/" + dev_name + "_" + to_string(timestamp) + ".jpg";
    
    InsertScreenshotDataToDB(dev_name, id, path);

    return 0;
}