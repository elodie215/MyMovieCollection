using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyMovies.Models;
using System.IO;
using System.Web.Helpers;
using System.Web.Security;
using PagedList;

namespace MyMovies.Controllers
{
    [Authorize]
    public class MoviesController : Controller
    {
        private MoviesContext db = new MoviesContext();

        //
        // GET: /Movies/

        public ActionResult Index(int? page)
        {
            var movies = db.Movies.Include(m => m.Genre).Include(m => m.Support).Where(m => m.User.UserName.Equals(User.Identity.Name));
            movies = movies.OrderBy(m => m.Title);
            int pageSize = 10;
            int pageNumber = page ?? 1;
            return View(movies.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Movies/Details/5

        public ActionResult Details(int id = 0)
        {
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }

            if (!UserIsOwner(movie.UserId))
            {
                return RedirectToAction("Index", "Movies");
            }

            return View(movie);
        }

        //
        // GET: /Movies/Create

        public ActionResult Create()
        {
            ViewBag.GenreID = new SelectList(db.Genres, "GenreID", "Name");
            ViewBag.SupportID = new SelectList(db.Supports, "SupportID", "Name");
            return View();
        }

        //
        // POST: /Movies/Create

        [HttpPost]
        public ActionResult Create(Movie movie, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                /* upload the associated image and update the movie with its path */
                UploadCover(file, movie);
                var profile = db.UserProfiles.Where(u => u.UserName.Equals(User.Identity.Name)).First(); 
                movie.UserId = ((UserProfile)profile).UserId;
                db.Movies.Add(movie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GenreID = new SelectList(db.Genres, "GenreID", "Name", movie.GenreID);
            ViewBag.SupportID = new SelectList(db.Supports, "SupportID", "Name", movie.SupportID);
            return View(movie);
        }

        //
        // GET: /Movies/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }

            if (!UserIsOwner(movie.UserId))
            {
                return RedirectToAction("Index", "Movies");
            }

            ViewBag.GenreID = new SelectList(db.Genres, "GenreID", "Name", movie.GenreID);
            ViewBag.SupportID = new SelectList(db.Supports, "SupportID", "Name", movie.SupportID);
            return View(movie);
        }

        //
        // POST: /Movies/Edit/5

        [HttpPost]
        public ActionResult Edit(Movie movie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GenreID = new SelectList(db.Genres, "GenreID", "Name", movie.GenreID);
            ViewBag.SupportID = new SelectList(db.Supports, "SupportID", "Name", movie.SupportID);
            return View(movie);
        }

        //
        // GET: /Movies/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }

            if (!UserIsOwner(movie.UserId))
            {
                return RedirectToAction("Index", "Movies");
            }

            return View(movie);
        }

        //
        // POST: /Movies/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Movie movie = db.Movies.Find(id);
            DeleteCover(movie);
            db.Movies.Remove(movie);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        //
        // GET : /Movies/Search
        public ActionResult Search(string genre, string rating, string query)
        {
            // generate search list to filter by genre
            var GenreList = new List<string>();

            var GenreQuery = from d in db.Movies.Include(m => m.Genre)
                                .Where(m => m.User.UserName.Equals(User.Identity.Name))
                             orderby d.Genre.Name
                             select d.Genre.Name;

            GenreList.AddRange(GenreQuery.Distinct());
            ViewBag.genre = new SelectList(GenreList);

            // generate search list to filter by rating
            var RatingList = new List<string>();

            List<int> RatingQuery = (from f in db.Movies
                                     where f.User.UserName.Equals(User.Identity.Name)
                                     orderby f.Rating
                                     select f.Rating).ToList<int>();

            var sRatingQuery = new List<string>();
            RatingQuery.ForEach(rate => sRatingQuery.Add(rate.ToString()));

            RatingList.AddRange(sRatingQuery.Distinct());
            ViewBag.rating = new SelectList(RatingList);


            var movies = from m in db.Movies
                         where m.User.UserName.Equals(User.Identity.Name)
                         select m;

            if (!String.IsNullOrEmpty(query))
            {
                movies = movies.Where(s => s.Title.Contains(query));
            }

            if (!string.IsNullOrEmpty(rating))
            {
                int rate = -1;
                if (int.TryParse(rating, out rate))
                {
                    movies = movies.Where(s => s.Rating == rate);
                }
            }

            movies = movies.OrderBy(m => m.Title);

            if (string.IsNullOrEmpty(genre))
            {
                return View(movies);
            }
            else
            {
                return View(movies.Where(x => x.Genre.Name == genre));
            }
        }



        //
        // Generate thumbnail for Movie Cover
        public void GetPhotoThumbnail(string path, int width, int height)
        {
            if (path != null)
            {
                new WebImage(path)
                    .Resize(width, height, false, true)
                    .Crop(1, 1) // Cropping it to remove 1px border at top and left sides (bug in WebImage)
                    .Write();
            }
            else
            {
                // Loading a default photo for realties that don't have a Photo
                new WebImage(Server.MapPath(@"~/Images/no-cover.jpg"))
                    .Resize(width, height, false, true)
                    .Crop(1, 1)
                    .Write();
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private void UploadCover(HttpPostedFileBase file, Movie movie)
        {
            if (file != null && file.ContentLength > 0)
            {
                string path = Path.Combine(Server.MapPath("~/Images/Upload"), User.Identity.Name + "-" + Path.GetFileName(file.FileName));
                file.SaveAs(path);
                movie.Cover = path;
            }
        }

        private void DeleteCover(Movie movie)
        {
            if (movie.Cover != null && movie.Cover.Contains("no-cover.jpg")) {
                if (System.IO.File.Exists(movie.Cover)) 
                {
                    System.IO.File.Delete(movie.Cover);
                }
            }
        }

        private Boolean UserIsOwner(int id)
        {
            UserProfile profile = db.UserProfiles.Where(u => u.UserName.Equals(User.Identity.Name)).FirstOrDefault();
            if (profile != null)
            {
                return id == profile.UserId;
            }
            return false;
        }
    }
}