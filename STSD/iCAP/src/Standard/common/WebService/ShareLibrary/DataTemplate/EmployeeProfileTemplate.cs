using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    public class EmployeeProfileTemplate
    {
        public string LoginName { get; set; }
        public string Email { get; set; }
        public string EmployeeNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PWD { get; set; }
        public string VerifyPWD { get; set; }
        public bool AdminFlag { get; set; }
        public bool EmailFlag { get; set; }
    }
}
