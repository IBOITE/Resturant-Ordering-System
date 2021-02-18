using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Lokanta.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public String UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public double OrderTotalOrginal { get; set; }

        [Required]
        [DisplayFormat(DataFormatString ="{0,c}")]
        public double OrderTotal { get; set; }

        [Required]
        [Display(Name = "Pickup Time")]
        public DateTime PickUpTime { get; set; }
        
        [Required]
        [NotMapped]
        public DateTime PickUpDate { get; set; }

        [Display(Name = "Coupon Code")]
        public String CouponCode { get; set; }
        public double CouponCodeDiscount { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public String Comments { get; set; }
        [Display(Name = "Pickup Name")]
        public String PickUpName { get; set; }
        [Display(Name = "Phone Number")]
        public String PhoneNumber { get; set; }
        public String TrasactionId { get; set; }


    }
}
