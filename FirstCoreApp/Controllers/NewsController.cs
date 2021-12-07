using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FirstCoreApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace FirstCoreApp.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class NewsController : Controller
    {
        private readonly NewsContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public NewsController(NewsContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _webHostEnvironment = hostEnvironment;

        }

        // GET: News
        public async Task<IActionResult> Index()
        {
            var newsContext = _context.News.Include(n => n.Category);
            return View(await newsContext.ToListAsync());
        }

        // GET: News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Include(n => n.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: News/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Title");
            return View();
        }

        // POST: News/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(model);
                News news = new News
                {
                    Title=model.Title,
                    Date=model.Date,
                    Image=uniqueFileName,
                    Topic=model.Topic,
                    CategoryId=model.CategoryId

                };

                _context.Add(news);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", model.CategoryId);
            return View();
        }
        private string UploadedFile(NewsViewModel model)
        {
            string uniqueFileName = null;

            if (model.Image != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "assets/img/blog/");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                }
            }
            else
            {
                uniqueFileName = "noimage.jpg";
            }
            return uniqueFileName;
        }

        // GET: News/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var news = await _context.News.FindAsync(id);
            News news = await _context.News.Where(x => x.Id == id).FirstOrDefaultAsync();
            NewsViewModel newsViewModel = new NewsViewModel
            {
                Title = news.Title,
                Topic = news.Topic,
                Date = news.Date,
                CategoryId = news.CategoryId
            };
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Title", news.CategoryId);
            return View(newsViewModel);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,NewsViewModel newsmodel, IFormFile file)
        {
            

            if (ModelState.IsValid)
            {
                if (id != newsmodel.Id)
                {
                    return NotFound();
                }
                News news = await _context.News.Where(x => x.Id == id).FirstOrDefaultAsync();
                news.Title = newsmodel.Title;
                news.Topic = newsmodel.Topic;
                news.Date = newsmodel.Date;
                news.CategoryId = newsmodel.CategoryId;
                if (newsmodel.Image != null)
                {
                    if (newsmodel.Image != null)
                    {
                        string filepath = Path.Combine(_webHostEnvironment.WebRootPath, "assets/img/blog/", newsmodel.Image.ToString());
                        System.IO.File.Delete(filepath);
                    }
                    news.Image = UploadedFile(newsmodel);
                }
                    _context.Update(news);
                    await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", newsmodel.CategoryId);
            return View();
        }

        // GET: News/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Include(n => n.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var news = await _context.News.FindAsync(id);
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.Id == id);
        }
    }
}
