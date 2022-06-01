#include <list>
#include <boost/any.hpp>
#include <jsoncpp/json/json.h>
#include "DataParser.hpp"
#include "Logger.hpp"

using namespace std;

/*
 * doc_str:     data source from mongoDB
 * path:        qurying path
 * arr_index:   -1: use the elements of the last array
 *              >=0: use the index value in an array
 */
ValueTimePair DataParser::ParseValueFromDB(const string& doc_str, list<string> path)
{
    ValueTimePair ret;
    string key = path.front();
    Json::Value root;
    Json::FastWriter fastWriter;
    Json::Reader reader;
    if (!reader.parse(doc_str, root)) {
        LOG_ERROR << "Failed to parse the querying path." << reader.getFormattedErrorMessages();
        return ret;
    }

    int time;
    if (root["time"].asInt() > 0)
        time = root["time"].asInt();
    else
        time = 0;
    
    if (key.compare("Storage") == 0) {
        path.pop_front();
        key = path.front();
    }
    
    boost::any value;
    while (!path.empty()) {
        switch (root[key].type()) {
            case Json::nullValue:
                value = NULL;
                path.clear();
                break;
            case Json::stringValue:
                root = root[key];
                value = fastWriter.write(root);
                path.pop_front();
                break;
            case Json::realValue:
                if (key.compare("Longitude") == 0) {
                    value = make_pair(root["Longitude"].asDouble(), root["Latitude"].asDouble());
                    root = root[key];
                } else {
                    root = root[key];
                    value = root.asDouble();
                }
                path.pop_front();
                break;
            case Json::intValue:
                root = root[key];
                value = root.asInt();
                path.pop_front();
                break;
            case Json::arrayValue:
                root = root[key][root[key].size() - 1];
                if (root["time"].asInt() > 0)
                    time = root["time"].asInt();
                else
                    time = 0;
                path.pop_front();
                key = path.front();
                break;
            case Json::objectValue: {
                root = root[key];
                path.pop_front();
                key = path.front();
            }
                break;
            default:
                LOG_TRACE << "DEFAULT";
        }
    }
    
    ret = make_pair(value, time);
    return ret;
}