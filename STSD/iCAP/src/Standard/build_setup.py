import os
import errno
import argparse
import sys
from shutil import copyfile


class ProjectSetting(object):
    def __init__(self):
        parser = argparse.ArgumentParser(
            description='Switch Version',
            formatter_class=argparse.RawTextHelpFormatter,
        )
        parser.add_argument(
            '--web-version',
            '-v',
            default='AETINA-RELEASE',
            action='store',
            choices=[
              "STD-RELEASE", 
              "STD-DEMOKIT",
              "AETINA-DEMOKIT", 
              "AETINA-RELEASE"
            ],
            help='setup iCAP Web version (default: AETINA-RELEASE)',
        )
        parser.add_argument(
            '--mode',
            '-m',
            default='RELEASE',
            action='store',
            choices=["RELEASE", "DEMOKIT", "STRESS", "LIGHT"],
            help="switch to specific mode:\r\n" +
            "RELEASE: production (Webservice: no mock data + no log)\r\n" +
            "DEMOKIT: icap demo site (Webservice: mock data + no log)\r\n" +
            "STRESS : more devices (Webservice: mock data + log)\r\n" +
            "LIGHT  : less devices (Webservice: mock data + log)\r\n" +
            "(default: RELEASE)"
        )
        parser.add_argument(
            '--dbchecker',
            '-d',
            default='AETINA',
            action='store',
            choices=["DEMOSITE", "RELEASE", "AETINA"],
            help="setup sql default data"
        )
        parser.add_argument(
            '--device-count',
            '-c',
            default='AETINA_100',
            action='store',
            choices=["HUNDRED", "THREE_HUNDRED", "SIX_HUNDRED",
                     "THOUSAND", "AETINA_100"],
            help="switch to specific device count:\r\n" +
            "HUNDRED: 100 devices\r\n" +
            "THREE_HUNDRED: 300 devices\r\n" +
            "SIX_HUNDRED : 600 devices\r\n" +
            "THOUSAND  : 1000 devices\r\n" +
            "AETINA_100  : 100 devices\r\n"
        )
        parser.add_argument(
            '--device-images',
            '-i',
            default='AETINA',
            action='store',
            choices=["STD", "AETINA"],
            help='setup mock device images version (default: AETINA)',
        )
        silentRemove()
        args = parser.parse_args()

        print('Web Version: {}'.format(args.web_version))
        print('mode: {}'.format(args.mode))
        print('dbchecker: {}'.format(args.dbchecker))
        print('device count: {}'.format(args.device_count))
        print('device images: {}'.format(args.device_images))
        # getattr(self, args.version)()
        getattr(self, args.mode)()
        if args.device_count != '':
            SetupDeviceCount(args.device_count)
        SetupDBChecker(args.dbchecker)
        SetupWebsite(args.web_version)
        SetupDeviceImg(args.device_images)
        exit(1)

    def RELEASE(self):
        SwitchToReleaseMode()

    def DEMOKIT(self):
        SwitchToDemoKitMode()

    def STRESS(self):
        SwitchToStressMode()

    def LIGHT(self):
        SwitchToLightMode()

def SwitchToReleaseMode():
    try:
        print("Switch to RELEASE mode...", end="")
        copyfile("../config/STD.mk", "../config/current.mk")
        copyfile("common/WebService/Website/http/Dockerfile",
                 "common/WebService/Website/Dockerfile")
        copyfile("common/WebService/Website/http/nginx.conf",
                 "common/WebService/Website/nginx.conf")
        copyfile("common/WebService/DashboardAPI/Docker/Release_Dockerfile",
                 "common/WebService/DashboardAPI/Dockerfile")
        copyfile("common/WebService/AuthenticationAPI/Docker/Release_Dockerfile",
                 "common/WebService/AuthenticationAPI/Dockerfile")
        copyfile("common/WebService/DeviceAPI/Docker/Release_Dockerfile",
                 "common/WebService/DeviceAPI/Dockerfile")
        print("success")
    except OSError:
        print("Fail")


def SwitchToDemoKitMode():
    try:
        print("Switch to DEMOKIT mode...", end="")
        copyfile("../config/DemoKit.mk", "../config/current.mk")
        print("success")
    except OSError:
        print("Fail")


def SwitchToStressMode():
    try:
        print("Switch to STRESS mode...", end="")
        copyfile("../config/Stress.mk", "../config/current.mk")
        copyfile("common/WebService/DashboardAPI/Docker/Debug_Dockerfile",
                 "common/WebService/DashboardAPI/Dockerfile")
        copyfile("common/WebService/AuthenticationAPI/Docker/Debug_Dockerfile",
                 "common/WebService/AuthenticationAPI/Dockerfile")
        copyfile("common/WebService/DeviceAPI/Docker/Debug_Dockerfile",
                 "common/WebService/DeviceAPI/Dockerfile")
        print("success")
    except OSError:
        print("Fail")


def SwitchToLightMode():
    try:
        print("Switch to LIGHT mode...", end="")
        copyfile("../config/Light.mk", "../config/current.mk")
        copyfile("common/WebService/DashboardAPI/Docker/Debug_Dockerfile",
                 "common/WebService/DashboardAPI/Dockerfile")
        copyfile("common/WebService/AuthenticationAPI/Docker/Debug_Dockerfile",
                 "common/WebService/AuthenticationAPI/Dockerfile")
        copyfile("common/WebService/DeviceAPI/Docker/Debug_Dockerfile",
                 "common/WebService/DeviceAPI/Dockerfile")
        print("success")
    except OSError:
        print("Fail")


def SetupDeviceCount(count):
    try:
        print("[MockData]Setup {} device-count...".format(count), end="")
        copyfile("../config/Device_Count/{}.json".format(count),
                 "../config/DeviceCount.json")
        copyfile("../config/DeviceCount.json",
                 "common/WebService/MockDataGenerator/DeviceCount.json")
        copyfile("../config/DeviceCount.json",
                 "common/CoreService/DM/DeviceCount.json")
        print("success")
    except OSError:
        print("Fail")


def SetupWebsite(version):
    try:
        print("[Website]Setup {} Website...".format(version), end="")
        copyfile("common/WebService/Website/config/json/{}.config.json".format(version),
                 "common/WebService/Website/config/current_config.json")
        print("success")
    except OSError:
        print("Fail")


def SetupDeviceImg(version):
    try:
        print("[DeviceImg]Setup {} Device Images...".format(version), end="")
        copyfile("../config/Device_Image/{}.mk".format(version),
                 "../config/device_image.mk")
        print("success")
    except OSError:
        print("Fail")


def SetupDBChecker(version):
    try:
        print("[DBChecker] Setup {}.json...".format(version), end="")
        copyfile("common/WebService/DBChecker/DefaultData/{}.json".format(version),
                 "common/WebService/DBChecker/DefaultData.json")
        print("success")
    except OSError:
        print("Fail")


def silentRemove():
    try:
        os.remove("../config/current.mk")
        os.remove("common/Client/ClientService/ServiceSettin g.json")
        os.remove("../config/DeviceCount.json")
        os.remove("common/WebService/Website/innodisk.com.crt")
        os.remove("common/WebService/Website/innodisk.com.key")
        os.remove("common/WebService/Website/Dockerfile")
        os.remove("common/WebService/Website/nginx.conf")
    except OSError as e:
        if e.errno != errno.ENOENT:
            raise

# Main function
if __name__ == "__main__":
    ProjectSetting()
