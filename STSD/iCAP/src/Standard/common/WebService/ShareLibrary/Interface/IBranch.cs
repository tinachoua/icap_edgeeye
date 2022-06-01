using System.Collections.Generic;
using ShareLibrary.AdminDB;
using ShareLibrary.DataTemplate;

namespace ShareLibrary.Interface
{
    public interface IBranch : IImages
    {
        //List<Branch> GetList();
        SelectOptionTemplate[] GetBranchList();
        SelectOptionTemplate[] GetBranchList(string devName);
        void Create(string branchName);
        void Update(BranchTemplate updateBranch);
        void Delete(int branchId);
        void DeviceAllocation(DeviceAllocationTemplate deviceAllocation);
        DeviceOption[] GetDeviceList(int branchId);
        bool NameExists(string branchName);
        bool NameExists(string branchName, int branchId);
        BranchTemplate GetInfo(int branchId);
        bool SaveInfo(BranchTemplate branchInfo);
        string GetBranchName(string devName);
    }
}