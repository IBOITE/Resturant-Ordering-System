using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Lokanta.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }
        [Required]
        [ForeignKey("OrderId")]
        public virtual MenuItem MenuItem { get; set; }

        public int Count { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public double Price { get; set; }
    }
}
