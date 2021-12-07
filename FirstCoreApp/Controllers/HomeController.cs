using FirstCoreApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FirstCoreApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        NewsContext db;
        public HomeController(ILogger<HomeController> logger, NewsContext context)
        {
            db = context;
            _logger = logger;
        }


        public IActionResult Index()
        {
            //var result = db.Categories.ToList();
            ViewData["cats"] = db.Categories.ToList();
            var mynews= db.News.ToList();
            mynews.Reverse();
            ViewData["news"] = mynews;
            return View();
        }
        public IActionResult Messages()
        {
            var messages = db.Contacts.ToList();
            messages.Reverse();
            return View(messages);
        }
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public IActionResult saveContact(ContactUs model)
        {
            db.Contacts.Add(model);
            db.SaveChanges();
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult News(int id)
        {
            var news = db.News.Where(x => x.CategoryId == id).OrderByDescending(x=>x.Date).ToList();
            Category category = db.Categories.Find(id);
            //ViewBag.cat = category.Title;
            ViewData["cat"] = category.Title;
            //news.Reverse();
            return View(news);
        }
        public IActionResult newDetails(int id)
        {
            var news = db.News.FirstOrDefault(x => x.Id == id);
            return View(news);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
