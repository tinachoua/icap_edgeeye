using ShareLibrary.DataTemplate;

namespace ShareLibrary.Interface
{
    public interface IEmployee :IImages
    {
        bool CheckAdmin(string loginName);
        bool CheckExists(string LoginName);
        bool CheckPWD(string Username, string Password);
        bool Create(EmployeeProfileTemplate e);
        bool? Delete(string loginName);
        EmployeeProfileTemplate Get(string LoginName);
        string[] GetList();
        bool? Update(EmployeeProfileTemplate p);
        SelectOptionTemplate[] GetName();
        int GetUserCommonMap(string loginName);
        void SetCommonMap(string loginName, int mapId);
        //EmailSearchTemplate[] FindEmailIncludePermission(string searchString);
        EmailSearchTemplate[] FindAllEmail(string searchString);
        string[] GetEmail(int permissionId);
        string[] GetEmailForTooltip(int permissionId);
        object GetEmployeeInfo(string email);
        string GetToken(string username);
    }
}