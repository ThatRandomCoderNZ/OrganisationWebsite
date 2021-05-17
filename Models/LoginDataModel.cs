using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrganisationWebsite.Models
{
    public class LoginDataModel
    {
        public int UserID { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string UserNameError { get; set; }

        public string PasswordError { get; set; }

        public string NewUserName { get; set; }

        public string NewPassword { get; set; }
    }
}