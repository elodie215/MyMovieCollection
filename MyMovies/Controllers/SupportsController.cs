using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyMovies.Models;

namespace MyMovies.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SupportsController : Controller
    {
        private MoviesContext db = new MoviesContext();

        //
        // GET: /Supports/

        public ActionResult Index()
        {
            return View(db.Supports.ToList());
        }

        //
        // GET: /Supports/Details/5

        public ActionResult Details(int id = 0)
        {
            Support support = db.Supports.Find(id);
            if (support == null)
            {
                return HttpNotFound();
            }
            return View(support);
        }

        //
        // GET: /Supports/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Supports/Create

        [HttpPost]
        public ActionResult Create(Support support)
        {
            if (ModelState.IsValid)
            {
                db.Supports.Add(support);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(support);
        }

        //
        // GET: /Supports/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Support support = db.Supports.Find(id);
            if (support == null)
            {
                return HttpNotFound();
            }
            return View(support);
        }

        //
        // POST: /Supports/Edit/5

        [HttpPost]
        public ActionResult Edit(Support support)
        {
            if (ModelState.IsValid)
            {
                db.Entry(support).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(support);
        }

        //
        // GET: /Supports/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Support support = db.Supports.Find(id);
            if (support == null)
            {
                return HttpNotFound();
            }
            return View(support);
        }

        //
        // POST: /Supports/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Support support = db.Supports.Find(id);
            db.Supports.Remove(support);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}