#ifndef DH_IMAGEHANDLER_H_
#define DH_IMAGEHANDLER_H_

#include <string>

int SaveScreenshotImg(const std::string& dev_name, const std::string& base64_str,
                        long long *timestamp, long long *id);
std::string ConvertImagetoBase64(const char* buffer, unsigned int size);
int AddImagePathToDB(const std::string& dev_name, const long long& timestamp, const long long& id);

#endif // DH_IMAGEHANDLER_H_