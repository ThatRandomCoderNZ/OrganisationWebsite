using OrganisationWebsite.DatabaseRepository;
using OrganisationWebsite.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace OrganisationWebsite.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        DBConnection dbConnection = new DBConnection();

        public ActionResult LoginScreen()
        {
            //Handle Displaying the Login Screen
            LoginDataModel loginData = new LoginDataModel();
            foreach(string key in TempData.Keys)
            {
                if(key == "password")
                {
                    loginData.PasswordError = TempData[key].ToString();
                }
                if(key == "username")
                {
                    loginData.UserNameError = TempData[key].ToString();
                }
            }
            
            
            return View(loginData);
        }

        [HttpPost]
        public ActionResult LoginScreen(LoginDataModel loginData) {
            //Here Handle all of the user's inputed information. Cleaning and validation may be required here.
            bool inputIsClean = true;
            //Clean all data so no MySQL Injections
            //Check with database
            if (!IsInputClean(loginData.UserName))
            {
                inputIsClean = false;
                TempData["username"] = "Usernames can only contain letters and numbers.";
            }
            if (!IsInputClean(loginData.Password)) {
                inputIsClean = false;
                TempData["password"] = "Passwords can only contain letters and numbers.";
            }
            //If Everything checks out then update the cookie with the user's id and proceed to the main index page.
            //Otherwise return LoginScreen with error messages.
            if (!inputIsClean)
            {
                return RedirectToAction("LoginScreen", "Login");
            }  
            else
            {
                string query = @"SELECT U.user_id, U.username, U.password 
                                 FROM student93061.users U
                                 WHERE U.username = '" + loginData.UserName + "' AND U.password = SHA2('" + loginData.Password + "', 256);";
                string[] columns = new string[]{ "user_id", "username", "password" };
                Dictionary<string, List<string>> loginResults = dbConnection.RetrieveFromDatabase(columns, query);
                //Login was successful
                if(loginResults["username"].Count > 0)
                {
                    Debug.WriteLine("Login Successful");
                    Response.Cookies["UserID"].Value = loginResults["user_id"][0];
                    Response.Cookies["UserID"].Expires = DateTime.Now.AddDays(1);
                    Debug.WriteLine("Cookie should equal: " + loginResults["user_id"][0] + " Cookie does equal: " + Response.Cookies["UserID"].Value);
                    return RedirectToAction("Index", "Home");
                }
                //Login was unsuccessful
                else
                {

                }
            }
            return RedirectToAction("LoginScreen", "Login");
        }

        public ActionResult SignUp() {

            return View();
        }

        [HttpPost]
        public ActionResult SignUp(LoginDataModel formData) {
            // First check to see if the username is already present
            string check_query = @"SELECT username FROM users WHERE username = @user_name";
            string[] check_columns = new string[] { "username" };
            List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();
            parameters.Add(new Tuple<string, string>("@user_name", formData.NewUserName));
            Dictionary<string, List<string>> results = dbConnection.RetrieveFromDatabase(check_columns, check_query, parameters);
            if(results["username"].Count == 0)
            {
                string newUser = @"INSERT INTO `student93061`.`users` (`username`, `password`) VALUES (@user_name, SHA2(@password, 256));";
                List<Tuple<string, string>> newParameters = new List<Tuple<string, string>>();
                newParameters.Add(new Tuple<string, string>("@user_name", formData.NewUserName));
                newParameters.Add(new Tuple<string, string>("@password", formData.NewPassword));
                dbConnection.InsertIntoDatabase(newUser, newParameters);
            }
            return RedirectToAction("LoginScreen");
        }

        bool IsInputClean(string testString) {
            string[] legal_letters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"};
            if(testString == null)
            {
                return true;
            }
            else if(testString.Length == 0)
            {
                return true;
            }
            for (int i = 0; i < testString.Length; i++) {
                if (legal_letters.Contains(testString[i].ToString()) == false) {
                    return false;
                }
            }
            return true;
        }
    }
}