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
    public class ModelView
    {
         
        public int ID { get; set; }

        [Required]
        [Display(Name ="Manufacturer's Name")]
        
        public string Manufacturer  { get; set; }

        [Required]
        [Display(Name = "Name of Model")]
        public string ModelName { get; set; }

        [Required]
        [Display(Name = "Daily Price")]
        public decimal DailyPrice { get; set; }

        [Required]
       [Display(Name = "Late Return Fine")]
        public decimal DelayReturnFee { get; set; }

        [Required]
        [Display(Name = "Transmission")]
        [EnumDataType(typeof(Gear), ErrorMessage = "Please enter a valid Gear")]
        public Gear? gear { get; set; }



        public  CarModel toBaseModelDetail()
        {
            return new CarModel
            {
                ModelID = ID,
                Manufacturer = Manufacturer,
                ModelName = ModelName,
                DailyPrice = DailyPrice,
                DelayReturnFee = DelayReturnFee,
                gear = gear.Value
            };
        }






        public ModelView() { }


        public ModelView(CarModel ModelDetails)

        {
            this.ID = ModelDetails.ModelID;
            this.Manufacturer = ModelDetails.Manufacturer;           
            this.ModelName = ModelDetails.ModelName;              
            this.DailyPrice = ModelDetails.DailyPrice;
            this.DelayReturnFee = ModelDetails.DelayReturnFee;  
            this.gear = ModelDetails.gear;
        }

    }

 }
