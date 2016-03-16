using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MusicProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MusicProject.Controllers
{
    
    public class AlbumsController : Controller
    {
        private MusicContext db = new MusicContext();
        ApplicationDbContext context;

        public AlbumsController()
        {
            context = new ApplicationDbContext();
        }
        //check if the login user is an admin
        public Boolean isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Admin")
                {
                    return true;
                }
            }
            return false;
        }

        //do a quick search for album
        public ActionResult QuickSearch(string term)
        {
            var albums = GetAlbums(term).Select(a => new { value = a.Title });
            return Json(albums, JsonRequestBehavior.AllowGet);
        }

        //get the Album in db
        private List<Album> GetAlbums(string searchString)
        {
            return db.Albums
                .Where(a => a.Title.Contains(searchString))
                .ToList();
        }
        // GET: Albums
        public ActionResult Index(string name, string currentFilter, string searchString)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ViewBag.Name = user.Name;

                ViewBag.displayMenu = "Member";

                if (isAdminUser())
                {
                    ViewBag.displayMenu = "Admin";
                }
            }

            //Set up sorting cases 
            ViewBag.TitleSorting = String.IsNullOrEmpty(name) ? "Title_DESC" : "";
            ViewBag.ArtistSorting = name == "Artist" ? "Artist_DESC" : "Artist";
            ViewBag.GenreSorting = name == "Genres" ? "Genres_DESC" : "Genres";
            ViewBag.ProducerSorting = name == "Producer" ? "Producer_DESC" : "Producer";
            ViewBag.CompanySorting = name == "Company" ? "Company_DESC" : "Company";
            ViewBag.DateSorting = name == "Date" ? "Date_DESC" : "Date";
            ViewBag.CreateSorting = name == "Create" ? "Create_DESC" : "Create";

            if (searchString == null)
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var album = from s in db.Albums select s;

            //Searching the song by song title
            if (!String.IsNullOrEmpty(searchString))
            {
                album = album.Where(s => s.Title.Contains(searchString));
            }

            //Sorting by title, genre, or artist first name
            switch (name)
            {
                case "Title_DESC":
                    album = album.OrderByDescending(s => s.Title);
                    break;
                case "Artist":
                    album = album.OrderBy(s => s.Artists.Fname);
                    break;
                case "Genres":
                    album = album.OrderBy(s => s.Genres);
                    break;
                case "Genres_DESC":
                    album = album.OrderByDescending(s => s.Genres);
                    break;
                case "Artist_DESC":
                    album = album.OrderByDescending(s => s.Artists.Fname);
                    break;
                case "Producer":
                    album = album.OrderBy(s => s.Producers);
                    break;
                case "Producer_DESC":
                    album = album.OrderByDescending(s => s.Producers);
                    break;
                case "Company":
                    album = album.OrderBy(s => s.Companies.Name);
                    break;
                case "Company_DESC":
                    album = album.OrderByDescending(s => s.Companies.Name);
                    break;
                case "Date":
                    album = album.OrderBy(s => s.Release);
                    break;
                case "Date_DESC":
                    album = album.OrderByDescending(s => s.Release);
                    break;
                case "Create":
                    album = album.OrderBy(s => s.CreateOrUpdate);
                    break;
                case "Create_DESC":
                    album = album.OrderByDescending(s => s.CreateOrUpdate);
                    break;
                default:
                    album = album.OrderBy(s => s.Title);
                    break;
            }

            return View(album.ToList());
        }

        // GET: Albums/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.displayMenu = "Member";
            }

            Album album = db.Albums.Find(id);

            if (album == null)
            {
                return HttpNotFound();
            }

            getImage(album);

            // Retrieve Album and its Songs from database
            var albumModel = db.Albums.Include(s => s.Songs)
                .Single(g => g.AlbumID == id);

            return View(album);
        }

        //Convert the image data in the database into image to display on the page
        public void getImage(Album album)
        {
            if (album.Image != null)
            {
                byte[] imageByteData = album.Image;
                string imageBase64Data = Convert.ToBase64String(imageByteData);
                string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                ViewBag.ImageData = imageDataURL;
            }
        }

        // GET: Albums/Create
        public ActionResult Create()
        {
            ViewBag.ArtistID = new SelectList(db.Artists, "ArtistID", "FullName");
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name");
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AlbumID,Title,ArtistID,Genres,Producers,CompanyID,Release")] Album album, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                    {
                        album.Image = reader.ReadBytes(upload.ContentLength);
                        album.ImageName = upload.FileName;
                    }
                }
                album.CreateOrUpdate = System.DateTime.Now;

                db.Albums.Add(album);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ArtistID = new SelectList(db.Artists, "ArtistID", "FullName", album.ArtistID);
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name", album.CompanyID);
            return View(album);
        }

        // GET: Albums/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);

            if (album == null)
            {
                return HttpNotFound();
            }

            getImage(album);

            ViewBag.ArtistID = new SelectList(db.Artists, "ArtistID", "FullName", album.ArtistID);
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name", album.CompanyID);
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, HttpPostedFileBase upload)
        {
            Album album = db.Albums.FirstOrDefault(i => i.AlbumID == id);

            if (upload != null && upload.ContentLength > 0)
            {
                using (var reader = new System.IO.BinaryReader(upload.InputStream))
                {
                    album.Image = reader.ReadBytes(upload.ContentLength);
                    album.ImageName = upload.FileName;
                }
            }
            album.CreateOrUpdate = System.DateTime.Now;

            UpdateModel(album);
            db.SaveChanges();

            ViewBag.ArtistID = new SelectList(db.Artists, "ArtistID", "FullName", album.ArtistID);
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name", album.CompanyID);

            return RedirectToAction("Details", new { id = album.AlbumID });
        }

        // GET: Albums/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);

            if (album == null)
            {
                return HttpNotFound();
            }

            getImage(album);

            return View(album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //set all albumID of songs in the deleted album to NULL
            var songs = from s in db.Songs select s;
            songs = songs.Where(s => s.AlbumID == id);

            foreach (var s in songs)
            {
                s.AlbumID = null;
            }

            //remove the album out of database
            Album album = db.Albums.Find(id);
            db.Albums.Remove(album);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
