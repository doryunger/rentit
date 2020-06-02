using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RentIt.Models;
using CarRent.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace RentIt.Models
{
    public class CarView
    {
            [Required]
            public int ID { get; set; }

            [Required]
            public double? KM { get; set; }

            [Required]
            [Display(Name = "Car Number")]
            public string CarNumber { get; set; }

            [Required]
            [Display(Name = "Id of Branch")]
            public int SnifID { get; set; }

            [Required]
            [Display(Name = "Id of Model")]
            public int ModelID { get; set; }

            [Required]
            [Display(Name = "Car is in proper state")]
            public bool CarAvilablity { get; set; }

            [Column(TypeName = "Picture")]
            public HttpPostedFileBase Picture { get; set; }

            public CarView() { }


            public Car toBaseCarDetails()
            {
                return new Car
                {
                    CarID = ID,
                    KM = KM,
                    CarNumber = CarNumber,
                    SnifID = SnifID,
                    ModelID = ModelID,
                    CarAvilablity = CarAvilablity
                };
            }

            public CarView(Car CarDetails)
            {
                this.ID = CarDetails.CarID;
                this.KM = CarDetails.KM;
                this.CarNumber = CarDetails.CarNumber;
                this.SnifID = CarDetails.SnifID;
                this.ModelID = CarDetails.ModelID;
                this.CarAvilablity = CarDetails.CarAvilablity;
         }
    }
}