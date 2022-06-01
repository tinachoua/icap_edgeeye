using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary
{
    public enum ReturnCode
    {
        //
        // Summary:
        //     Token Error
        TokenError = 1,
        //
        // Summary:
        //     Permission denied 
        PermissionDenied = 2,
        // Summary:
        //     Device can not be deleted when online.
        DeviceOnline = 3,
        // Summary:
        //     The File Extension is unacceptable..
        FileExtensionUnacceptable = 4,
        // Summary:
        //     The root user can not be deleted.
        RootCantBeDeleted = 5,
        // Summary:
        //     The request payload is null.
        PayloadNull = 6,
        // Summary:
        //     Duplicate login name
        DuplicateLoginName = 7,
        // Summary:
        //     Password format error.
        PWDFormatError = 8,
        // Summary:
        //     Password and verify-password are not the same.
        PWDVerifyError = 9,
        // Summary:
        //     The login name existed already.
        LoginNameExisted = 10,
        // Summary:
        //     Not enough access to create/update employee data.
        AuthorityNotEnough = 11,
        // Summary:
        // Incorrect account password
        IncorrectAccountPWD = 12,
        // Summary:
        // Incorrect account password
        HeaderParmNull = 13,
        // Summary:
        // Account error
        AccountError = 14,
        // Summary:
        // Password error
        PWDError = 15,
    }
}