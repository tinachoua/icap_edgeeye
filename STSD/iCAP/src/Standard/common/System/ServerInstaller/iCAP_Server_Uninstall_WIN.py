try:
    import docker
except ImportError:
    import __docker__ as docker
import subprocess, ctypes, sys

def Remove_container_WIN(name):
    print ('Remove container ' + name + '...', end="")
    p = subprocess.Popen('docker rm -f '+name, stdout=subprocess.PIPE, shell=True, stderr =subprocess.PIPE)
    p_status = p.wait()
    print ('Success')

def Shellcommand(command):
    p = subprocess.Popen(command, stdout=subprocess.PIPE, shell=True)
    (output, err) = p.communicate()
    p_status = p.wait()
    return p_status

def runAsAdmin(cmdLine=None, wait=True):
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

if ctypes.windll.shell32.IsUserAnAdmin():
    Remove_container_WIN('website')
    Remove_container_WIN('webservice_oobservice')
    Remove_container_WIN('core_innoagemanager')
    Remove_container_WIN('core_dlm')
    Remove_container_WIN('core_dashboardagent')
    Remove_container_WIN('core_notifyservice')
    Remove_container_WIN('core_storanalyzer')
    Remove_container_WIN('core_datahandler')
    Remove_container_WIN('core_dm')
    Remove_container_WIN('dashboardapi')
    Remove_container_WIN('deviceapi')
    Remove_container_WIN('authapi')
    Remove_container_WIN('gateway')
    Remove_container_WIN('innoAGE-WebService')
    Remove_container_WIN('innoAGE-Gateway')
    Remove_container_WIN('redis')
    Remove_container_WIN('adminDB')
    Remove_container_WIN('dataDB')
    # Remove_container_WIN('icap_cluster_manager')
    Shellcommand('docker network rm icap_net')
    Shellcommand('rmdir /s/q "c:\\Program Files\\iCAP_Server\\AdminDB"')
    Shellcommand('rmdir /s/q "c:\\Program Files\\iCAP_Server\\config"')
    result = Shellcommand('netsh advfirewall firewall show rule name=ICAP_WEB_IN >nul')
    if result != 1:
        Shellcommand('netsh advfirewall firewall delete rule name="ICAP_WEB_IN"')

    result = Shellcommand('netsh advfirewall firewall show rule name=ICAP_WEB_OUT >nul')
    if result != 1:
        Shellcommand('netsh advfirewall firewall delete rule name="ICAP_WEB_OUT"')

    result = Shellcommand('netsh advfirewall firewall show rule name=MQTT_IN >nul')
    if result != 1:
        Shellcommand('netsh advfirewall firewall delete rule name="MQTT_IN"')
  
    result = Shellcommand('netsh advfirewall firewall show rule name=MQTT_OUT >nul')
    if result != 1:
        Shellcommand('netsh advfirewall firewall delete rule name="MQTT_OUT"')

    print('Remove iCAP success.')
else:
    runAsAdmin()