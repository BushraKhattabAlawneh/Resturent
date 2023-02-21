using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Resturent.Models;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Resturent.Controllers
{
    public class AuthentecationController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AuthentecationController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id,Fname,Lname,ImagePath,ImageFile")] Customer customer, string pass , string username )
        {
            if (ModelState.IsValid)
            {
                if (customer.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;

                    string fileName = Guid.NewGuid().ToString() + "_" + customer.ImageFile.FileName;

                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await customer.ImageFile.CopyToAsync(fileStream);
                    }
                    customer.ImagePath = fileName;

                }
                _context.Add(customer);
                await _context.SaveChangesAsync();

                UserLogin userLogin = new UserLogin();
                userLogin.UserName = username;
                userLogin.Passwordd = pass;
                userLogin.RoleId = 2;
                userLogin.CustomerId = customer.Id;

                _context.Add(userLogin);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index","Home");
            }
            return View(customer);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Id,UserName,Passwordd,RoleId,CustomerId")] UserLogin userLogin)
        {
            var auth = _context.UserLogins.Where(x => x.UserName == userLogin.UserName &&
            x.Passwordd == userLogin.Passwordd).FirstOrDefault();

            ViewBag.Login = 0;
            if (auth != null)
            {
               
                var customer = _context.Customers.Where(x => x.Id == auth.CustomerId).FirstOrDefault();

                switch (auth.RoleId)
                {
                    case 1: //Admin
                        HttpContext.Session.SetInt32("id",(int)customer.Id);
                        HttpContext.Session.SetString("username",customer.Fname); //username=Amal;
                        HttpContext.Session.SetInt32("Login", 1);
                        HttpContext.Session.SetInt32("CusId", (int)customer.Id);
                        return RedirectToAction("Index", "Categories");
                    case 2: //Customer
                        HttpContext.Session.SetInt32("id", (int)customer.Id);
                        HttpContext.Session.SetString("username", customer.Fname); //username=Amal;
                        HttpContext.Session.SetInt32("Login", 1);
                        return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.flag = 0;
            }
            return View();
        }
    }
}
