using ShareLibrary.AdminDB;

namespace ShareLibrary.Interface
{
    public interface ILicenselist
    {
        void Create(LicenseList lic);
        int DeviceCount();
        bool OverLimitation();
        string[] GetDeviceListToSetOffline();
    }
}