#ifndef DA_DATAPARSSER_H_
#define DA_DATAPARSSER_H_

typedef std::pair<boost::any, int> ValueTimePair;
class DataParser
{
public:
    ValueTimePair ParseValueFromDB(const std::string& doc_str, std::list<std::string> path);
private:
};

#endif // DA_DATAPARSSER_H_