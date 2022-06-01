#ifndef DM_CHECKALIVE_H_
#define DM_CHECKALIVE_H_

#include <string>

void Alive_insert_new_data(char* devName);
int Alive_find_data(std::string devName);
void Alive_update_data_status(std::string devName);
void Alive_remove_data(std::string devName);
int Alive_check_data(std::string devName);
void Alive_clear_data_status(std::string devName);

#endif // DM_CHECKALIVE_H_