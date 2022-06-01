using ShareLibrary.AdminDB;

namespace ShareLibrary.Interface
{
    public interface IDevicecertificate
    {
        void Create(Devicecertificate cert);
        Devicecertificate Get(string Thumbprint);
        void Update(Devicecertificate cert);
    }
}