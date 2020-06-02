using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CarRent.BL;
using RentIt.Models;
using System.Web.Security;



namespace RentIt.Controllers
{
    public class HomeController : Controller
    {
        private readonly GuestQuery _guest;

        private readonly ManagerQuery _manager;

        private  CompanyRoleProvider _roleprovider;

        public HomeController()
        {
            _guest = new GuestQuery();
            _manager = new ManagerQuery();
           _roleprovider = new CompanyRoleProvider();
            
        }
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Login()
        {
            return View(new LoginUser());
        }
       
        [HttpPost]
        public ActionResult Login(LoginUser login)
        {
            if (ModelState.IsValid)
            {

                if (Validate(login))
                {
                    var roles = _roleprovider.GetRolesForUser(login.Username).ToArray();

                    //foreach (var role in roles)
                    //{
                    //    User.IsInRole(role);
                    //}          
                    SetAuth(login.Username, roles);                    
                    var a =  User.IsInRole("manager");
                    return RedirectToAction("Index");
                }
                else
                {
                    FormsAuthentication.SignOut();
                    return View(login);
                }
            }

            else
            {
                return View("Login");
            }
        }

        private void SetAuth(string userName, string[] roles)
        {
            //FormsAuthentication.SetAuthCookie(userName, false);
            string strRoles = string.Join(",", roles);
            var authTicket = new FormsAuthenticationTicket(
                1,                             // version
                userName,                      // user name
                DateTime.Now,                  // created
                DateTime.Now.AddMinutes(20),   // expires
                false,                    // persistent?
                strRoles                        // can be used to store roles
                );

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            HttpContext.Response.Cookies.Add(authCookie);
        }


        private bool Validate(LoginUser login)
        {

            bool userExists = _guest.ClientExist(login.converttoUser(login));

            if (!userExists)
            {
                ModelState.AddModelError(string.Empty, "User does not exist");
                return false;
            }

            else if (!_guest.PasswordMatches(login.converttoUser(login)))
            {

                ModelState.AddModelError(string.Empty, "Password does not match");
                return false;
            }

            else
            {
                return true;
            }
        }





        public ActionResult SignUp()
        {

            return View();
        }


        [HttpPost]
        public ActionResult SignUp(UserView CVM)
        {

            if (ModelState.IsValid)
            {

                if (_roleprovider.UserExists(CVM.UserName))
                {
                    ModelState.AddModelError(string.Empty, "User Already Exists");
                    return View(CVM);
                }

                else
                {
                    var domainclient = CVM.toBaseClient_Details();


                    _manager.AddClient(domainclient);


                    string[] users = { CVM.UserName };

                    string[] roles = { "Customer" };

                    _roleprovider.AddUsersToRoles(users, roles);

                    TempData["Success"] = "You were Signed Up Successfully! Please Log In";
                    ModelState.Clear();
                    var model = new UserView();
                    return RedirectToAction("Login");
                }
            }

            else
            {
                return View("SignUp");
            }

        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}