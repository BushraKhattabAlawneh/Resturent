using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Resturent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resturent.Controllers
{
    public class AdminController : Controller
    {
        private readonly ModelContext _context;
        public AdminController(ModelContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var customers = _context.Customers.ToList().Take(5);
            var products=_context.Products.Include(p=>p.Category).ToList().Take(5);

            var model = Tuple.Create<IEnumerable<Customer>, IEnumerable<Product>>(customers, products);

            ViewBag.count = _context.Customers.Count();
            ViewData["Sales"] = _context.Products.Sum(x => x.Sale);
            return View(model);
        }
        public IActionResult Join()
        {
            var customer = _context.Customers.ToList();
            var products = _context.Products.ToList();
            var category = _context.Categories.ToList();
            var productCustomer = _context.ProductCustomers.ToList();

            var model = from c in customer
                        join pc in productCustomer on c.Id equals pc.CustomerId
                        join p in products on pc.ProductId equals p.Id
                        join cat in category on p.CategoryId equals cat.Id
                        select new JoinTable { product = p, category = cat, customer = c, productCustomer = pc };

            return View(model);
                        
        }

        public IActionResult Search()
        {
            var pc = _context.ProductCustomers.Include(p=>p.Product).Include(p=>p.Customer).ToList();
            return View(pc);
        }

        [HttpPost]
        public async Task<IActionResult> Search(DateTime? startDate, DateTime? endDate)
        {
            var pc = _context.ProductCustomers.Include(p => p.Product).Include(p => p.Customer);

            if(startDate==null && endDate == null)
            {
                return View(await pc.ToListAsync());
            }
            else if (startDate==null && endDate != null)
            {
                var result = await pc.Where(p => p.DateFrom <= endDate).ToListAsync();
                return View(result);
            }
            else if(startDate!=null && endDate == null)
            {
                var result1 = await pc.Where(p => p.DateFrom >= startDate).ToListAsync();
                return View(result1);
            }
            else
            {
                var result3= await pc.Where(p=>p.DateFrom >=startDate && p.DateFrom <= endDate).ToListAsync();
                return View(result3);
            }
        }

        public IActionResult Report()
        {

            var customer = _context.Customers.ToList();
            var products = _context.Products.ToList();
            var category = _context.Categories.ToList();
            var productCustomer = _context.ProductCustomers.ToList();

            var model = from c in customer
                        join pc in productCustomer on c.Id equals pc.CustomerId
                        join p in products on pc.ProductId equals p.Id
                        join cat in category on p.CategoryId equals cat.Id
                        select new JoinTable { product = p, category = cat, customer = c, productCustomer = pc };
            ViewBag.Quantity = _context.ProductCustomers.Sum(x => x.Quantity);
            ViewBag.sales = _context.ProductCustomers.Sum(x => x.Product.Price * x.Quantity);
            return View(model);
        }

        [HttpPost]
        public IActionResult Report(DateTime? startDate, DateTime? endDate)
        {
            var customer = _context.Customers.ToList();
            var products = _context.Products.ToList();
            var category = _context.Categories.ToList();
            var productCustomer = _context.ProductCustomers.ToList();

            var model = from c in customer
                        join pc in productCustomer on c.Id equals pc.CustomerId
                        join p in products on pc.ProductId equals p.Id
                        join cat in category on p.CategoryId equals cat.Id
                        select new JoinTable { product = p, category = cat, customer = c, productCustomer = pc };
            if (startDate == null && endDate == null)
            {
                ViewBag.Quantity = _context.ProductCustomers.Sum(x => x.Quantity);
                ViewBag.sales = _context.ProductCustomers.Sum(x => x.Product.Price * x.Quantity);
                return View(model.ToList());
            }
            else if (startDate == null && endDate != null)
            {
                var result =model.Where(p => p.productCustomer.DateFrom.Value.Date <= endDate).ToList();
                ViewBag.Quantity = result.Sum(x => x.productCustomer.Quantity);
                ViewBag.sales = result.Sum(x => x.product.Price * x.productCustomer.Quantity);

                return View(result);
            }
            else if (startDate != null && endDate == null)
            {
                var result1 =model.Where(p => p.productCustomer.DateFrom.Value.Date >= startDate).ToList();
                ViewBag.Quantity = result1.Sum(x => x.productCustomer.Quantity);
                ViewBag.sales = result1.Sum(x => x.product.Price * x.productCustomer.Quantity);

                return View(result1);
            }
            else
            {
                var result3 = model.Where(p => p.productCustomer.DateFrom.Value.Date >= startDate && p.productCustomer.DateFrom.Value.Date <= endDate).ToList();
                ViewBag.Quantity = result3.Sum(x => x.productCustomer.Quantity);
                ViewBag.sales = result3.Sum(x => x.product.Price * x.productCustomer.Quantity);

                return View(result3);
            }
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
