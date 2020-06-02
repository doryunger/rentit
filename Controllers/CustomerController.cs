using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CarRent.BL;
using CarRent.Entities;
using RentIt.Models;

namespace RentIt.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CustomerQuery _customer;

        private static ModelView customerModel;

        private static CarView customerCar;

        private static double totallPrice;

        private static DateTime startDate;

        private static DateTime supposedReturn;

        private const string DEALS_IN_THE_BUSKET = "dealsinfo";

        public CustomerController()
        {

            _customer = new CustomerQuery();
        }
        [Authorize(Roles = "Employee, Manager, Customer")]
        [HttpPost]
        public ActionResult GetInfo(CalcModel hvm)
        {
            var date1 = hvm.StartDate.Split('-');

            var date2 = hvm.SupposedReturn.Split('-');

            startDate = new DateTime(int.Parse(date1[0]), int.Parse(date1[1]), int.Parse(date1[2]));

            supposedReturn = new DateTime(int.Parse(date2[0]), int.Parse(date2[1]), int.Parse(date2[2]));

            customerCar = new CarView(_customer.CarInfo(hvm.carID));

            customerModel = new ModelView(_customer.ModelofCar(hvm.modelID));

            totallPrice = hvm.totallPrice;

            return Json(JsonRequestBehavior.AllowGet);
        }

        //  [Authorize(Roles = "Employee, Manager, Customer")]
        public ActionResult Index()
        {
            var UserID = _customer.UserId(User.Identity.Name);

            ViewBag._Model = customerModel;

            ViewBag._Car = customerCar;

            ViewBag.price = totallPrice;

            ViewBag.userId = UserID; 

            ViewBag.startDate = startDate;

            ViewBag.supposedReturn = supposedReturn;

            var model = new PurchaseView();
           
            return View(model);
        }

        [Authorize(Roles = "Employee, Manager, Customer")]
        [HttpPost]
        public ActionResult AddtoBusket(PurchaseView deal)
        {

            TempData["Success"] = "Reservation Succeded!";
            var deals = Session[DEALS_IN_THE_BUSKET] as List<PurchaseView>;

            if (deals == null) {

                deals = new List<PurchaseView>();
            }

            deals.Add(deal);

            Session[DEALS_IN_THE_BUSKET] = deals;

            return  RedirectToAction("MyBusket");
        }



        [Authorize(Roles = "Employee, Manager, Customer")]
        public ActionResult MyBusket()
        {
            //var deals = Session[DEALS_IN_THE_BUSKET] as List<PurchaseView>;
            ManagerQuery manager = new ManagerQuery();
            if (User == null || string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Login", "Home");
            }
            var UserID = manager.GetAllUsers().Where(p => p.UserName == User.Identity.Name).Select(p => p.ID).FirstOrDefault();
            var listPur = manager.AllPurchases().Where(p => p.UserId == UserID && (!p.PurchasesStatus.HasValue || p.PurchasesStatus.Value == 0)).ToList();
            List<PurchaseView> deals = new List<PurchaseView>();
            if(listPur.Count>0)
            {
                foreach(var pur in listPur)
                {
                    var purView = new PurchaseView(pur);
                    purView.Availlability = (pur.CarID.HasValue && pur.PurchaseDate.HasValue && pur.ReturnDate.HasValue) ? CheckCarAvailability(pur.PurchaseDate.Value, pur.ReturnDate.Value, pur.CarID.Value) : false;
                    deals.Add(purView);
                }
            }
            return View(deals);
        }


        [Authorize(Roles = "Employee, Manager, Customer")]
        public ActionResult PreviousReservations()
        {
            var UserID = _customer.UserId(User.Identity.Name);
            var myDeals = _customer.PurchasesHistory(UserID).Select(d => new PurchaseView(d));
            return View(myDeals);
        }

        public ActionResult Confimation(int dealID)
        {
            //var deals = Session[DEALS_IN_THE_BUSKET] as List<PurchaseView>;

            //var currenDeal = deals.Where(d => d.ID == dealID).FirstOrDefault();

            //deals.Remove(currenDeal);

            //Session[DEALS_IN_THE_BUSKET] = deals;

            //_customer.PurcaseConfirmation(currenDeal.ReturnDateDetails());

            //TempData["Success"] = "Reservation Completed!";

            //return RedirectToAction("PreviousReservations");
            try
            {
                if (User == null || string.IsNullOrEmpty(User.Identity.Name))
                {
                    return Json(new { IsOk = 0 }, JsonRequestBehavior.AllowGet);
                }
                ManagerQuery manager = new ManagerQuery();
                var _Purchases = manager.AllPurchases().Where(p => p.ID == dealID).FirstOrDefault();
                _Purchases.PurchasesStatus = 1;
                manager.UpdatePurchase(_Purchases);
                return Json(new { IsOk = 1, Url = Url.Action("MyBusket","Customer") }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { IsOk = -1 }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RemoveItemFromBusket(int dealID)
        {
            //var deals = Session[DEALS_IN_THE_BUSKET] as List<PurchaseView>;
            //var dealtoRemove = deals.Where(d => d.ID == dealID).FirstOrDefault();

            //deals.Remove(dealtoRemove);

            //Session[DEALS_IN_THE_BUSKET] = deals;

            //TempData["Success"] = "Order Removed! ";
            ManagerQuery manager = new ManagerQuery();
            manager.DeletePurchas(dealID);
            return RedirectToAction("MyBusket");
        }

        public ActionResult ClearBusket()
        {
            Session[DEALS_IN_THE_BUSKET] = new List<PurchaseView>();
            return RedirectToAction("MyBusket");
        }

        public ActionResult QuantityOfItemsInBusket()
        {
            int count;

            var deals = Session[DEALS_IN_THE_BUSKET] as List<PurchaseView>;

            if(deals == null)
            {
                count = 0;
            }

            else
            {
                count = deals.Count;
            }


            return Json(count,JsonRequestBehavior.AllowGet);
        }

        private bool CheckCarAvailability(DateTime sDateStart, DateTime sDateEnd, int CarID)
        {
            ManagerQuery manager = new ManagerQuery();
            DateTime DateStart = new DateTime(sDateStart.Year, sDateStart.Month, sDateStart.Day);
            DateTime DateEnd = new DateTime(sDateEnd.Year, sDateEnd.Month, sDateEnd.Day);
            var listPur = manager.AllPurchases().Where(p => p.CarID.HasValue && p.CarID.Value == CarID && p.PurchasesStatus.HasValue && p.PurchasesStatus.Value == 1 && p.PurchaseDate.HasValue && p.ReturnDate.HasValue && ((p.PurchaseDate.Value <= DateStart && p.ReturnDate.Value >= DateStart) || (p.PurchaseDate.Value <= DateEnd && DateEnd <= p.ReturnDate.Value))).FirstOrDefault();
            if (listPur != null)
                return false;
            return true;
        } 
               
        public ActionResult ListPurchased()
        {
            try
            {
                if (User == null || string.IsNullOrEmpty(User.Identity.Name))
                {
                    return PartialView(new List<PurchaseView>());
                }
                ManagerQuery manager = new ManagerQuery();
                var UserID = manager.GetAllUsers().Where(p => p.UserName == User.Identity.Name).Select(p => p.ID).FirstOrDefault();
                var model = new List<PurchaseView>();
                var lstPurchased = manager.AllPurchases().Where(p => p.UserId.HasValue && p.UserId.Value == UserID && p.PurchasesStatus.HasValue && p.PurchasesStatus.Value == 1).ToList();
                if(lstPurchased.Count>0)
                {
                    foreach (var pur in lstPurchased)
                    {
                        var purView = new PurchaseView(pur);
                        purView.Availlability = (pur.CarID.HasValue && pur.PurchaseDate.HasValue && pur.ReturnDate.HasValue) ? CheckCarAvailability(pur.PurchaseDate.Value, pur.ReturnDate.Value, pur.CarID.Value) : false;
                        model.Add(purView);
                    }
                }
                return PartialView(model);
            }
            catch
            {
                return PartialView(new List<PurchaseView>());
            }
        }
	}
}