using Lokanta.Data;
using Lokanta.Models;
using Lokanta.Models.ViewModels;
using Lokanta.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lokanta.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db;
        public HomeController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task <IActionResult> Index()
        {
            //shopping conut eklemek icin
            //مشان يجيب الاي دي تبع المستخدم 2 سطر
            var calimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = calimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                List<ShoppingCart> shoppingCartsList = await db.shoppingCarts.Where(m => m.ApplicationUserId == claim.Value).ToListAsync();
                HttpContext.Session.SetInt32(SD.ShoppingCartCount, shoppingCartsList.Count);
            }

            IndexViewModel IndexVM = new IndexViewModel()
            {
                Categories = await db.Categories.ToListAsync(),
                Coupons=await db.Coupons.Where(m=>m.ISAction==true).ToListAsync(),
                MenuItems= await db.MenuItems.Include(m=>m.Category).Include(m=>m.SubCategory).ToListAsync()
            };
            return View(IndexVM);
        }

        [HttpGet]
        //مشان لما اضغط عليها بتاخدني لشاشة الدخول
        [Authorize]

        public async Task<IActionResult> Details(int itemid)
        {
            var menuItem = await db.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == itemid).FirstOrDefaultAsync();
            ShoppingCart shoppingCart = new ShoppingCart()
            {
                MenuItem = menuItem,
                MenuItemId = menuItem.Id
            };
            return View(shoppingCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            if(ModelState.IsValid)
            {
                //عطينا قيمة صفر لانو عم ياخد الاي دي تبع المنتج و عم يحطو لالو لهيك بيعطي غلط
                //shoppingCart.Id = 0;   birinci cozum ,ikincin cozum index icinde details botununda id degistiriyorum itemid olacak bu su anda kullanyorum 

                //مشان يجيب الاي دي تبع المستخدم 2 سطر
                var calimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = calimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                //عم يخزن الاي دي تبع المستخدم
                shoppingCart.ApplicationUserId = claim.Value;
                //عم يخزن قيمة الطلب ب اي دي تبع المنتج و اي دي تبع المستخدم
                var shoppingCartFromDB = await db.shoppingCarts.Where(m => m.ApplicationUserId == shoppingCart.ApplicationUserId && m.MenuItemId == shoppingCart.MenuItemId).FirstOrDefaultAsync();
                //اذا الطلب مو مطلوب من قبل ضيفو متل مل هو
                if(shoppingCartFromDB==null)
                {
                    db.shoppingCarts.Add(shoppingCart);
                }
                //اذا مطلوب جمعو مع الماضي
                else
                {
                    shoppingCartFromDB.Count += shoppingCart.Count;
                }
                await db.SaveChangesAsync();
                //عدد طلبات المستخدم
                var count = db.shoppingCarts.Where(m => m.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32(SD.ShoppingCartCount,count);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var menuItem = await db.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == shoppingCart.MenuItemId).FirstOrDefaultAsync();
                ShoppingCart shoppingCartObj = new ShoppingCart()
                {
                    MenuItem = menuItem,
                    MenuItemId = menuItem.Id
                };
                return View(shoppingCartObj);
            }
        }
        

    }
}
