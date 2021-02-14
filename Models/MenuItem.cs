using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Lokanta.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public String Name { get; set; }
        public String Discription { get; set; }
        public double Price { get; set; }
        public String Image { get; set; }
        public String Spicyness { get; set; }
        public enum ESpicy { Na=0,NotSpicy=1,Spicy=2,VerySpicy=3 }
        //bu 4 satir foreignKey olusturmak icin(categoru) 
        [Display(Name ="Category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        //bu 4 satir foreignKey olusturmak icin (subcategory)
        [Display(Name ="Sub Category")]
        public int SubCategoryId { get; set; }
        [ForeignKey("SubCategoryId")]
        public SubCategory SubCategory { get; set; }
    }
}
