using Lokanta.Data;
using Lokanta.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lokanta.Areas.Admin.Controllers
{
    //لمين صلاحية هاد الكونترولر
    [Authorize(Roles =SD.ManagerUser)]
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;


        public UsersController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task < IActionResult> Index()
        {
            //bu uc satir simdi user bulan
            var claimsIdentity = (ClaimsIdentity )User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            String UserId = claim.Value;
            //return icinde tum sonuclar bana goster Userid hairic
            return View(await db.ApplicationUsers.Where(m=>m.Id!=UserId).ToListAsync());
        }




        public async Task<IActionResult> LockUnLock(string? id)
        {
            if(id==null)
            {
                return NotFound();
            }
            var user = await db.ApplicationUsers.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if(user.LockoutEnd==null||user.LockoutEnd<DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            else
            {
                user.LockoutEnd = DateTime.Now;
            }
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
