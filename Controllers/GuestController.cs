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
    public class GuestController : Controller
    {
        private readonly ManagerQuery manager;

        private readonly GuestQuery guest;

        private static IEnumerable<ModelView> allmodels;

        private static IEnumerable<Car> domaninCars;

        private static IEnumerable<CarView> allCars;

        private static IEnumerable<PurchaseView> allDeals;

        public GuestController()
        {

            manager = new ManagerQuery();

            guest = new GuestQuery();
        }
        //
        // GET: /Guest/
        public ActionResult Index()
        {
            domaninCars = guest.CarsCondition();

            allCars = domaninCars.Select(c => new CarView(c));

            allmodels = guest.AllModels().Select(c => new ModelView(c));

            allDeals = manager.AllPurchases().Select(d => new PurchaseView(d));

            List<SelectListItem> modelsItems = new List<SelectListItem>();
            List<SelectListItem> manufacsItems = new List<SelectListItem>();
            foreach (var model in allmodels)
            {
                modelsItems.Add(new SelectListItem { Text = model.ModelName, Value = model.ID.ToString() });
            }

            ViewBag.model = modelsItems;
            ViewBag.manufac = manufacsItems;

            return View();
        }

        public ActionResult _List(int? gear, int? model, string manufa, decimal? price, DateTime? purchaseDate, DateTime? returnDate)
        {
            var domaninCars = guest.ListCar(gear, model, manufa, price, purchaseDate, returnDate);
            int UserID = 0;
            if(User != null && !string.IsNullOrEmpty(User.Identity.Name))
            {
                UserID = manager.GetAllUsers().Where(p => p.UserName == User.Identity.Name).Select(p => p.ID).FirstOrDefault();
            }
           
            var listSelect = guest.ListCarSelect(UserID).Select(p => p.CarID).ToList();
            var result = domaninCars.Where(p => !listSelect.Contains(p.CarID)).ToList();                      
            return PartialView(result);
        }

        public ActionResult GetImage(int id)
        {
            var image = guest.ShowPic(id);
            return File(image, "image/jpeg");
        }

        public ActionResult PriceCalculation(int? carid)
        {
            var car = (from c in domaninCars
                       where c.CarID == carid.Value
                       select c).FirstOrDefault();

            return View(car);
        }
        public int PurchaseDate(int CarID)
        {
            return 0;
        }
        public ActionResult _ListSelect()
        {
            int UserID = 0;
            if(User != null && !string.IsNullOrEmpty(User.Identity.Name))
            {
                UserID = manager.GetAllUsers().Where(p => p.UserName == User.Identity.Name).Select(p => p.ID).FirstOrDefault();
            }
            var domaninCars = guest.ListCarSelect(UserID);
            return PartialView(domaninCars);
        }
        public ActionResult AddNewPurchases(int CarID)
        {
            try
            {
                if (User == null || string.IsNullOrEmpty(User.Identity.Name))
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
                var UserID = manager.GetAllUsers().Where(p => p.UserName == User.Identity.Name).Select(p => p.ID).FirstOrDefault();
                Purchase pur = new Purchase();
                pur.UserId = UserID;
                pur.CarID = CarID;
                var now = DateTime.Now;
                pur.PurchaseDate = now;
                pur.ReturnDate = now;
                manager.AddPurchase(pur);
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult RemovePurchases(int CarID)
        {
            try
            {
                if (User == null || string.IsNullOrEmpty(User.Identity.Name))
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
                var UserID = manager.GetAllUsers().Where(p => p.UserName == User.Identity.Name).Select(p => p.ID).FirstOrDefault();
                Purchase pur = manager.AllPurchases().Where(p => p.CarID == CarID && p.UserId == UserID && (!p.PurchasesStatus.HasValue || p.PurchasesStatus.Value == 0)).FirstOrDefault();
                manager.DeletePurchas(pur.ID);
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult RemoveAllPurchases()
        {
            try
            {
                if (User == null || string.IsNullOrEmpty(User.Identity.Name))
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
                var UserID = manager.GetAllUsers().Where(p => p.UserName == User.Identity.Name).Select(p => p.ID).FirstOrDefault();
                var listPur = manager.AllPurchases().Where(p => p.UserId == UserID && (!p.PurchasesStatus.HasValue || p.PurchasesStatus.Value == 0)).Select(p => p.ID).ToList();
                if(listPur.Count>0)
                {
                    foreach(var purID in listPur)
                    {
                        manager.DeletePurchas(purID);
                    }
                   
                }
                
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CalculationPrice(int? NumberDate, DateTime DateStart, DateTime DateEnd)
        {
            PriceCalViewModel model = new PriceCalViewModel();
            if (User == null || string.IsNullOrEmpty(User.Identity.Name))
            {
                return PartialView(model);                
            }
            if(NumberDate.HasValue && NumberDate.Value >0 )
            {
                model.NumberDate = NumberDate.Value;
                var UserID = manager.GetAllUsers().Where(p => p.UserName == User.Identity.Name).Select(p => p.ID).FirstOrDefault();
                var domaninCars = guest.ListCarSelect(UserID);
                model.NumberCar = domaninCars.Count();
                var price = domaninCars.Sum(p => p.Model.DailyPrice);
                model.Price = (decimal)NumberDate * price;
                var listPur = manager.AllPurchases().Where(p => p.UserId == UserID && (!p.PurchasesStatus.HasValue || p.PurchasesStatus.Value == 0)).ToList();
                model.Availability = CheckCarAvailability(DateStart, DateEnd, listPur);
            }
            return PartialView(model);
        }
        
        public ActionResult MakeOrder(DateTime DateStart, DateTime DateEnd)
        {
            try
            {
                if (User == null || string.IsNullOrEmpty(User.Identity.Name))
                {
                     return Json(new { data = true, url = Url.Action("Login","Home") }, JsonRequestBehavior.AllowGet);
                }
                var UserID = manager.GetAllUsers().Where(p => p.UserName == User.Identity.Name).Select(p => p.ID).FirstOrDefault();
                var listPur = manager.AllPurchases().Where(p => p.UserId == UserID && (!p.PurchasesStatus.HasValue || p.PurchasesStatus.Value == 0)).ToList();                            
                if (listPur.Count > 0)
                {
                    foreach (var pur in listPur)
                    {
                        pur.PurchaseDate = DateStart;
                        pur.ReturnDate = DateEnd;
                        pur.PurchasesStatus = 0;
                        pur.Price = guest.PriceOfCar(pur.CarID.Value);
                        manager.UpdatePurchase(pur);
                    }

                }
                return Json(new { data = true, url = Url.Action("MyBusket","Customer")  }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { data = true, url = Url.Action("Login", "Home") }, JsonRequestBehavior.AllowGet);
            }
        }
        public bool CheckCarAvailability(DateTime DateStart, DateTime DateEnd, List<Purchase> _listPur)
        { 
            var listPurID = _listPur.Where(p => p.CarID.HasValue).Select(p => p.CarID.Value).ToList();
            var listPur = manager.AllPurchases().Where(p => p.CarID.HasValue && listPurID.Contains(p.CarID.Value) && p.PurchasesStatus.HasValue && p.PurchasesStatus.Value == 1 && p.PurchaseDate.HasValue && p.ReturnDate.HasValue && ((p.PurchaseDate.Value <= DateStart && p.ReturnDate.Value >= DateStart) || (p.PurchaseDate.Value <= DateEnd && DateEnd <= p.ReturnDate.Value))).FirstOrDefault();
            if (listPur != null)
                return false;
            return true;
        }
        public ActionResult LoadDateSelect()
        {           
            try
            {
                if (User == null || string.IsNullOrEmpty(User.Identity.Name))
                {
                    return Json(new { IsOk = 0 }, JsonRequestBehavior.AllowGet);
                }
                var UserID = manager.GetAllUsers().Where(p => p.UserName == User.Identity.Name).Select(p => p.ID).FirstOrDefault();
                var pur = manager.AllPurchases().Where(p => p.UserId == UserID && (!p.PurchasesStatus.HasValue || p.PurchasesStatus.Value == 0)).OrderByDescending(p => p.PurchaseDate).FirstOrDefault();
                var DateStart = "";
                var DateEnd = "";
                if (pur != null)
                {
                    var now = DateTime.Today;
                    DateStart = pur.PurchaseDate.HasValue ? pur.PurchaseDate.Value.ToShortDateString() : now.ToShortDateString();
                    DateEnd = pur.ReturnDate.HasValue ? pur.ReturnDate.Value.ToShortDateString() : now.ToShortDateString(); ;                            
                }
                var result = new { IsOk = 1, DateStart = DateStart, DateEnd = DateEnd }; 
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { IsOk = -1}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetNumberMyBusket()
        {
            if (User == null || string.IsNullOrEmpty(User.Identity.Name))
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            var UserID = manager.GetAllUsers().Where(p => p.UserName == User.Identity.Name).Select(p => p.ID).FirstOrDefault();
            var result = manager.AllPurchases().Where(p => p.UserId.HasValue && p.UserId.Value == UserID && (!p.PurchasesStatus.HasValue || p.PurchasesStatus.Value == 0)).Count();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
	}
}