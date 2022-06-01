#include <chrono>
#include <ctime>
#include <iostream>
#include <utility> //std::move
#include <bsoncxx/builder/stream/document.hpp>
#include <bsoncxx/json.hpp>
#include <bsoncxx/stdx/make_unique.hpp>
#include <mongocxx/client.hpp>
#include <mongocxx/exception/bulk_write_exception.hpp>
#include <mongocxx/exception/operation_exception.hpp>
#include <mongocxx/logger.hpp>
#include <mongocxx/uri.hpp>
#include "Logger.hpp"
#include "Mongodb.hpp"
#include "Nosqlcommand.hpp"

using namespace std;
using bsoncxx::builder::basic::kvp;
using bsoncxx::builder::basic::make_document;
using bsoncxx::builder::concatenate;
using bsoncxx::builder::stream::close_array;
using bsoncxx::builder::stream::close_document;
using bsoncxx::builder::stream::document;
using bsoncxx::builder::stream::finalize;
using bsoncxx::builder::stream::open_document;
using bsoncxx::builder::stream::open_array;

const char *dbName = "iCAP";
const char *mongodb_addr = "mongodb://icap-admin:icap-admin-pwd@172.30.0.2:27017/?authSource=iCAP";

bool configureDB(mongocxx::uri uri)
{
class noop_logger : public mongocxx::logger {
public:
	virtual void operator()(mongocxx::log_level,
	bsoncxx::stdx::string_view,
	bsoncxx::stdx::string_view) noexcept {}
};

	auto instance =
			bsoncxx::stdx::make_unique<mongocxx::instance>(bsoncxx::stdx::make_unique<noop_logger>());

	bool result = MongoDatabase.Initialize(move(instance), bsoncxx::stdx::make_unique<mongocxx::pool>(move(uri)));
	
	return result;
}

bool InitializeMongoDB(void)
{
	mongocxx::uri uri(mongodb_addr);
	return configureDB(move(uri)); 
}

bool CheckCollExists(const std::string &collection_name)
{
	if (collection_name.empty()) {
		LOG_ERROR << "Parameter collection_name is empty.";
		return false;
	}

	auto connection = MongoDatabase.TryGetConnection();
	if (!connection) {
		LOG_ERROR << "Get dataDB connection failed!";
		return false;
	}

	auto &conn = **connection;
	auto mdb = conn[dbName];
	return mdb.has_collection(collection_name);
}

int GetDocCountInColl(const std::string &collection_name)
{
	if (collection_name.empty()) {
		LOG_ERROR << "Parameter collection_name is empty.";
		return -1;
	}
	
	auto connection = MongoDatabase.TryGetConnection();
	if (!connection) {
		LOG_ERROR << "Get dataDB connection failed!";
		return -1;
	}

	auto &conn = **connection;
	auto mdb = conn[dbName];
	auto collection = mdb[collection_name];
	return collection.count_documents({});
}

bool DropColl(const std::string &collection_name)
{
	if (collection_name.empty()) {
		LOG_ERROR << "Parameter collection_name is empty.";
		return false;
	}

	auto connection = MongoDatabase.TryGetConnection();
	if (!connection) {
		LOG_ERROR << "Get dataDB connection failed!";
		return false;
	}

	auto &conn = **connection;
	auto mdb = conn[dbName];
	mdb[collection_name].drop();
	return true;
}

int InsertDataToCappedColl(const std::string& collection_name, const std::string& json_string)
{ 
	if (collection_name.empty()) {
		LOG_ERROR << "Parameter collection_name is empty.";
		return -1;
	}
	if (json_string.empty()) {
		LOG_ERROR << "Parameter json_string is empty.";
		return -1;
	}
	
	auto connection = MongoDatabase.TryGetConnection();
	if (!connection) {
		LOG_ERROR << "Get dataDB connection failed!";
		return -1;
	}
	auto &conn = **connection;
	auto mdb = conn[dbName];
	mongocxx::collection collection;
	auto doc = bsoncxx::from_json(json_string);
	auto view = doc.view();
	auto create_opts = mongocxx::options::create_collection{}.capped(true).size(2048 * 2048).max(1);

	try {
		mdb.create_collection(collection_name, create_opts);
		auto result = mdb[collection_name].insert_one(view);
	} catch (const mongocxx::operation_exception& e) {
		if (e.code().value() == 48) // The collection already exists.
		{
			// exception from 'create_collection', so we insert the document here
			auto result = mdb[collection_name].insert_one(view);
		} else {
			LOG_ERROR << e.what();
			return -1;
		}
	}

	return 0;
}

int InsertDataToDB(const std::string& collection_name, const std::string& json_string)
{
	if (collection_name.empty()) {
		LOG_ERROR << "Parameter collection_name is empty.";
		return -1;
	}
	if (json_string.empty()) {
		LOG_ERROR << "Parameter json_string is empty.";
		return -1;
	}

	auto connection = MongoDatabase.TryGetConnection();
	if (!connection) {
		LOG_ERROR << "Get dataDB connection failed!";
		return -1;
	}
	auto &conn = **connection;
	auto mdb = conn[dbName];
	document doc{};
	auto data = bsoncxx::from_json(json_string);
	doc << concatenate(data.view()) << "date" << bsoncxx::types::b_date(chrono::system_clock::now());

	try {
		auto result = mdb[collection_name].insert_one(doc.view()); //read only
	} catch (const mongocxx::operation_exception& e) {
		LOG_ERROR << e.what();
		return -1;
	}
	
	return 0;
}

int QueryLatestDataFromDB(const std::string& collection_name, std::string& data)
{
	if (collection_name.empty()) {
		LOG_ERROR << "Parameter collection_name is empty.";
		return -1;
	}

	auto connection = MongoDatabase.TryGetConnection();
	if (!connection) {
		LOG_ERROR << "Get dataDB connection failed!";
		return -1;
	}

	auto &conn = **connection;
	auto mdb = conn[dbName];
	auto collection = mdb[collection_name];
	int64_t count = collection.count_documents({});

	if (count == 0) return 0;

	mongocxx::options::find opts{};
	opts.limit(1);
	opts.sort(document{} << "_id" << -1 << finalize);
	auto cursor = collection.find({}, opts);
	for (auto doc : cursor) 
		data = bsoncxx::to_json(doc);
	
	return 1;
}

int QueryDataFromDB(const std::string& collection_name, int direction, int limit, std::vector<std::string>& data)
{
	if (collection_name.empty()) {
		LOG_ERROR << "Parameter collection_name is empty.";
		return -1;
	}
	if (direction != 1 && direction != -1) {
		LOG_ERROR << "Parameter direction is fault.";
		return -1;
	}

	auto connection = MongoDatabase.TryGetConnection();
	if (!connection) {
		LOG_ERROR << "Get dataDB connection failed!";
		return -1;
	}

	auto &conn = **connection;
	auto mdb = conn[dbName];
	auto collection = mdb[collection_name];
	mongocxx::options::find opts;
	int64_t count = collection.count_documents({});

	if (count == 0) return 0;

	if (limit < 0) {
		LOG_ERROR << "limit option fault!";
		return -1;
	}

	if (limit > count)
		opts.limit(count);
	else if (limit != 0)
		opts.limit(limit);
	
	opts.sort(document{} << "_id" << direction << finalize);

	auto cursor = collection.find({}, opts);
	for (auto doc : cursor) 
		data.push_back(bsoncxx::to_json(doc));
	if (data.size() == 0) {
		LOG_DEBUG << "There is no any document meet your options!";
		return 0;
	}

	return data.size();
}

int QueryAnalyzerDataFromDB(const std::string& SN, std::string& data)
{
	if (SN.empty()) {
		LOG_ERROR << "Query analyzer data from DB, parameter SN is empty.";
		return -1;
	}

	auto connection = MongoDatabase.TryGetConnection();
	if (!connection) {
		LOG_ERROR << "Get dataDB connection failed!";
		return -1;
	}

	auto &conn = **connection;
	auto mdb = conn[dbName];
	auto collection = mdb["StorageAnalyzer"];
	int64_t count = collection.count_documents({});

	if (count == 0) return -1;

	auto doc = document{} << "SN" << SN << finalize;
	auto maybe_result = collection.find_one(doc.view());
	if (maybe_result)  {
		data = bsoncxx::to_json(*maybe_result);
		LOG_TRACE << "maybe_result:" << data;
	} else {
		LOG_DEBUG << "There is no any document meet your options!";
		return -1;
	}
			
	return 0;
}

int QueryScreenshotDataFromDB(const string& dev_name, string& data)
{
	if (dev_name.empty()) {
		LOG_ERROR << "Parameter dev_name is empty.";
		return -1;
	}

	auto connection = MongoDatabase.TryGetConnection();
	if (!connection) {
		LOG_ERROR << "Get dataDB connection failed!";
		return -1;
	}

	auto &conn = **connection;
	auto mdb = conn[dbName];
	auto collection = mdb["Screenshot"];
	int64_t count = collection.count_documents({});

	if (count == 0)	return -1;

	auto doc = document{} << "Dev" << dev_name << finalize;
	auto maybe_result = collection.find_one(doc.view());
	if (maybe_result) {
		data = bsoncxx::to_json(*maybe_result);
		LOG_TRACE << "maybe_result:" << data;
	} else {
		LOG_DEBUG << "There is no any document meet your options!";
		return -1;
	}
			
	return 0;
}

int QueryLatestEventByDevNameFromDB(const string& dev_name, string& event)
{
	auto connection = MongoDatabase.TryGetConnection();
	if (!connection) {
		LOG_ERROR << "Get dataDB connection failed!";
		return -1;
	}

	auto &conn = **connection;
	auto mdb = conn[dbName];
	auto collection = mdb["EventLog"];
	int64_t count = collection.count_documents({});

	if (count == 0)	return 0;

	auto doc = document{} << "Dev" << dev_name << "Checked" << false << finalize;
	
	mongocxx::options::find opts{};
	opts.limit(1);
	opts.sort(document{} << "_id" << -1 << finalize);
	auto maybe_result = collection.find_one(doc.view(), opts);
	if (maybe_result) {
		event = bsoncxx::to_json(*maybe_result);
		LOG_TRACE << event;
		return 1;
	}

	LOG_DEBUG << "There is no any document meet your options!";
	return 0;
}

int QueryLatestEventByMsgFromDB(const string& message, string& event) 
{
	auto connection = MongoDatabase.TryGetConnection();
	if (!connection) {
		LOG_ERROR << "Get dataDB connection failed!";
		return -1;
	}

	auto &conn = **connection;
	auto mdb = conn[dbName];
	auto collection = mdb["EventLog"];
	int64_t count = collection.count_documents({});

	if (count == 0) return 0;
	
	auto doc = document{} << "Message" << message << finalize;

	mongocxx::options::find opts{};
	opts.limit(1);
	opts.sort(document{} << "_id" << -1 << finalize);
	auto maybe_result = collection.find_one(doc.view(), opts);
	if (maybe_result) {
		event = bsoncxx::to_json(*maybe_result);
		LOG_TRACE << event;
		return 1;
	}

	LOG_DEBUG << "There is no any document meet your options!";
	return 0;
}

int QueryEventDataWithinTimeFromDB(const string& dev_name, const string& message)
{
	auto connection = MongoDatabase.TryGetConnection();
	if (!connection) {
		LOG_ERROR << "Get dataDB connection failed!";
		return -1;
	}

	auto &conn = **connection;
	auto mdb = conn[dbName];
	auto collection = mdb["EventLog"];
	int64_t count = collection.count_documents({});

	if (count == 0)	return 0;

	chrono::system_clock::time_point t = chrono::system_clock::now() - chrono::hours(24);
	bsoncxx::types::b_date d{t};
	
	auto doc = document{} << "Dev" << dev_name << "Message" << message << "Checked" << false 
				<< "date" << open_document << "$gt" << d << close_document << finalize;
	mongocxx::options::find opts{};
	opts.limit(1);
	opts.sort(document{} << "_id" << -1 << finalize);
	auto maybe_result = collection.find_one(doc.view(), opts);
	if (maybe_result) {
		LOG_TRACE << "maybe_result:" << bsoncxx::to_json(*maybe_result);
		return 1;
	}

	LOG_DEBUG << "There is no any document meet your options!";
	return 0;
}

bsoncxx::types::b_date read_date(const std::string& date) {
    tm utc_tm{};
    istringstream ss{date};

    // Read time into std::tm.
    ss >> get_time(&utc_tm, "%Y-%m-%d");

    // Convert std::tm to std::time_t.
    time_t utc_time = mktime(&utc_tm);

    // Convert std::time_t std::chrono::systemclock::time_point.
    chrono::system_clock::time_point time_point =
		chrono::system_clock::from_time_t(utc_time);

    return bsoncxx::types::b_date { time_point };
}

int QueryOverThrsholdDataFromDB(const string& collection_name, const string& start_date, const string& end_date,
						  		const string& data_path, double threshold_value)
{
 	auto connection = MongoDatabase.TryGetConnection();
	if (!connection) {
		LOG_ERROR << "Get dataDB connection failed!";
		return -1;
	}

	auto &conn = **connection;
	auto mdb = conn[dbName];
	auto collection = mdb[collection_name];
	int64_t count = collection.count_documents({});

	if (count == 0) return -1;

	auto doc = document{} 
			   << "date" << open_document
			   << "$gte" << read_date(start_date) 
			   << "$lte" << read_date(end_date) << close_document 
			   << data_path << open_document <<
			   "$gte" << threshold_value << close_document
			   << finalize;

	auto maybe_result = collection.find_one(doc.view());
	if (maybe_result) {
		LOG_DEBUG << "Found data for report.";
		LOG_TRACE << bsoncxx::to_json(*maybe_result);
		return 1;
	}
	LOG_DEBUG << "There is no any document meet your options!";

	return 0;
}

int QueryLessThrsholdDataFromDB(const string& collection_name, const string& start_date, const string& end_date,
						  		const string& data_path, double threshold_value)
{
	auto connection = MongoDatabase.TryGetConnection();
	if (!connection) {
		LOG_ERROR << "Get dataDB connection failed!";
		return -1;
	}

	auto &conn = **connection;
	auto mdb = conn[dbName];
	auto collection = mdb[collection_name];
	int64_t count = collection.count_documents({});

	if (count == 0) return -1;

	auto doc = document{} 
			   << "date" << open_document
			   << "$gte" << read_date(start_date) 
			   << "$lte" << read_date(end_date) << close_document 
			   << data_path << open_document <<
			   "$lt" << threshold_value << close_document
			   << finalize;

	auto maybe_result = collection.find_one(doc.view());
	if (maybe_result) {
		LOG_DEBUG << "Found data for report.";
		LOG_TRACE << bsoncxx::to_json(*maybe_result);
		return 1;
	} 
	LOG_DEBUG << "There is no any document meet your options!";
	
	return 0;
}

int QueryFakeDeviceStatusFromDB(const std::string& dev_name)
{
	auto connection = MongoDatabase.TryGetConnection();
	if (!connection) {
		LOG_ERROR << "Get dataDB connection failed!";
		return -1;
	}

	auto &conn = **connection;
	auto mdb = conn[dbName];
	auto collection = mdb["EventLog"];
	int64_t count = collection.count_documents({});

	if (count == 0) return -1;

	auto maybe_result = collection.find_one(
		make_document(kvp("Dev", dev_name), kvp("Message", bsoncxx::types::b_regex{"^.*offline"})));
	
	if (maybe_result) {
		LOG_DEBUG << "Found offline message.";
		LOG_TRACE << bsoncxx::to_json(*maybe_result);
		return 1;
	}

	LOG_DEBUG << "There is no any document meet your options!";
	return 0;
}

int InsertAnalyzerDataToDB(const string& SN, const int& time, double health, int value)
{
	if (SN.empty()) {
		LOG_ERROR << "Insert Analyzer data to DB, parameter SN is empty.";
		return -1;
	}
	if (time < 0 || health < 0 || value < 0) {
		LOG_ERROR << "Parameter time or health or value error.";
		return -1;
	}

	auto connection = MongoDatabase.TryGetConnection();
	if (!connection) {
		LOG_ERROR << "Get dataDB connection failed!";
		return -1;
	}

	auto &conn = **connection;
	auto mdb = conn[dbName];
	auto collection = mdb["StorageAnalyzer"];
	document doc{};
	auto STdata = doc
		<< "$push" << open_document << "Lifespan" << open_document << "time" << time << "health" << health
		<< "data" << value << close_document << close_document << finalize;
	auto filter = doc << "SN" << SN << finalize;
	collection.update_one(filter.view(), STdata.view());

	return 0;
}

int InsertScreenshotDataToDB(const string& dev_name, const int64_t& id, const string& path)
{
	if (dev_name.empty()) {
		LOG_ERROR << "Parameter dev_name is empty.";
		return -1;
	}
	if (id < 0) {
		LOG_ERROR << "Parameter id error.";
		return -1;
	}
	if (path.empty()) {
		LOG_ERROR << "Parameter path is empty.";
		return -1;
	}

	auto connection = MongoDatabase.TryGetConnection();
	if (!connection) {
		LOG_ERROR << "Get dataDB connection failed!";
		return -1;
	}

	auto &conn = **connection;
	auto mdb = conn[dbName];
	auto collection = mdb["Screenshot"];
	
	auto filter = document{} << "Dev" << dev_name << finalize;
	auto maybe_result = collection.find_one(filter.view());
	if (maybe_result)  {
		auto ImageData = document{}
			<< "$push" << open_document << "Images" << open_document << "Id" << id << "Path" << path 
			<< close_document << close_document << finalize;

		collection.update_one(filter.view(), ImageData.view());
	} else {
		document builder{};

		auto before_array = builder << "Dev" << dev_name;
		auto in_array = before_array << "Images" << open_array << open_document << "Id" << id 
									<< "Path" << path << close_document << close_array;
		auto doc = in_array << finalize;
		LOG_DEBUG << bsoncxx::to_json(doc);

		collection.insert_one(doc.view());
	}

	return 0;
}

int DeleteAnalyzerDocFromDB(const std::string& SN)
{
	auto connection = MongoDatabase.TryGetConnection();
	if (!connection) {
		LOG_ERROR << "Get dataDB connection failed!";
		return -1;
	}

	auto &conn = **connection;
	auto mdb = conn[dbName];
	auto collection = mdb["StorageAnalyzer"];

	try {
		collection.delete_one(document{} << "SN" << SN << finalize);
	} catch (const exception& e) {
		LOG_ERROR << e.what();
		return -1;
	}
	
	return 0;
}

int DeleteScreenshotDocFromDB(const std::string& dev_name)
{
	auto connection = MongoDatabase.TryGetConnection();
	if (!connection) {
		LOG_ERROR << "Get dataDB connection failed!";
		return -1;
	}

	auto &conn = **connection;
	auto mdb = conn[dbName];
	auto collection = mdb["Screenshot"];

	try {
		collection.delete_one(document{} << "Dev" << dev_name << finalize);
	} catch (const exception& e) {
		LOG_ERROR << e.what();
		return -1;
	}
	
	return 0;
}

int DeleteScreenshotDataFromDB(const std::string& dev_name, const int64_t& id)
{
	auto connection = MongoDatabase.TryGetConnection();
	if (!connection) {
		LOG_ERROR << "Get dataDB connection failed!";
		return -1;
	}

	auto &conn = **connection;
	auto mdb = conn[dbName];
	auto collection = mdb["Screenshot"];

	try {
		auto ImgElement = document{}
			<< "$pull" << open_document << "Images" << open_document << "Id" << id << close_document 
			<< close_document << finalize;
		auto filter = document{} 
			<< "Dev" << dev_name << finalize;
		collection.update_one(filter.view(), ImgElement.view());
	} catch (const exception& e) {
		LOG_ERROR << e.what();
		return -1;
	}
	
	return 0;
}

int DeleteOldDataInDB(const int& expired_days)
{
	auto connection = MongoDatabase.TryGetConnection();
	if (!connection) {
		LOG_ERROR << "Get dataDB connection failed!";
		return -1;
	}

	auto &conn = **connection;
	auto mdb = conn[dbName];
	
	chrono::system_clock::time_point t = chrono::system_clock::now() - chrono::hours(24 * expired_days);
	bsoncxx::types::b_date d{t};

	int delete_count = 0;
	try {
		auto cursor = mdb.list_collections();
		for (auto doc : cursor) {
			auto ele = doc["name"];
			string collection_name = ele.get_utf8().value.to_string();
			if (collection_name.find("Widget") != string::npos)
				continue; // Ignore capped collection
			auto collection = mdb[collection_name];
			auto result = collection.delete_many(make_document(kvp("date", make_document(kvp("$lt", d)))));
			delete_count += result->result().deleted_count();
		}
	} catch (const exception& e) {
		LOG_ERROR << e.what();
		return -1;
	}

	return delete_count;
}