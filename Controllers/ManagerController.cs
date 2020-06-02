using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CarRent.BL;
using CarRent.Entities;
using RentIt.Models;
using System.IO;


namespace RentIt.Controllers
{
    public class ManagerController : Controller
    {

        private readonly ManagerQuery _manager;

        private readonly GuestQuery guest;

       // private readonly CompanyRoleProvider _roleprovider;

        private static IEnumerable<ModelView> allmodels;

        private static IEnumerable<CarView> allCars;

        private static IEnumerable<PurchaseView> allDeals;

        private static IEnumerable<UserView> allCustomers;

        private static IEnumerable<UserView> allEmployees;


        public ManagerController()
        {
            _manager = new ManagerQuery();
            guest = new GuestQuery();
           // _roleprovider = new CompanyRoleProvider();
        }
        [Authorize(Roles = "Manager")]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Manager")]
        public JsonResult HelpAjax()
        {

            allCars = _manager.AllCars().Select(c => new CarView(c));

            allmodels = guest.AllModels().Select(c => new ModelView(c));

            allDeals = _manager.AllPurchases().Select(d => new PurchaseView(d));

            allCustomers = _manager.AllCustomers().Select(u => new UserView(u));

            allEmployees = _manager.AllEmployees().Select(u => new UserView(u));

            var managerHelper = new CalcModel();

            managerHelper.AllCarModels = allmodels;

            managerHelper.AllDeals = allDeals;

            managerHelper.AllCustomers = allCustomers;

            managerHelper.AllEmployees = allEmployees;

            managerHelper.AllCars = allCars;

            return Json(managerHelper, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public ActionResult ManagerActions(CalcModel cm)
        {
            CalcModel result = new CalcModel();


            switch (cm.ManagerAction)
            {

                ///////// MODELS ///////////

                case "AddModel":
                    return PartialView("AddModel");


                case "EditModel":
                    var model = (from m in allmodels where m.ID == cm.ID select m).FirstOrDefault();
                    return PartialView("EditModel", model);


                /////////////  CARS /////////////

                case "AddCar":
                    return PartialView("AddCar");


                case "AddPicture":
                    var carp = (from c in allCars where c.ID == cm.ID select c).FirstOrDefault();
                    return PartialView("AddCarPicture", carp);


                case "EditCar":
                    var car = (from c in allCars where c.ID == cm.ID select c).FirstOrDefault();
                    return PartialView("EditCar", car);


                case "DeleteCar":
                    _manager.DeleteCar(cm.ID);
                    CalcModel carresult = new CalcModel();
                    carresult.ActionResult = "Model Deleted";
                    return Json(carresult, JsonRequestBehavior.AllowGet);


                /////////////// USERS ////////////////////

                case "AddUser":
                    return PartialView("AddUser");



                case "EditUser":
                    var customer = (from c in allCustomers where c.ID == cm.ID select c).FirstOrDefault();
                    return PartialView("EditUser", customer);



                case "EditEmployee":
                    var employee = (from c in allEmployees where c.ID == cm.ID select c).FirstOrDefault();
                    return PartialView("EditUser", employee);


                case "DeleteCustomer":
                    _manager.DeleteClient(cm.ID);
                    CalcModel customerresult = new CalcModel();
                    customerresult.ActionResult = "Model Deleted";
                    return Json(customerresult, JsonRequestBehavior.AllowGet);



                case "AddDeal":
                    return PartialView("AddDeal");

                case "EditDeal":
                    var deal = (from d in allDeals where d.ID == cm.ID select d).FirstOrDefault();
                    return PartialView("EditDeal", deal);

                case "DeleteDeal":
                    _manager.DeletePurchas(cm.ID);
                    CalcModel dealresult = new CalcModel();
                    dealresult.ActionResult = "Model Deleted";
                    return Json(dealresult, JsonRequestBehavior.AllowGet);

                default:
                    return null;


            }
        }
        [Authorize(Roles = "Manager")]
        public ActionResult SubmitNewCustomer(UserView uv)
        {
            if (ModelState.IsValid)
            {


                /*if (_roleprovider.UserExists(uv.UserName))
                {
                    ModelState.AddModelError(string.Empty, "Username already in use");

                    return PartialView("AddUser");
                }*/

                var managerHelper = new CalcModel();
                _manager.AddClient(uv.toBaseClient_Details());

                string[] users = { uv.UserName };

                string[] roles = { "Customer" };

               // _roleprovider.AddUsersToRoles(users, roles);

                managerHelper.ActionResult = "New Customer Submitted";
                return Json(managerHelper, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return PartialView("AddUser");
            }
        }





        [Authorize(Roles = "Manager")]
        public ActionResult SubmitEditCustomer(UserView uv)
        {
            if (ModelState.IsValid)
            {
                var managerHelper = new CalcModel();
                _manager.UpdateClient(uv.toBaseClient_Details());
                managerHelper.ActionResult = "Customer edit submitted";
                return Json(managerHelper, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return PartialView("EditUser", uv);
            }
        }
        [Authorize(Roles = "Manager")]
        public ActionResult SubmitNewEmployee(UserView uv)
        {
            if (ModelState.IsValid)
            {

               // if (_roleprovider.UserExists(uv.UserName))
               // {
                    ModelState.AddModelError(string.Empty, "Username already in use");

                   // return PartialView("AddUser");
              //  }



                var managerHelper = new CalcModel();

                var domainclient = uv.toBaseClient_Details();

                _manager.AddClient(domainclient);

                string[] users = { uv.UserName };

                string[] roles = { "Employee" };

              //  _roleprovider.AddUsersToRoles(users, roles);


                managerHelper.ActionResult = "New Employee Submitted";
                return Json(managerHelper, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return PartialView("AddUser");
            }
        }





        [Authorize(Roles = "Manager")]
        public ActionResult SubmitEditEmployee(UserView uv)
        {
            if (ModelState.IsValid)
            {
                var managerHelper = new CalcModel();
                _manager.UpdateClient(uv.toBaseClient_Details());
                managerHelper.ActionResult = "Employee edit submitted";
                return Json(managerHelper, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return PartialView("EditUser", uv);
            }
        }
        [Authorize(Roles = "Manager")]
        public ActionResult SubmitNewCar(CarView cv)
        {
            if (ModelState.IsValid)
            {
                var managerHelper = new CalcModel();
                _manager.AddCar(cv.toBaseCarDetails());
                managerHelper.ActionResult = "New car submitted";
                return Json(managerHelper, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return PartialView("AddCar");
            }
        }



        [Authorize(Roles = "Manager")]
        public ActionResult SubmitEditCar(CarView cv)
        {
            if (ModelState.IsValid)
            {
                var managerHelper = new CalcModel();
                _manager.UpdateCar(cv.toBaseCarDetails());
                managerHelper.ActionResult = "Car edit submitted";
                return Json(managerHelper, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return PartialView("EditCar", cv);
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public ActionResult SubmitPicture(CarView car, HttpPostedFileBase picture)
        {

            if (ModelState.IsValid)
            {
                var managerHelper = new CalcModel();
                var domainCar = car.toBaseCarDetails();

                try
                {

                    using (MemoryStream ms = new MemoryStream())
                    {
                        picture.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                        domainCar.Pic = array;
                        _manager.UpdateCar(domainCar);
                    }

                    TempData["message"] = "Upload Succeded!";

                    return RedirectToAction("Index");

                }

                catch
                {
                    TempData["message"] = "Upload Failed!";

                    return RedirectToAction("Index");
                }


            }

            else
            {
                return PartialView("AddCarPicture", car);
            }

        }

        [Authorize(Roles = "Manager")]
        public ActionResult SubmitNewDeal(PurchaseView pv)
        {
            if (ModelState.IsValid)
            {
                var managerHelper = new CalcModel();
                _manager.AddPurchase(pv.ReturnDateDetails());
                managerHelper.ActionResult = "New deal submitted";
                return Json(managerHelper, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return PartialView("AddDeal");
            }
        }



        [Authorize(Roles = "Manager")]
        public ActionResult SubmitEditDeal(PurchaseView pv)
        {
            if (ModelState.IsValid)
            {
                var managerHelper = new CalcModel();
                _manager.UpdatePurchase(pv.ReturnDateDetails());
                managerHelper.ActionResult = "Deal edit submitted";
                return Json(managerHelper, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return PartialView("EditDeal", pv);
            }
        }


    }
        
}