using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Lokanta.Models
{
    public class SubCategory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name="Sub Category Name")]
        public String Name { get; set; }
        [Required]
        [Display(Name = "Category ")]
        //بيمثل خاصية الفورن كي من اجل ربطها مع جدول الفئات
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
