using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.Interface
{
    public interface IPermission
    {
        bool CheckCreatePermission(string loginName, DataDefine.PermissionFlag permission);
        bool CheckUpdatePermission(string loginName, DataDefine.PermissionFlag permission);
        bool CheckDeletePermission(string loginName, DataDefine.PermissionFlag permission);
    }
}