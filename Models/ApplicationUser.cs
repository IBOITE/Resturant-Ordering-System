using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lokanta.Models
{
    public class ApplicationUser:IdentityUser   //Identity'te sql'den gelen aspNetuseru table Icinde bulunyor bu nedenle kalitim yaptim 
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string StreetAdress { get; set; }
        public string State { get; set; }
    }
}
