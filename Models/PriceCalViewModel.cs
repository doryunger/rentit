using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RentIt.Models
{
    public class PriceCalViewModel
    {
        public int NumberCar { get; set; }
        public int NumberDate { get; set; }
        public decimal Price { get; set; }
        public bool Availability { get; set; }       
    }
}