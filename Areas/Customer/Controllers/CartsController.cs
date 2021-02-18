using Lokanta.Data;
using Lokanta.Models;
using Lokanta.Models.ViewModels;
using Lokanta.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        //bu action adi degistirdigimdan bu sey kullandim (httpget ve http post ayni isimler olmak lazim ayni degilse  bu [ActionName(".....")] kullaniyoruz)
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(string stripeToken)
        {




            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);




            OrderDetailsCartVM.ShoppingCartsList = await db.shoppingCarts.Where(m => m.ApplicationUserId == claim.Value).ToListAsync();

            OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            OrderDetailsCartVM.OrderHeader.OrderDate = DateTime.Now;
            OrderDetailsCartVM.OrderHeader.UserId = claim.Value;
            OrderDetailsCartVM.OrderHeader.Status = SD.PaymentStatusPending;
            OrderDetailsCartVM.OrderHeader.PickUpTime = Convert.ToDateTime(OrderDetailsCartVM.OrderHeader.PickUpDate.ToShortDateString() + " " +
                OrderDetailsCartVM.OrderHeader.PickUpTime.ToShortTimeString());
            OrderDetailsCartVM.OrderHeader.OrderTotalOrginal = 0;

            db.OrderHeaders.Add(OrderDetailsCartVM.OrderHeader);
            await db.SaveChangesAsync();


            foreach (var item in OrderDetailsCartVM.ShoppingCartsList)
            {
                item.MenuItem = db.MenuItems.FirstOrDefault(m => m.Id == item.MenuItemId);

                OrderDetail orderDetail = new OrderDetail()
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = OrderDetailsCartVM.OrderHeader.Id,
                    Description = item.MenuItem.Discription,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                };

                OrderDetailsCartVM.OrderHeader.OrderTotal += item.MenuItem.Price * item.Count;
                db.OrderDetails.Add(orderDetail);
            }


            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                OrderDetailsCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = db.Coupons.Where(m => m.Name.ToLower() == OrderDetailsCartVM.OrderHeader.CouponCode.ToLower()).FirstOrDefault();
                OrderDetailsCartVM.OrderHeader.OrderTotal = SD.DiscountPrice(couponFromDb, OrderDetailsCartVM.OrderHeader.OrderTotalOrginal);
            }
            else
            {
                OrderDetailsCartVM.OrderHeader.OrderTotal = OrderDetailsCartVM.OrderHeader.OrderTotalOrginal;
            }

            OrderDetailsCartVM.OrderHeader.CouponCodeDiscount = OrderDetailsCartVM.OrderHeader.OrderTotalOrginal - OrderDetailsCartVM.OrderHeader.OrderTotal;

            db.shoppingCarts.RemoveRange(OrderDetailsCartVM.ShoppingCartsList);
            HttpContext.Session.SetInt32(SD.ShoppingCartCount, 0);
            await db.SaveChangesAsync();



            //online odeme icin
            var options = new Stripe.ChargeCreateOptions
            {
                Amount = Convert.ToInt32(OrderDetailsCartVM.OrderHeader.OrderTotal * 100),
                Currency="usd",
                Description="Order ID : "+OrderDetailsCartVM.OrderHeader.Id,
                Source= stripeToken
            };
            var service = new ChargeService();
            Charge charge = service.Create(options);
            if(charge.BalanceTransactionId==null)
            {
                OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;

            }
            else
            {
                OrderDetailsCartVM.OrderHeader.TrasactionId = charge.BalanceTransactionId;
            }
            if(charge.Status.ToLower()=="succeeded")
            {
                OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                OrderDetailsCartVM.OrderHeader.Status = SD.StatusSubmitted;
            }
            else
            {
                OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }
            await db.SaveChangesAsync();


            return RedirectToAction("Index", "Home");
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
