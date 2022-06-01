#ifndef NS_DATAPARSSER_H_
#define NS_DATAPARSSER_H_

#include <list>
#include <boost/any.hpp>

typedef std::pair<boost::any, int> ValueTimePair;

class DataParser
{
public:
    
    ValueTimePair ParseValueFromDB(const std::string& doc_str, std::list<std::string> path);
};

#endif // NS_DATAPARSSER_H_