#!/usr/bin/python3
from __future__ import print_function
try:
    import docker
except ImportError:
    import __docker__ as docker
import os

if os.name=='nt':
    import sys, atexit, traceback, types, msvcrt, ctypes, shutil, tarfile
else:
    import sys, termios, atexit
    import fcntl
import struct
import time
import subprocess
from select import select
from argparse import ArgumentParser

# Check current.mk 'VERSION' key, likes STD or DFI
def CheckProjectVersion(projectTag):
	# This installer will in iCAP_Server_Release floder
    with open('./current.mk') as f:
        if 'VERSION' and projectTag in f.read():
            print('Version: ', projectTag)
            f.close()
            return True
        else:
            f.close()
            return False
IS_DFI_PROJECT = CheckProjectVersion('DFI')

# nt is Windows OS
if os.name == 'nt':
    STD_INPUT_HANDLE = -10
    STD_OUTPUT_HANDLE= -11
    STD_ERROR_HANDLE = -12

    FOREGROUND_White = 0x07

    class Color:
        ''' See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winprog/winprog/windows_api_reference.asp
        for information on Windows APIs.'''
        std_out_handle = ctypes.windll.kernel32.GetStdHandle(STD_OUTPUT_HANDLE)
        
        def set_cmd_color(self, color, handle=std_out_handle):
            """(color) -> bit
            Example: set_cmd_color(FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE | FOREGROUND_INTENSITY)
            """
            bool = ctypes.windll.kernel32.SetConsoleTextAttribute(handle, color)
            return bool
        
        def reset_color(self):
            # self.set_cmd_color(FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE)
            self.set_cmd_color(FOREGROUND_White)

    def Checkret_WIN(return_code):
        if return_code==0:
            print ('Success')
        else:
            print ('Fail')

    def Load_image_WIN(name):
        p = subprocess.Popen('docker load --input ' + name, stdout = subprocess.PIPE, shell=True)
        p_status = p.wait()
        Checkret_WIN(p_status)

    def Remove_container_WIN(name):
        print ('Remove container ' + name + '...', end="")
        p = subprocess.Popen('docker rm -f '+name, stdout=subprocess.PIPE, shell=True, stderr =subprocess.PIPE)
        p_status = p.wait()
        print ('Success')

    def runAsAdmin(cmdLine=None, wait=True):

        if os.name != 'nt':
            raise (RuntimeError, "This function is only implemented on Windows.")

        import win32api, win32con, win32event, win32process
        from win32com.shell.shell import ShellExecuteEx
        from win32com.shell import shellcon

        python_exe = sys.executable

        if cmdLine is None:
            cmdLine = [python_exe] + sys.argv
        elif type(cmdLine) not in (types.TupleType,types.ListType):
            raise (ValueError, "cmdLine is not a sequence.")
        cmd = '"%s"' % (cmdLine[0],)
        # XXX TODO: isn't there a function or something we can call to massage command line params?
        params = " ".join(['"%s"' % (x,) for x in cmdLine[1:]])
        cmdDir = ''
        showCmd = win32con.SW_SHOWNORMAL
        #showCmd = win32con.SW_HIDE
        lpVerb = 'runas'  # causes UAC elevation prompt.

        # print "Running", cmd, params

        # ShellExecute() doesn't seem to allow us to fetch the PID or handle
        # of the process, so we can't get anything useful from it. Therefore
        # the more complex ShellExecuteEx() must be used.

        # procHandle = win32api.ShellExecute(0, lpVerb, cmd, params, cmdDir, showCmd)

        procInfo = ShellExecuteEx(nShow=showCmd,
                                  fMask=shellcon.SEE_MASK_NOCLOSEPROCESS,
                                  lpVerb=lpVerb,
                                  lpFile=cmd,
                                  lpParameters=params)

        if wait:
            procHandle = procInfo['hProcess']    
            obj = win32event.WaitForSingleObject(procHandle, win32event.INFINITE)
            rc = win32process.GetExitCodeProcess(procHandle)
            #print "Process handle %s returned code %s" % (procHandle, rc)
        else:
            rc = None

        return rc

    def Print_eula_WIN():
        with open("eula.md", encoding = 'utf-8-sig') as eula:
            for line in eula:
                print(line)

    def begin_install(MOCK_DATA_FLAG, OPEN_PORT_FLAG):
        clr = Color()
        print ('[Success]Permission permitted')
        print ('iCAP Service has an EULA that needs you consent.')
        print ('If you use it, you grant consent as well.')
        print ('Press any key to print EULA...')
        while (True):
            if (ord(msvcrt.getch())):
                break
        Print_eula_WIN()
        while (True):
            value = input("Do you accept the EULA? (yes/no): ")
            if (value.lower() == "yes"):
                break
            elif (value.lower() == "no"):
                sys.exit(0)
            else:
                print('Please enter "yes" or "no"')

        print("...")
        # print ('Loading iCAP Cluster Manager image...', end = "")
        # Load_image_WIN('icap_cluster_manager.tar')

        print ('Loading iCAP Data DB image...', end = "")
        Load_image_WIN('icap_datadb.tar')

        print ('Loading iCAP Admin DB image...', end = "")
        Load_image_WIN('icap_admindb.tar')

        print ('Loading iCAP Redis Cache image...', end = "")
        Load_image_WIN('icap_redis.tar')

        print ('Loading iCAP Gateway image...', end = "")
        Load_image_WIN('icap_gateway.tar')

        print ('Loading iCAP Web-service : Authentication API image...', end = "")
        Load_image_WIN('icap_webservice_authapi.tar')

        print ('Loading iCAP Web-service : Device API image...', end = "")
        Load_image_WIN('icap_webservice_deviceapi.tar')

        print ('Loading iCAP Web-service : Dashboard API image...', end = "")
        Load_image_WIN('icap_webservice_dashboardapi.tar')

        print ('Loading iCAP Web-service : OOB Service image...', end = "")
        Load_image_WIN('icap_webservice_oobservice.tar')

        print ('Loading iCAP Web-service : Website image...', end = "")
        Load_image_WIN('icap_webservice_website.tar')

        print ('Loading iCAP DB Checker image...', end = "")
        Load_image_WIN('icap_dbchecker.tar')
        
        if (MOCK_DATA_FLAG == True):
            print ('Loading iCAP Mock Data Generator image...', end = "")
            Load_image_WIN('icap_mockdata.tar')

        print ('Loading innoAge Gateway image ...', end = "")
        Load_image_WIN('innoAGE-Gateway.tar')

        print ('Loading innoAge Web-service image ...', end = "")
        Load_image_WIN('innoAGE-WebService.tar')

        print ('Loading iCAP Core service : Device Management image...', end = "")
        Load_image_WIN('icap_coreservice_dm.tar')

        print ('Loading iCAP Core service : Data handler image...', end ="")
        Load_image_WIN('icap_coreservice_datahandler.tar')

        print ('Loading iCAP Core service : Storage analyzer image...', end = "")
        Load_image_WIN('icap_coreservice_storanalyzer.tar')

        print ('Loading iCAP Core service : Dashboard Angent image...', end = "")
        Load_image_WIN('icap_coreservice_dashboardagent.tar')

        print ('Loading iCAP Core service : Notification service image...', end ="")
        Load_image_WIN('icap_coreservice_notify.tar')

        print ('Loading iCAP Core service : Data Lifecycle Manager image...', end ="")
        Load_image_WIN('icap_coreservice_dlm.tar')

        print ('Loading iCAP Core service : innoAge Manager image...', end ="")
        Load_image_WIN('icap_coreservice_innoagemanager.tar')

        print ('Starting containers...')
        reInstall = False
        p = subprocess.Popen("docker network inspect icap_net", stdout = subprocess.PIPE, shell=True , stderr = subprocess.PIPE)
        p_status = p.wait()
        if p_status == 0:
            while (True):
                value = input("Seems the iCAP server was installed,do you want to re-install?(y/n)")
                if (value.lower() == "y"):
                    break
                elif (value.lower() == "n"):
                    sys.exit(0)
                else:
                    print('Please enter "y" or "n"')
            
            print('Try to remove iCAP server continers...')

            Remove_container_WIN('innoage_gateway')
            Remove_container_WIN('innoage_webservice')
            Remove_container_WIN('core_dm')
            Remove_container_WIN('core_datahandler')
            Remove_container_WIN('core_storanalyzer')
            Remove_container_WIN('core_notifyservice')
            Remove_container_WIN('core_dashboard')
            Remove_container_WIN('core_dashboardagent')
            Remove_container_WIN('core_dlm')
            Remove_container_WIN('core_innoagemanager')
            Remove_container_WIN('webservice_oobservice')
            Remove_container_WIN('dashboardapi')
            Remove_container_WIN('deviceapi')
            Remove_container_WIN('authapi')
            # Remove_container_WIN('icap_cluster_manager')
            Remove_container_WIN('gateway')
            Remove_container_WIN('redis')
            Remove_container('innoAGE-WebService')
            Remove_container('innoAGE-Gateway')
            Remove_container_WIN('adminDB')
            Remove_container_WIN('dataDB')
            Remove_container_WIN('website')
            Shellcommand('docker network rm icap_net')
            print('Remove network icap_net...Success')
            reInstall = True
        else:
            Shellcommand('md "c:\\Program Files\\iCAP_Server"')
            Shellcommand('icacls "c:/Program Files/iCAP_Server" /grant Users:F /T')
            tar = tarfile.open('Images.tar.gz')
            names = tar.getnames()
            for name in names:
              tar.extract(name, path = "C:\\Program Files\\iCAP_Server\\")
            tar.close()
            
            print ('Add a script at startup...', end= "")
            sys.stdout.flush()
            Shellcommand('copy setting.json "c:\\Program Files\\iCAP_Server"')
            Shellcommand('icacls "c:\\Program Files\\iCAP_Server\\setting.json" /grant Users:F /T')
            Shellcommand('copy setting.env "c:\\Program Files\\iCAP_Server"')
            Shellcommand('icacls "c:\\Program Files\\iCAP_Server\\setting.env" /grant Users:F /T')
            Shellcommand('copy Log_Update.cmd "c:\\Program Files\\iCAP_Server"')
            # Shellcommand('tar -C "c:\\Program Files\\iCAP_Server\\Images" -zxvf Images.tar.gz')

        result = Shellcommand('netsh advfirewall firewall show rule name=ICAP_WEB_IN >nul')
        if result != 1:
            print('You already got a in rule by ICAP_WEB_IN, you cannot put another one in!')
        else:
            print('Rule ICAP_WEB_IN does not exist. Creating...')
            Shellcommand('netsh advfirewall firewall add rule name=ICAP_WEB_IN dir=in action=allow protocol=TCP localport=80')

        result = Shellcommand('netsh advfirewall firewall show rule name=ICAP_WEB_OUT >nul')
        if result != 1:
            print('You already got a in rule by ICAP_WEB_OUT, you cannot put another one in!')
        else:
            print('Rule ICAP_WEB_OUT does not exist. Creating...')
            Shellcommand('netsh advfirewall firewall add rule name=ICAP_WEB_OUT dir=out action=allow protocol=TCP localport=80')

        result = Shellcommand('netsh advfirewall firewall show rule name=MQTT_IN >nul')
        if result != 1:
            print('You already got a in rule by MQTT_IN, you cannot put another one in!')
        else:
            print('Rule MQTT_IN does not exist. Creating...')
            Shellcommand('netsh advfirewall firewall add rule name=MQTT_IN dir=in action=allow protocol=TCP localport=1883')

        result = Shellcommand('netsh advfirewall firewall show rule name=MQTT_OUT >nul')
        if result != 1:
            print('You already got a in rule by MQTT_OUT, you cannot put another one in!')
        else:
            print('Rule MQTT_OUT does not exist. Creating...')
            Shellcommand('netsh advfirewall firewall add rule name=MQTT_OUT dir=out action=allow protocol=TCP localport=1883')

        Shellcommand('docker network create --driver bridge --subnet 172.30.0.0/16 icap_net')
        # print ('Mount container clustermanager from image icap_cluster_manager...', end="") 
        # result = Shellcommand('docker run -idt --network=icap_net --ip 172.30.0.15 --restart always --privileged --name icap_cluster_manager -v //var/run/docker.sock:/var/run/docker.sock -w=/root icap_cluster_manager:v1.6.1')
        # Checkret_WIN(result)

        print ('Mount container dataDB from image icap_datadb...', end="")
        result = Shellcommand('docker run -idt --network=icap_net --ip 172.30.0.2 --restart always --name dataDB -p 27017:27017 -e MONGO_INITDB_ROOT_USERNAME=root -e MONGO_INITDB_ROOT_PASSWORD=root -e MONGO_INITDB_DATABASE=iCAP icap_datadb:v1.6.1')
        Checkret_WIN(result)
        
        print ('Mount container adminDB from image icap_admindb...', end = "")
        result = Shellcommand('docker-compose up -d')
        clr.reset_color()
        Checkret_WIN(result)
        
        print ('Mount container redis from image icap_redis...', end="")
        if ps.Checkarg()=='STRESS' or ps.Checkarg()=='DEBUG':
            result = Shellcommand('docker run -idt --network=icap_net --ip 172.30.0.5 --restart always --name redis -p 6379:6379 icap_redis:v1.6.1')
            Checkret_WIN(result)
        else:
            result = Shellcommand('docker run -idt --network=icap_net --ip 172.30.0.5 --restart always --name redis icap_redis:v1.6.1')
            Checkret_WIN(result)

        if Checkstatus('adminDB') == 0:
            result = Shellcommand('docker run -idt --network=icap_net --ip 172.30.0.50 --name dbchecker icap_dbchecker:v1.6.1')
            while Checkstatus('dbchecker') != -1:
                time.sleep(2)
            result=Shellcommand('docker rm dbchecker')
            result=Shellcommand('docker rmi icap_dbchecker:v1.6.1')
        else:
            print(bcolors.WARNING+"[Fail]iCAP Admin DB can't be installed properly")
            print('Please try again'+bcolors.ENDC)

        if reInstall==0:
            if ps.Checkarg()=='STRESS' or ps.Checkarg()=='DEMOKIT':
                result = Shellcommand('docker run -idt --network=icap_net --ip 172.30.0.51 --name mockdata icap_mockdata:v1.6.1')
                time.sleep(2)
                while Checkstatus('mockdata') !=- 1:
                    time.sleep(2)
                result = Shellcommand('docker rm mockdata')
                result = Shellcommand('docker rmi icap_mockdata:v1.6.1')

        print ('Mount container innoage_gateway from image eclipse-mosquitto...', end="")
        result = Shellcommand('docker run -idt -p 8883:8883 -p 18883:18883 --network=icap_net --ip 172.30.0.101 --restart always --name innoAGE-Gateway -e MQTT_USER=innoage -e MQTT_PASSWORD=B673AEBC6D65E7F42CFABFC7E01C02D0 --log-driver json-file --log-opt max-size=10m --log-opt max-file=5 eclipse-mosquitto:innoage')
        Checkret_WIN(result)

        print ('Mount container innoage_webservice from image innoage-webservice...', end="")
        if IS_DFI_PROJECT:
            result = Shellcommand('docker run -idt -p 8161:8161 --network=icap_net --ip 172.30.0.100 --restart always --name innoAGE-WebService -e MQTT_USER=innoage -e MQTT_PASSWORD=B673AEBC6D65E7F42CFABFC7E01C02D0 -e MQTT_HOST=mqtt://innoAGE-Gateway:18883 -e MQTT_KEEPALIVE=120 --log-driver json-file --log-opt max-size=10m --log-opt max-file=5 innoage-webservice:v5-dfi')
        else:
            result = Shellcommand('docker run -idt -p 8161:8161 --network=icap_net --ip 172.30.0.100 --restart always --name innoAGE-WebService -e MQTT_USER=innoage -e MQTT_PASSWORD=B673AEBC6D65E7F42CFABFC7E01C02D0 -e MQTT_HOST=mqtts://innoAGE-Gateway:8883 -e MQTT_KEEPALIVE=120 --log-driver json-file --log-opt max-size=10m --log-opt max-file=5 innoage-webservice:v5-icap')
        Checkret_WIN(result)

        if Checkstatus('redis') == 0:
            print ('Mount container iCAP gateway from image icap_gateway...', end = "")
            result = Shellcommand('docker run -idt -p 1883:1883 --network=icap_net --ip 172.30.0.4 --restart always --name gateway -w=/root icap_gateway:v1.6.1')
            Checkret_WIN(result)
            time.sleep(2)
            if Checkstatus('adminDB') == 0:
                print ('Mount container authapi from image icap_webservice_authapi...', end="")
                result = Shellcommand("docker run -idt --network=icap_net --ip 172.30.0.6 --restart always --name authapi -w=/root icap_webservice_authapi:v1.6.1")
                Checkret_WIN(result)
                while Checkstatus('authapi') != 0:
                    time.sleep(2)
            else:
                print ("[Fail]iCAP Admin DB can't be installed properly")
                print ('Please try again')

            if Checkstatus('gateway') == 0 and Checkstatus('dataDB') == 0:
                print ('Mount container deviceapi from image icap_webservice_deviceapi...',end="")
                result = Shellcommand('docker run -idt --network=icap_net --ip 172.30.0.7 --restart always --name deviceapi -w=/root -v c:/"Program Files"/iCAP_Server/Images:/var/images icap_webservice_deviceapi:v1.6.1')
                Checkret_WIN(result)
                while Checkstatus('deviceapi') != 0:
                    time.sleep(2)
                print ('Mount container dashboardapi from image icap_webservice_dashboardapi...',end="")
                result = Shellcommand('docker run -idt --network=icap_net --ip 172.30.0.8 --restart always --name dashboardapi -v c:/"Program Files"/iCAP_Server/Images:/var/images -w=/root icap_webservice_dashboardapi:v1.6.1')
                Checkret_WIN(result)
                while Checkstatus('dashboardapi') != 0:
                    time.sleep(2)
                time.sleep(2)
                print ('Mount container core_dm from image icap_coreservice_dm...', end="")
                result = Shellcommand('docker run -idt --network=icap_net --ip 172.30.0.10 --restart always --name core_dm -v c:/"Program Files"/iCAP_Server:/var/iCAP -w=/root icap_coreservice_dm:v1.6.1')
                Checkret_WIN(result)
                print ('Mount container core_datahandler from image icap_coreservice_datahandler...', end="")
                result = Shellcommand('docker run -idt --network=icap_net --ip 172.30.0.11 --restart always --name core_datahandler -v c:/"Program Files"/iCAP_Server:/var/iCAP -w=/root icap_coreservice_datahandler:v1.6.1')
                Checkret_WIN(result)
                print ('Mount container core_storanalyzer from image icap_coreservice_storanalyzer...',end="")
                result = Shellcommand('docker run -idt --network=icap_net --ip 172.30.0.12 --restart always --name core_storanalyzer -v c:/"Program Files"/iCAP_Server:/var/iCAP -w=/root icap_coreservice_storanalyzer:v1.6.1')
                Checkret_WIN(result)
                print('Mount container core_notifyservice from image icap_coreservice_notify...',end="")
                result = Shellcommand('docker run -idt --network=icap_net --ip 172.30.0.13 --restart always --name core_notifyservice -v c:/"Program Files"/iCAP_Server:/var/iCAP -w=/root icap_coreservice_notify:v1.6.1')
                Checkret_WIN(result)
                print ('Mount container core_dashboardagent from image icap_coreservice_dashboardagent...', end="")
                result = Shellcommand('docker run -idt --network=icap_net --ip 172.30.0.16 --restart always --name core_dashboardagent -v c:/"Program Files"/iCAP_Server:/var/iCAP -w=/root icap_coreservice_dashboardagent:v1.6.1')
                Checkret_WIN(result)
                print('Mount container core_dlm from image icap_coreservice_dlm...',end="")
                result=Shellcommand('docker run -idt --network=icap_net --ip 172.30.0.17 --restart always --name core_dlm -v c:/"Program Files"/iCAP_Server:/var/iCAP -w=/root icap_coreservice_dlm:v1.6.1')
                Checkret_WIN(result)
                print('Mount container core_innoagemanager from image icap_coreservice_innoagemanager...',end="")
                result=Shellcommand('docker run -idt --network=icap_net --ip 172.30.0.18 --restart always --name core_innoagemanager -v c:/"Program Files"/iCAP_Server:/var/iCAP -w=/root icap_coreservice_innoagemanager:v1.6.1')
                Checkret_WIN(result)
                print('Mount container webservice_oobservice from image icap_webservice_oobservice...',end="")
                result=Shellcommand('docker run -idt --network=icap_net --ip 172.30.0.22 --restart always --name webservice_oobservice -e PORT=8165 -e ICAP_AUTH_SERVICE=172.30.0.6:50000 -e OOB_HOST=172.30.0.100:8161 -e OOB_WEBSOCKET_HOST=ws://172.30.0.100:8161/ws/innoage -e REDIS_HOST=172.30.0.5  -e DEBUG_MODE=true -p 8165:8165 icap_webservice_oobservice:v1.6.1')
                Checkret_WIN(result)
                print ('Mount container website from image icap_webservice_website...',end="")
                if ps.Checkarg() == 'DEMOKIT':
                    result = Shellcommand('docker run -idt --network=icap_net --ip 172.30.0.14 --restart always -p 80:80 -p 443:443 --name website icap_webservice_website:v1.6.1')
                else:
                    result = Shellcommand('docker run -idt --network=icap_net --ip 172.30.0.14 --restart always -p 80:80 --name website icap_webservice_website:v1.6.1')
                Checkret_WIN(result)
            else:
                print("[Fail]iCAP gateway and dataDB can't be installed properly")
                print('Please try again')
        else:
            print("[Fail]iCAP Redis can't be installed properly")
            print('Please try again')

        time.sleep(3)
else:
    # #color setting
    class bcolors:
        HEADER = '\033[30;47m'
        YELLO = '\033[33m'
        OKGREEN = '\033[92m'
        WARNING = '\033[1;31;40m'
        FAIL = '\033[91m'
        ENDC = '\033[0m'
        BOLD = '\033[1m'
        UNDERLINE = '\033[4m'

    def terminal_size():
        th, tw, hp, wp = struct.unpack('HHHH',fcntl.ioctl(0, termios.TIOCGWINSZ,struct.pack('HHHH', 0, 0, 0, 0)))
        return tw,th    

    def Print_eula():
        width,height = terminal_size()
        line=0
        page=0
        cursor=[0]
        counter=0
        with open("eula.md","r")as eula:
            while True:
                try:
                    c=eula.read(1)
                    counter=counter+1
                    if c=='\n':
                        line=line+1
                    if line==height-2 or eula.tell()==15717:
                        print("")
                        print(bcolors.HEADER + '(d)Page Down,(u)Page UP,(q)Quit' +bcolors.ENDC)
                        while True:
                            input=kb.getch()
                            if page==0:
                                if input=='q':
                                    eula.close()
                                    print(input)
                                    break
                                if input=='d':
                                    line=0
                                    page=page+1
                                    counter=0
                                    os.system('cls' if os.name == 'nt' else 'clear')
                                    break
                            elif eula.tell()==15717:
                                if input=='q':
                                    eula.close()
                                    print(input)
                                    break
                                if input=='u':
                                    line=0
                                    page=page-1
                                    eula.seek(cursor[page])
                                    counter=0
                                    os.system('cls' if os.name == 'nt' else 'clear')
                                    break

                            else:
                                if input=='q':
                                    eula.close()
                                    print(input)
                                    break
                                if input=='d':
                                    line=0
                                    if len(cursor)==page:
                                        cursor.append(eula.tell()-counter)
                                    page=page+1
                                    counter=0
                                    os.system('cls' if os.name == 'nt' else 'clear')
                                    break
                                if input=='u':
                                    line=0
                                    page=page-1
                                    eula.seek(cursor[page])
                                    counter=0
                                    os.system('cls' if os.name == 'nt' else 'clear')
                                    break
                    print (c,end="")
                except:
                    break

    class KBHit:
        def __init__(self):
            if os.name =='nt':
                pass
            else:
            # Save the terminal settings
                self.fd = sys.stdin.fileno()
                self.new_term = termios.tcgetattr(self.fd)
                self.old_term = termios.tcgetattr(self.fd)
            # New terminal setting unbuffered
                self.new_term[3] = (self.new_term[3] & ~termios.ICANON & ~termios.ECHO)
                termios.tcsetattr(self.fd, termios.TCSAFLUSH, self.new_term)
            # Support normal-terminal reset at exit
                atexit.register(self.set_normal_term)
        def set_normal_term(self):
            if os.name == 'nt':
                pass
            else:
                termios.tcsetattr(self.fd, termios.TCSAFLUSH, self.old_term)
        def getch(self):
            s = ''
            if os.name == 'nt':
                return msvcrt.getch().decode('utf-8')
            else:
                return sys.stdin.read(1)
        def getstr(self):
            s=''
            return sys.stdin.read(4)

        def kbhit(self):
            ''' Returns True if keyboard character was hit, False otherwise.
            '''
            if os.name == 'nt':
                return msvcrt.kbhit()
            else:
                dr,dw,de = select([sys.stdin], [], [], 0)
                return dr != []

    def Checkret(return_code):
        if return_code==0:
            print (bcolors.OKGREEN +'Success'+bcolors.ENDC)
        else:
            print (bcolors.WARNING +'Fail'+bcolors.ENDC)

    def Load_image(name):
        p = subprocess.Popen('sudo docker load --input '+name, stdout=subprocess.PIPE, shell=True)
        p_status = p.wait()
        Checkret(p_status)
    def Remove_container(name):
        print ('Remove container '+name+'...',end="")
        p = subprocess.Popen('sudo docker rm -f '+name, stdout=subprocess.PIPE, shell=True, stderr=subprocess.PIPE)
        p_status = p.wait()
        print (bcolors.OKGREEN +'Success'+bcolors.ENDC)


class ProjectSetting:
    def Checkarg(self):
        parser = ArgumentParser()
        parser.add_argument("-o", "--optional-arg", help = "STRESS DEMOKIT DEBUG", dest = "opt", 
            default="default") 
        args = parser.parse_args()
        return args.opt
#func
client = docker.from_env()
def Checkstatus(name):
    container = client.containers.get(name)
    if container.status=='running':
        return 0
    elif container.status=='exited':
        return -1

def Shellcommand(command):
    p = subprocess.Popen(command, stdout=subprocess.PIPE, shell=True)
    (output, err) = p.communicate()
    p_status = p.wait()
    return p_status

#main
try:
    
    if __name__ == "__main__":
        MOCK_DATA_FLAG = False
        OPEN_PORT_FLAG = False
        RUN_INNOAGE_MANAGER = True
        ps = ProjectSetting()
        
        if ps.Checkarg() == 'STRESS':
            OPEN_PORT_FLAG = True
            MOCK_DATA_FLAG = True
        if ps.Checkarg() == 'DEMOKIT':
            MOCK_DATA_FLAG = True
            RUN_INNOAGE_MANAGER = False
        if ps.Checkarg() == 'DEBUG':
            OPEN_PORT_FLAG = True
        
        if os.name == 'nt':
            if ctypes.windll.shell32.IsUserAnAdmin():
                if (OPEN_PORT_FLAG == True):
                    shutil.copy('.\\Config\\open_port_adminDB.yml', '.\\docker-compose.yml')
                else:
                    shutil.copy('.\\Config\\close_port_adminDB.yml', '.\\docker-compose.yml')
                
                begin_install(MOCK_DATA_FLAG, OPEN_PORT_FLAG)
                os.remove('docker-compose.yml')
            else:
                runAsAdmin()
        else:
            print (bcolors.HEADER + '===Start iCAP Server installation script===' +bcolors.ENDC)
            print ('Checking permission...')

            if os.getuid()!=0:
                print (bcolors.WARNING +'[Fail]Please run iCAP server installer with root permission')
                print ('Usage: sudo ./iCAP_Server_Installer' + bcolors.ENDC)
                sys.exit()
            elif os.getuid()==0:
                print (bcolors.OKGREEN +'[Success]Permission permitted' +bcolors.ENDC)
                print ('iCAP Service has an EULA that needs you consent.')
                print ('If you use it, you grant consent as well.')
                print ('Press any key to print EULA...')
                kb = KBHit()
                while (True):
                    if kb.kbhit():
                        break;
                Print_eula()
                print(bcolors.YELLO+ "Do you accept the EULA? (yes/no)"+bcolors.ENDC)
                keyin=[]
                while (True):
                    input=kb.getch()
                    print(input,end="")
                    if input!='\n':
                        keyin.append(input)
                        continue
                    elif input=='\n':
                        if keyin[0]=='y':
                            if len(keyin)==3:
                                if keyin[1]=='e' and keyin[2]=='s':
                                    break
                            else:
                                print("")
                                print(bcolors.YELLO+ 'Please enter "yes"'+bcolors.ENDC)
                                keyin=[]
                                continue

                        elif keyin[0]=='n':
                            print("")
                            sys.exit()
                        else:
                            print("")
                            sys.exit()

                print("...")
                # print ('Loading iCAP Cluster Manager image...',end="")
                # Load_image('icap_cluster_manager.tar')
                
                print ('Loading iCAP Data DB image...',end="")
                Load_image('icap_datadb.tar')

                print ('Loading iCAP Admin DB image...',end="")
                Load_image('icap_admindb.tar')

                print ('Loading iCAP Redis Cache image...',end="")
                Load_image('icap_redis.tar')
        
                print ('Loading iCAP Gateway image...',end="")
                Load_image('icap_gateway.tar')

                print ('Loading iCAP Web-service : Authentication API image...',end="")
                Load_image('icap_webservice_authapi.tar')

                print ('Loading iCAP Web-service : Device API image...',end="")
                Load_image('icap_webservice_deviceapi.tar')

                print ('Loading iCAP Web-service : Dashboard API image...',end="")
                Load_image('icap_webservice_dashboardapi.tar')

                print ('Loading iCAP Web-service : OOB Service image...',end="")
                Load_image('icap_webservice_oobservice.tar')

                print ('Loading iCAP Web-service : Website image...',end="")
                Load_image('icap_webservice_website.tar')

                print ('Loading iCAP DB Checker image...',end="")
                Load_image('icap_dbchecker.tar')

                # ps = ProjectSetting()
                # if ps.Checkarg()=='STRESS' or ps.Checkarg()=='DEMOKIT':
                if (MOCK_DATA_FLAG == True):
                    print ('Loading iCAP Mock data generator...',end="")
                    Load_image('icap_mockdata.tar')

                print ('Loading innoAge Gateway image...',end="")
                Load_image('innoAGE-Gateway.tar')

                print ('Loading innoAge Web-service image...',end="")
                Load_image('innoAGE-WebService.tar')
                
                print ('Loading iCAP Core service : Device Management image...',end="")
                Load_image('icap_coreservice_dm.tar')

                print ('Loading iCAP Core service : Data Handler image...',end="")
                Load_image('icap_coreservice_datahandler.tar')

                print ('Loading iCAP Core service : Storage Analyzer image...',end="")
                Load_image('icap_coreservice_storanalyzer.tar')

                print ('Loading iCAP Core service : Notification Service image...',end="")
                Load_image('icap_coreservice_notify.tar')

                print ('Loading iCAP Core service : Dashboard Agent image...',end="")
                Load_image('icap_coreservice_dashboardagent.tar')

                print ('Loading iCAP Core service : Data Life Manager image...',end="")
                Load_image('icap_coreservice_dlm.tar')

                print ('Loading iCAP Core service : innoAge Manager image...',end="")
                Load_image('icap_coreservice_innoagemanager.tar')

                print ('Starting containers...')
                reInstall=0
                p = subprocess.Popen("sudo docker network inspect icap_net", stdout=subprocess.PIPE, shell=True,stderr=subprocess.PIPE)
                p_status=p.wait()
                if p_status==0:
                    keyin=[]
                    print("Seems the iCAP server was installed,do you want to re-install?(y/n)")
                    while True:
                        input=kb.getch()
                        print(input,end="")
                        keyin.append(input)
                        if keyin[0]=='y':
                            if len(keyin)==2:
                                if keyin[1]=='\n':
                                    break
                        elif keyin[0]=='n':
                            print("")
                            sys.exit()
                        else:
                            keyin=[]
                            continue
                    print('Try to remove iCAP server continers...')
                    Remove_container('innoage_gateway')
                    Remove_container('innoage_webservice')
                    Remove_container('core_dm')
                    Remove_container('core_datahandler')
                    Remove_container('core_storanalyzer')
                    Remove_container('core_notifyservice')
                    Remove_container('core_dashboardagent')
                    Remove_container('core_dlm')
                    Remove_container('core_innoagemanager')
                    Remove_container('webservice_oobservice')
                    Remove_container('dashboardapi')
                    Remove_container('deviceapi')
                    Remove_container('authapi')
                    # Remove_container('icap_cluster_manager')
                    Remove_container('gateway')
                    Remove_container('redis')
                    Remove_container('innoAGE-WebService')
                    Remove_container('innoAGE-Gateway')
                    Remove_container('adminDB')
                    Remove_container('dataDB')
                    Remove_container('website')
                    reInstall = 1
                else:
                    Shellcommand('sudo docker network create --driver bridge --subnet 172.30.0.0/16 icap_net')
                    Shellcommand('sudo mkdir -p /var/iCAP/')
                    Shellcommand('sudo mkdir -p /var/iCAP/AdminDB')
                    Shellcommand('sudo mkdir -p /var/iCAP/DataDB')
                    Shellcommand('sudo tar -C /var/iCAP/ -zxvf Images.tar.gz')
                    print ('Add a script at startup...', end= "")
                    sys.stdout.flush()
                    Shellcommand('sudo cp setting.json /var/iCAP/')
                    Shellcommand('sudo cp setting.env /var/iCAP/')
                    Shellcommand('sudo cp Log_Update.sh /var/iCAP/')
                    if MOCK_DATA_FLAG == True:
                        Shellcommand('sudo cp DeviceCount.json /var/iCAP/')

                # print ('Mount container clustermanager from image icap_cluster_manager...',end="")
                # result=Shellcommand('sudo docker run -idt --network=icap_net --ip 172.30.0.15 --restart always --privileged --name icap_cluster_manager -v /var/run/docker.sock:/var/run/docker.sock -v /etc/localtime:/etc/localtime:ro -w=/root icap_cluster_manager:v1.6.1')
                # Checkret(result)
                print ('Mount container dataDB from image icap_datadb...',end="")
                result=Shellcommand('sudo docker run -idt --network=icap_net --ip 172.30.0.2 --restart always --name dataDB -p 27017:27017 -v /var/iCAP/DataDB:/data/db -v /etc/localtime:/etc/localtime:ro -e MONGO_INITDB_ROOT_USERNAME=root -e MONGO_INITDB_ROOT_PASSWORD=root -e MONGO_INITDB_DATABASE=iCAP icap_datadb:v1.6.1')
                Checkret(result)
                print ('Mount container adminDB from image icap_admindb...',end="")
                if ps.Checkarg()=='STRESS' or ps.Checkarg()=='DEBUG':
                    result=Shellcommand('sudo docker run -idt --network=icap_net --ip 172.30.0.3 --restart always --name adminDB -p 3389:3306 -v /var/iCAP/AdminDB:/var/lib/mysql -v /etc/localtime:/etc/localtime:ro -e MYSQL_ROOT_PASSWORD=admin icap_admindb:v1.6.1')
                    Checkret(result)
                else:
                    result=Shellcommand('sudo docker run -idt --network=icap_net --ip 172.30.0.3 --restart always --name adminDB -v /var/iCAP/AdminDB:/var/lib/mysql -v /etc/localtime:/etc/localtime:ro -e MYSQL_ROOT_PASSWORD=admin icap_admindb:v1.6.1')
                    Checkret(result)
                print ('Mount container redis from image icap_redis...',end="")
                if ps.Checkarg()=='STRESS' or ps.Checkarg()=='DEBUG':
                    result=Shellcommand('sudo docker run -idt --network=icap_net --ip 172.30.0.5 --restart always --name redis -v /etc/localtime:/etc/localtime:ro -p 6379:6379 icap_redis:v1.6.1')
                    Checkret(result)
                else:
                    result=Shellcommand('sudo docker run -idt --network=icap_net --ip 172.30.0.5 --restart always --name redis -v /etc/localtime:/etc/localtime:ro icap_redis:v1.6.1')
                    Checkret(result)
                if Checkstatus('adminDB')==0:
                    print ('Mount container dbchecker from image icap_dbchecker...',end="")
                    result=Shellcommand('sudo docker run -idt --network=icap_net --ip 172.30.0.50 --name dbchecker -v /etc/localtime:/etc/localtime:ro icap_dbchecker:v1.6.1')
                    Checkret(result)
                    while Checkstatus('dbchecker')!=-1:
                        time.sleep(2)
                    result=Shellcommand('sudo docker rm dbchecker')
                    result=Shellcommand('sudo docker rmi icap_dbchecker:v1.6.1')
                else:
                    print(bcolors.WARNING+"[Fail]iCAP Admin DB can't be installed properly")
                    print('Please try again'+bcolors.ENDC)
                if reInstall==0:
                    if ps.Checkarg()=='STRESS' or ps.Checkarg()=='DEMOKIT':
                            result=Shellcommand('sudo docker run -idt --network=icap_net --ip 172.30.0.51 --name mockdata -v /etc/localtime:/etc/localtime:ro icap_mockdata:v1.6.1')
                            time.sleep(2)
                            while Checkstatus('mockdata')!=-1:
                                time.sleep(2)
                            result=Shellcommand('sudo docker rm mockdata')
                            result=Shellcommand('sudo docker rmi icap_mockdata:v1.6.1')

                print ('Mount container innoage_gateway from image eclipse-mosquitto...', end="")
                result = Shellcommand('sudo docker run -idt -p 8883:8883 -p 18883:18883 --network=icap_net --ip 172.30.0.101 --restart always --name innoAGE-Gateway -e MQTT_USER=innoage -e MQTT_PASSWORD=B673AEBC6D65E7F42CFABFC7E01C02D0 --log-driver json-file --log-opt max-size=10m --log-opt max-file=5 eclipse-mosquitto:innoage')
                Checkret(result)

                print ('Mount container innoage_webservice from image innoage-webservice...', end="")
                if IS_DFI_PROJECT:
                    result = Shellcommand('sudo docker run -idt -p 8161:8161 --network=icap_net --ip 172.30.0.100 --restart always --name innoAGE-WebService -e MQTT_USER=innoage -e MQTT_PASSWORD=B673AEBC6D65E7F42CFABFC7E01C02D0 -e MQTT_HOST=mqtt://innoAGE-Gateway:18883 -e MQTT_KEEPALIVE=120 --log-driver json-file --log-opt max-size=10m --log-opt max-file=5 innoage-webservice:v5-dfi')
                else:
                    result = Shellcommand('sudo docker run -idt -p 8161:8161 --network=icap_net --ip 172.30.0.100 --restart always --name innoAGE-WebService -e MQTT_USER=innoage -e MQTT_PASSWORD=B673AEBC6D65E7F42CFABFC7E01C02D0 -e MQTT_HOST=mqtts://innoAGE-Gateway:8883 -e MQTT_KEEPALIVE=120 --log-driver json-file --log-opt max-size=10m --log-opt max-file=5 innoage-webservice:v5-icap')
                Checkret(result)
                
                if Checkstatus('redis')==0:
                    print ('Mount container gateway from image icap_gateway...',end="")
                    result=Shellcommand('sudo docker run -idt -p 1883:1883 --network=icap_net --ip 172.30.0.4 --restart always --name gateway -v /etc/localtime:/etc/localtime:ro -w=/root icap_gateway:v1.6.1')
                    Checkret(result)
                    time.sleep(2)
                    if Checkstatus('adminDB')==0:
                        print ('Mount container authapi from image icap_webservice_authapi...',end="")
                        result=Shellcommand("sudo docker run -idt --network=icap_net --ip 172.30.0.6 --restart always --name authapi -v /etc/localtime:/etc/localtime:ro -w=/root icap_webservice_authapi:v1.6.1")
                        Checkret(result)
                    else:
                        print(bcolors.WARNING+"[Fail]iCAP Admin DB can't be installed properly")
                        print('Please try again'+bcolors.ENDC)

                    if Checkstatus('gateway') == 0 and Checkstatus('dataDB') == 0:
                        print ('Mount container deviceapi from image icap_webservice_deviceapi...',end="")
                        result=Shellcommand('sudo docker run -idt --network=icap_net --ip 172.30.0.7 --restart always --name deviceapi -w=/root -v /var/iCAP/Images:/var/images -v /etc/localtime:/etc/localtime:ro icap_webservice_deviceapi:v1.6.1')
                        Checkret(result)
                        print('Mount container dashboardapi from image icap_webservice_dashboardapi...',end="")
                        result=Shellcommand('sudo docker run -idt --network=icap_net --ip 172.30.0.8 --restart always --name dashboardapi -w=/root -v /var/iCAP/Images:/var/images  -v /etc/localtime:/etc/localtime:ro icap_webservice_dashboardapi:v1.6.1')
                        Checkret(result)
                        print ('Mount container core_dm from image icap_coreservice_dm...',end="")
                        result=Shellcommand('sudo docker run -idt --network=icap_net --ip 172.30.0.10 --restart always --name core_dm -v /var/iCAP:/var/iCAP -v /etc/localtime:/etc/localtime:ro -w=/root icap_coreservice_dm:v1.6.1')
                        Checkret(result)
                        print ('Mount container core_datahandler from image icap_coreservice_datahandler...',end="")
                        result=Shellcommand('sudo docker run -idt --network=icap_net --ip 172.30.0.11 --restart always --name core_datahandler -v /var/iCAP:/var/iCAP -v /etc/localtime:/etc/localtime:ro -w=/root icap_coreservice_datahandler:v1.6.1')
                        Checkret(result)
                        print ('Mount container core_storanalyzer from image icap_coreservice_storanalyzer...',end="")
                        result=Shellcommand('sudo docker run -idt --network=icap_net --ip 172.30.0.12 --restart always --name core_storanalyzer -v /var/iCAP:/var/iCAP -v /etc/localtime:/etc/localtime:ro -w=/root icap_coreservice_storanalyzer:v1.6.1')
                        Checkret(result)
                        print('Mount container core_notifyservice from image icap_coreservice_notify...',end="")
                        result=Shellcommand('sudo docker run -idt --network=icap_net --ip 172.30.0.13 --restart always --name core_notifyservice -v /var/iCAP:/var/iCAP -v /etc/localtime:/etc/localtime:ro -w=/root icap_coreservice_notify:v1.6.1')
                        Checkret(result)
                        print ('Mount container core_dashboardagent from image icap_coreservice_dashboardagent...',end="")
                        result=Shellcommand('sudo docker run -idt --network=icap_net --ip 172.30.0.16 --restart always --name core_dashboardagent -v /var/iCAP:/var/iCAP -v /etc/localtime:/etc/localtime:ro -w=/root icap_coreservice_dashboardagent:v1.6.1')
                        Checkret(result)
                        print('Mount container core_dlm from image icap_coreservice_dlm...',end="")
                        result=Shellcommand('sudo docker run -idt --network=icap_net --ip 172.30.0.17 --restart always --name core_dlm -v /var/iCAP:/var/iCAP -v /etc/localtime:/etc/localtime:ro -w=/root icap_coreservice_dlm:v1.6.1')
                        Checkret(result)
                        
                        if (RUN_INNOAGE_MANAGER == True):
                            print('Mount container core_innoagemanager from image icap_coreservice_innoagemanager...',end="")
                            result=Shellcommand('sudo docker run -idt --network=icap_net --ip 172.30.0.18 --restart always --name core_innoagemanager -v /var/iCAP:/var/iCAP -v /etc/localtime:/etc/localtime:ro -w=/root icap_coreservice_innoagemanager:v1.6.1')
                            Checkret(result)
                        
                        print('Mount container webservice_oobservice from image icap_webservice_oobservice...',end="")
                        result=Shellcommand('sudo docker run -idt --network=icap_net --ip 172.30.0.22 --restart always --name webservice_oobservice -e PORT=8165 -e ICAP_AUTH_SERVICE=172.30.0.6:50000 -e OOB_HOST=172.30.0.100:8161 -e OOB_WEBSOCKET_HOST=ws://172.30.0.100:8161/ws/innoage -e REDIS_HOST=172.30.0.5  -e DEBUG_MODE=true -v /etc/localtime:/etc/localtime:ro icap_webservice_oobservice:v1.6.1')
                        Checkret(result)
                        print ('Mount container website from image icap_webservice_website...',end="")
                        if ps.Checkarg() == 'DEMOKIT':
                            result=Shellcommand('sudo docker run -idt --network=icap_net --ip 172.30.0.14 --restart always -p 80:80 -p 443:443 -v /etc/localtime:/etc/localtime:ro --name website icap_webservice_website:v1.6.1')
                        else:
                            result=Shellcommand('sudo docker run -idt --network=icap_net --ip 172.30.0.14 --restart always -p 80:80 -v /etc/localtime:/etc/localtime:ro --name website icap_webservice_website:v1.6.1')
                        Checkret(result)
                    else:
                        print(bcolors.WARNING+"[Fail]iCAP gateway and dataDB can't be installed properly")
                        print('Please try again'+bcolors.ENDC)
                else:
                    print(bcolors.WARNING+"[Fail]iCAP Redis can't be installed properly")
                    print('Please try again'+bcolors.ENDC)

                time.sleep(3)
except KeyboardInterrupt:
    print("")
    sys.exit()