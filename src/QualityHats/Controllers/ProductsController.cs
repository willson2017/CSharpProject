using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QualityHats.Data;
using QualityHats.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;

namespace QualityHats.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;

        public ProductsController(ApplicationDbContext context, IHostingEnvironment hEnv)
        {
            _context = context;
            _hostingEnv = hEnv;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category).Include(p => p.Supplier);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // var product = await _context.Products.SingleOrDefaultAsync(m => m.ProductID == id);
            var product = await _context.Products.Include(p => p.Category)
                                           .Include(p => p.Supplier)
                                           .AsNoTracking()
                                           .SingleOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName");
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,CategoryID,Description,ImagePath,ProductName,SupplierID,UnitPrice")] Product product
                                                , IList<IFormFile> _files)
        {
            var relativeName = "";
            var fileName = "";

            if (_files.Count < 1)
            {
                relativeName = "/images/Default.jpg";
            }
            else
            {
                foreach (var file in _files)
                {
                    fileName = ContentDispositionHeaderValue
                                      .Parse(file.ContentDisposition)
                                      .FileName
                                      .Trim('"');
                    //Path for localhost
                    relativeName = "/images/ProductImages/" + DateTime.Now.ToString("ddMMyyyy-HHmmssffffff") + fileName;

                    using (FileStream fs = System.IO.File.Create(_hostingEnv.WebRootPath + relativeName))
                    {
                        await file.CopyToAsync(fs);
                        fs.Flush();
                    }
                }
            }
            product.ImagePath = relativeName;
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " + "Try again, and if the problem persists " + "see your system administrator.");
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierName", product.SupplierID);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.SingleOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierName", product.SupplierID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,CategoryID,Description,ImagePath,ProductName,SupplierID,UnitPrice")] Product product
                                              , IList<IFormFile> _files, byte[] rowVersion)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }
            var relativeName = "";
            var fileName = "";
            var productToUpdate = await _context.Products.AsNoTracking().SingleOrDefaultAsync(s => s.ProductID == id);

            if (productToUpdate == null)
            {
                Product deleteProduct = new Product();
                await TryUpdateModelAsync(deleteProduct);
                ModelState.AddModelError(string.Empty, "Unable to save changes. The product was deleted by another user.");
                ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", deleteProduct.CategoryID);
                ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierName", deleteProduct.SupplierID);
                return View(deleteProduct);
            }


            if (_files.Count < 1)
            {
                relativeName = productToUpdate.ImagePath;
            }
            else
            {
                foreach (var file in _files)
                {
                    fileName = ContentDispositionHeaderValue
                                      .Parse(file.ContentDisposition)
                                      .FileName
                                      .Trim('"');
                    //Path for localhost
                    relativeName = "/images/ProductImages/" + DateTime.Now.ToString("ddMMyyyy-HHmmssffffff") + fileName;

                    using (FileStream fs = System.IO.File.Create(_hostingEnv.WebRootPath + relativeName))
                    {
                        await file.CopyToAsync(fs);
                        fs.Flush();
                    }
                }
            }
            productToUpdate.ImagePath = relativeName;
            _context.Entry(productToUpdate).Property("RowVersion").OriginalValue = rowVersion;

            if (await TryUpdateModelAsync<Product>(
                                productToUpdate,
                                "",
                                s => s.ProductName, s => s.UnitPrice, s => s.SupplierID, s => s.CategoryID, s => s.Description,
                                s => s.ImagePath))
            {
                try
                {
                    _context.Update(productToUpdate);
                    await _context.SaveChangesAsync();
                    ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);
                    ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierName", product.SupplierID);
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var databaseEntity = await _context.Products
                                           .AsNoTracking()
                                           .SingleAsync(d => d.ProductID == ((Product)exceptionEntry.Entity).ProductID);
                    var databaseEntry = _context.Entry(databaseEntity);                    
                    ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                   + "was modified by another user after you got the original value. The "
                   + "edit operation was canceled and the current values in the database "
                   + "have been displayed. If you still want to edit this record, click "
                   + "the Save button again. Otherwise click the Back to List hyperlink.");
                    productToUpdate.RowVersion = (byte[])databaseEntry.Property("RowVersion").CurrentValue;
                    ModelState.Remove("RowVersion");
                }

            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierName", product.SupplierID);
            return View(product);


            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(product);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!ProductExists(product.ProductID))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //    return RedirectToAction("Index");
            //}
            //ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryID);
            //ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierName", product.SupplierID);
            //return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.SingleOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("Index");
                }
                return NotFound();

            }
            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The record you attempted to delete "
                + "was modified by another user after you got the original values. "
                + "The delete operation was canceled and the current values in the "
                + "database have been displayed. If you still want to delete this "
                + "record, click the Delete button again. Otherwise "
                + "click the Back to List hyperlink.";
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.SingleOrDefaultAsync(m => m.ProductID == id);
            _context.Products.Remove(product);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                TempData["TutorialUsed"] = "The product being deleted has been used in previous orders.Delete those orders before trying again.";
                return RedirectToAction("Delete");
            }

            return RedirectToAction("Index");
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }
    }
}
