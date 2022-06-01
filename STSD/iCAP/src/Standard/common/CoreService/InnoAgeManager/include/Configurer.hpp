#ifndef IAM_CONFIGURER_H_
#define IAM_CONFIGURER_H_
#include <string>

class Configurer {
public:
    Configurer(std::string path = "/var/iCAP/setting.json");
    ~Configurer();
    std::string GetInnoAgeMqttBrokerUrl();
    std::string GetInnoAgeWebserviceUrl();

private:
    std::string LoadSetting();
    std::string _path;
};

#endif  // IAM_CONFIGURER_H_