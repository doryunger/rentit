using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Spatial;
using CarRent.Entities;
using System.ComponentModel.DataAnnotations;

namespace RentIt.Models
{
    public class SnifView
    {
         public int ID { get; set; }

       [Display(Name = "Branch Name")]
        [Required]
        public string BranchName { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string District { get; set; }

        public DbGeography Location { get; set; }



        public SnifView() { }
        public  Snif toBaseSnifDetails ()
        {
           
            return new Snif
            {
                SnifID = ID,
                SnifhName = BranchName,
                City = City,
                Location= Location
                
            };
        }
         public SnifView(Snif domainSnif)
        {
            this.ID = domainSnif.SnifID;

            this.BranchName = domainSnif.SnifhName;

            this.City = domainSnif.City;

        }

    }
}
