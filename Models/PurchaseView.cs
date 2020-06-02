using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RentIt.Models;
using CarRent.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;


namespace RentIt.Models
{
    public class PurchaseView
    {
         public int ID { get; set; }

        [Required]
        public string StartDate { get; set; }


        [Required]
        public string SupposedReturn { get; set; }

      
        public string RealReturn { get; set; }

        [Required]
        
        [Display(Name = "Id of Client")]
        public int? ClientID { get; set; }
        
        [Required]
        
        [Display(Name = "Id of Car")]
        public int? CarID { get; set; }
        [Display(Name = "Car is purchased")]
        public bool? Availlability { get; set; }

        public  Purchase ReturnDateDetails ()
        {

            if (RealReturn != null)
            {
                return new Purchase
                {
                    ID = ID,

                    
                    PurchaseDate = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    ReturnDate =  DateTime.ParseExact(SupposedReturn, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    ActualReturnDate =  DateTime.ParseExact(RealReturn, "dd/MM/yyyy", CultureInfo.InvariantCulture),   
                    UserId = ClientID,
                    CarID = CarID

                };
            }

            else
            {
                return new Purchase
                {
                    ID = ID,
                    PurchaseDate = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    ReturnDate = DateTime.ParseExact(SupposedReturn, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    ActualReturnDate = null,
                    UserId = ClientID,
                    CarID = CarID

                };
            }
        }
        [Display(Name = "Price per day")]
        public decimal? Price { get; set; }
        public PurchaseView() { }


        public PurchaseView( Purchase PurchaseDetails)


        {
            this.ID = PurchaseDetails.ID;

            this.StartDate = PurchaseDetails.PurchaseDate.Value.ToShortDateString();

            this.SupposedReturn = PurchaseDetails.ReturnDate.Value.ToShortDateString();

            if(PurchaseDetails.ActualReturnDate.HasValue)
            {
                this.RealReturn = PurchaseDetails.ActualReturnDate.Value.ToShortDateString();
            }
            

            this.ClientID = PurchaseDetails.UserId;

            this.CarID = PurchaseDetails.CarID;

            this.Price = PurchaseDetails.Price;

        }
     }
    public class CalcModel
    {
        public int ID { get; set; }

        public IEnumerable<ModelView> AllCarModels { get; set; }

        public IEnumerable<CarView> AllCars { get; set; }

        public IEnumerable<UserView> AllCustomers { get; set; }

        public IEnumerable<UserView> AllEmployees { get; set; }

        public IEnumerable<PurchaseView> AllDeals { get; set; }

        public string ManagerAction { get; set; }

        public string ActionResult { get; set; }

        public int? ClientID { get; set; }

        public int carID { get; set; }

        public int modelID { get; set; }

        public int dealID { get; set; }

        public string StartDate { get; set; }

        public string SupposedReturn { get; set; }

        public string RealReturn { get; set; }

        public int totallPrice { get; set; }

        public string modelName { get; set; }


    }
 }
