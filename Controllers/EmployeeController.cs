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
    public class EmployeeController : Controller
    {
        public readonly EmployeeQuery _employee;

        private static IEnumerable<PurchaseView> allDeals;

        private static IEnumerable<CarView> allCars;

        private static IEnumerable<ModelView> allModels;

        public EmployeeController()
        {

            _employee = new EmployeeQuery();

        }
        [Authorize(Roles = "Employee, Manager")]
        public ActionResult Index()
        {
            return View();
        }


        [Authorize(Roles = "Employee, Manager")]
        public JsonResult HelpAjax()
        {
            var Calcer = new CalcModel();

            allDeals = _employee.AllPurchases().Select(d => new PurchaseView(d));

            allCars = _employee.GettAllCars().Select(c => new CarView(c));

            allModels = _employee.GettAllModels().Select(m => new ModelView(m));

            Calcer.AllDeals = allDeals;

            Calcer.AllCars = allCars;

            Calcer.AllCarModels = allModels;

            return Json(Calcer, JsonRequestBehavior.AllowGet);
        }



        [Authorize(Roles = "Employee, Manager")]
        public ActionResult CloseTheDeal(CalcModel cm)
        {
            var calcer = new CalcModel();

            _employee.PurchaseConfirm(cm.dealID, cm.RealReturn);

            calcer.ActionResult = "Deal Deleted";

            return Json(calcer, JsonRequestBehavior.AllowGet);
        }
    }
}