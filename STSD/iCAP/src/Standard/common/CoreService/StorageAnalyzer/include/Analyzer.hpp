#ifndef SA_ANALYZER_H_
#define SA_ANALYZER_H_

#include <string>

typedef struct
{
    double InitialCapacity;
    double InitialHealth;
    double CurrentCapacity;
    int days;
    int PECycle;
    int LastTime;
} analyzer_data;

int Analyzer_GetStaticRawData(const std::string& raw_string);
int Analyzer_GetDynamicRawData(const std::string& raw_string);
void Analyzer_insert_data(const std::string& storSN, const analyzer_data& temp);
void Analyzer_update_data(const std::string& storSN, const analyzer_data& temp);
int Analyzer_remove_data(const std::string& storSN);
int Analyzer_find_data(const std::string& storSN);

#endif // SA_ANALYZER_H_