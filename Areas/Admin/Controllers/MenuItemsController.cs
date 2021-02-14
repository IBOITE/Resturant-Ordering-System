using Lokanta.Data;
using Lokanta.Models;
using Lokanta.Models.ViewModels;
using Lokanta.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lokanta.Areas.Admin.Controllers
{
    //لمين صلاحية هاد الكونترولر
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class MenuItemsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }
        public MenuItemsController(ApplicationDbContext db,IWebHostEnvironment webHostEnvironment)
        {
            this.db = db;
            this.webHostEnvironment = webHostEnvironment;
            MenuItemVM = new MenuItemViewModel()
            {
                MenuItem = new MenuItem(),
                CategoriesList = db.Categories.ToList()
            };
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var menuItems = await db.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync();
            return View(menuItems);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreatPost()
        {
            if(ModelState.IsValid)
            {
                db.MenuItems.Add(MenuItemVM.MenuItem);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(MenuItemVM);
        }



        [HttpGet]
        public async Task< IActionResult> Edit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }
            var menuItem = db.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefault(m => m.Id == id);
            if(menuItem==null)
            {
                return NotFound();
            }
            MenuItemVM.MenuItem = menuItem;
            MenuItemVM.SubCategoriesList = await db.SubCategories.Where(m => m.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
            return View(MenuItemVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public async Task<IActionResult> EditPost()
        {
            if (ModelState.IsValid)
            {
                db.MenuItems.Update(MenuItemVM.MenuItem);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(MenuItemVM);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var menuItem = db.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefault(m => m.Id == id);
            if (menuItem == null)
            {
                return NotFound();
            }
            MenuItemVM.MenuItem = menuItem;
            MenuItemVM.SubCategoriesList = await db.SubCategories.Where(m => m.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
            return View(MenuItemVM);
        }






        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var menuItem = db.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefault(m => m.Id == id);
            if (menuItem == null)
            {
                return NotFound();
            }
            MenuItemVM.MenuItem = menuItem;
            MenuItemVM.SubCategoriesList = await db.SubCategories.Where(m => m.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
            return View(MenuItemVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost()
        {
            
                db.MenuItems.Remove(MenuItemVM.MenuItem);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            
        }



    }
}
