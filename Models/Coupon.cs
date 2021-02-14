using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lokanta.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public String CouponType { get; set; }
        public enum ECouponType { Percent=0, Dollar=1}
        [Required]
        public double Discount { get; set; }
        [Required]
        [Display(Name = "Minimun Amount")]
        public double MinimunAmount { get; set; }
        [Display(Name = "IS Active")]
        public bool ISAction { get; set; }
    }
}
