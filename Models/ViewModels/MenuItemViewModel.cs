using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lokanta.Models.ViewModels
{
    public class MenuItemViewModel
    {
        public MenuItem MenuItem { get; set; }
        public IEnumerable<Category> CategoriesList { get; set; }
        public IEnumerable<SubCategory> SubCategoriesList { get; set; }
    }
}
