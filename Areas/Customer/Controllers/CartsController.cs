using Lokanta.Data;
using Lokanta.Models.ViewModels;
using Lokanta.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lokanta.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext db;

        public CartsController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [BindProperty]
        public OrderDetailsCartViewModel OrderDetailsCartVM { get; set; }

        public IActionResult Index()
        {
            OrderDetailsCartVM = new OrderDetailsCartViewModel()
            {
                OrderHeader = new Models.OrderHeader()
            };

            OrderDetailsCartVM.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var shoppingCarts = db.shoppingCarts.Where(m => m.ApplicationUserId == claim.Value);

            if(shoppingCarts!=null)
            {
                OrderDetailsCartVM.ShoppingCartsList = shoppingCarts.ToList();
            }

            foreach(var item in OrderDetailsCartVM.ShoppingCartsList)
            {
                item.MenuItem = db.MenuItems.FirstOrDefault(m => m.Id == item.MenuItemId);
                OrderDetailsCartVM.OrderHeader.OrderTotal += item.MenuItem.Price * item.Count;
            }

            OrderDetailsCartVM.OrderHeader.OrderTotalOrginal = OrderDetailsCartVM.OrderHeader.OrderTotal;
            if(HttpContext.Session.GetString(SD.ssCouponCode) !=null)
            {
                OrderDetailsCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = db.Coupons.Where(m => m.Name.ToLower() == OrderDetailsCartVM.OrderHeader.CouponCode.ToLower()).FirstOrDefault();
                OrderDetailsCartVM.OrderHeader.OrderTotal = SD.DiscountPrice(couponFromDb,OrderDetailsCartVM.OrderHeader.OrderTotalOrginal);
            }

            return View(OrderDetailsCartVM);
        }

        public IActionResult Summary()
        {
            OrderDetailsCartVM = new OrderDetailsCartViewModel()
            {
                OrderHeader = new Models.OrderHeader()
            };

            OrderDetailsCartVM.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var appUser = db.ApplicationUsers.Find(claim.Value);

            OrderDetailsCartVM.OrderHeader.PickUpName = appUser.Name;
            OrderDetailsCartVM.OrderHeader.PhoneNumber = appUser.PhoneNumber;
            OrderDetailsCartVM.OrderHeader.PickUpTime = DateTime.Now ;

            var shoppingCarts = db.shoppingCarts.Where(m => m.ApplicationUserId == claim.Value);

            if (shoppingCarts != null)
            {
                OrderDetailsCartVM.ShoppingCartsList = shoppingCarts.ToList();
            }

            foreach (var item in OrderDetailsCartVM.ShoppingCartsList)
            {
                item.MenuItem = db.MenuItems.FirstOrDefault(m => m.Id == item.MenuItemId);
                OrderDetailsCartVM.OrderHeader.OrderTotal += item.MenuItem.Price * item.Count;
            }

            OrderDetailsCartVM.OrderHeader.OrderTotalOrginal = OrderDetailsCartVM.OrderHeader.OrderTotal;
            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                OrderDetailsCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = db.Coupons.Where(m => m.Name.ToLower() == OrderDetailsCartVM.OrderHeader.CouponCode.ToLower()).FirstOrDefault();
                OrderDetailsCartVM.OrderHeader.OrderTotal = SD.DiscountPrice(couponFromDb, OrderDetailsCartVM.OrderHeader.OrderTotalOrginal);
            }

            return View(OrderDetailsCartVM);
        }


        public IActionResult ApplyCoupon()
        {
            if(OrderDetailsCartVM.OrderHeader.CouponCode==null)
            {
                OrderDetailsCartVM.OrderHeader.CouponCode = "";
            }
            HttpContext.Session.SetString(SD.ssCouponCode, OrderDetailsCartVM.OrderHeader.CouponCode );
            return RedirectToAction(nameof(Index));
        }


        public IActionResult RemoveCoupon()
        {
            
            HttpContext.Session.SetString(SD.ssCouponCode,String.Empty);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var shoppingCart = await db.shoppingCarts.FindAsync(cartId);

            shoppingCart.Count += 1;
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var shoppingCart = await db.shoppingCarts.FindAsync(cartId);
            if(shoppingCart.Count>1)
            {
                shoppingCart.Count -= 1;
                await db.SaveChangesAsync();
            }
            
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var shoppingCart = await db.shoppingCarts.FindAsync(cartId);
            db.shoppingCarts.Remove(shoppingCart);
                await db.SaveChangesAsync();

            var count = db.shoppingCarts.Where(m => m.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.ssCouponCode, count);
            


            return RedirectToAction(nameof(Index));
        }
    }
}
