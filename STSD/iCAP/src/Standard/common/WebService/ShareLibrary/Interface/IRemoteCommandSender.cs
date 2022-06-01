namespace ShareLibrary.Interface
{
    public interface IRemoteCommandSender
    {
        void SendRemoteCommand(string deviceName, string RemoteTarget, string RemoteCmd);
    }
}