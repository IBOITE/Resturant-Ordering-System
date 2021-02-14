using Lokanta.Data;
using Lokanta.Models;
using Lokanta.Utility;
using Microsoft.AspNetCore.Authorization;
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
    public class CouponsController : Controller
    {
        private readonly ApplicationDbContext db;
        public CouponsController(ApplicationDbContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public async Task< IActionResult> Index()
        {
            var coupons = await db.Coupons.ToListAsync();
            return View(coupons);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupun)
        {
            if(ModelState.IsValid)
            {
                db.Coupons.Add(coupun);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(coupun);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }
            var coupun = await db.Coupons.FindAsync(id);
            if(coupun==null)
            {
                return NotFound();
            }

            return View(coupun);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< IActionResult> Edit(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                db.Coupons.Update(coupon);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(coupon);
        }



        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coupun = await db.Coupons.FindAsync(id);
            if (coupun == null)
            {
                return NotFound();
            }

            return View(coupun);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Coupon coupon)
        {
            
                db.Coupons.Remove(coupon);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            
        }


        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coupun = await db.Coupons.FindAsync(id);
            if (coupun == null)
            {
                return NotFound();
            }

            return View(coupun);

        }
    }

}
