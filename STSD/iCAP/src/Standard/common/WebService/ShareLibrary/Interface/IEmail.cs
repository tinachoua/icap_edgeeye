using System;
using System.Collections.Generic;
using System.Text;
using ShareLibrary.DataTemplate;
using ShareLibrary.AdminDB;

namespace ShareLibrary.Interface
{
    public interface IEmail
    { 
        bool CteateOrUpdate(EmailSettingTemplate e);
        List<EmailSettingTemplate> GetEmailList(int? CompanyId);
        int? GetOwnerId(string deviceName);
        bool? Delete(string emailFrom);       
        bool Send(EmailSendingInfoTemplate e, int? employeeId);
        //Email GetEmail(int CompanyId);
        EmailSettingTemplate GetEmail(int CompanyId);
        List<string> GetEmployeeEmailList(int CompanyId);
        void Send(string user, EmailTestTemplate emailTestInfo);
        bool IsValidMailServerData(EmailTestTemplate emailTestInfo);
        bool IsValidEmail(string emailAddress);       
        bool IsValidPassword(string password);
        void Send(EmailMessageTemplate msgInfo);
    }
}
