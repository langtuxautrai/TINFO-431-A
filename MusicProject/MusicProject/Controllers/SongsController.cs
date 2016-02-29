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
    public class SongsController : Controller
    {
        private MusicContext db = new MusicContext();
        ApplicationDbContext context;

        public SongsController()
        {
            context = new ApplicationDbContext();
        }

        //check if the user is adminitrator or not
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
                else
                {
                    return false;
                }
            }
            return false;
        }

        // GET: Songs
        public ActionResult Index(String name, string searchString)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ViewBag.Name = user.Name;
                
                ViewBag.displayMenu = "No";

                if (isAdminUser())
                {
                    ViewBag.displayMenu = "Yes";
                }
            }
            
            var songs = from s in db.Songs select s;
            
            //Searching the song by song title
            if (!String.IsNullOrEmpty(searchString))
            {
                songs = songs.Where(s => s.Title.Contains(searchString));                
            }           

            //Set up sorting cases 
            ViewBag.TitleSorting = String.IsNullOrEmpty(name) ? "Title_DESC" : "";
            ViewBag.GenreSorting = name == "Genres" ? "Genres_DESC" : "Genres";
            ViewBag.ArtistSorting = name == "Artist" ? "Artist_DESC" : "Artist";
            ViewBag.ComposerSorting = name == "Composer" ? "Composer_DESC" : "Composer";
            ViewBag.AlbumSorting = name == "Album" ? "Album_DESC" : "Album";
            ViewBag.DateSorting = name == "Date" ? "Date_DESC" : "Date";

            //Sorting by title, genre, or artist first name
            switch (name)
            {
                case "Title_DESC":
                    songs = songs.OrderByDescending(s => s.Title);
                    break;
                case "Genres":
                    songs = songs.OrderBy(s => s.Genres);
                    break;
                case "Genres_DESC":
                    songs = songs.OrderByDescending(s => s.Genres);
                    break;
                case "Artist":
                    songs = songs.OrderBy(s => s.Artists.Fname);
                    break;
                case "Artist_DESC":
                    songs = songs.OrderByDescending(s => s.Artists.Fname);
                    break;
                case "Composer":
                    songs = songs.OrderBy(s => s.Composers.Fname);
                    break;
                case "Composer_DESC":
                    songs = songs.OrderByDescending(s => s.Composers.Fname);
                    break;
                case "Album":
                    songs = songs.OrderBy(s => s.Albums.Title);
                    break;
                case "Album_DESC":
                    songs = songs.OrderByDescending(s => s.Albums.Title);
                    break;
                case "Date":
                    songs = songs.OrderBy(s => s.Release);
                    break;
                case "Date_DESC":
                    songs = songs.OrderByDescending(s => s.Release);
                    break;
                default:
                    songs = songs.OrderBy(s => s.Title);
                    break;
            }

            return View(songs.ToList());
        }

        // GET: Songs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(song);
        }

        // GET: Songs/Create
        public ActionResult Create()
        {
            ViewBag.AlbumID = new SelectList(db.Albums, "AlbumID", "Title");
            ViewBag.ArtistID = new SelectList(db.Artists, "ArtistID", "Fname");
            ViewBag.ComposerID = new SelectList(db.Composers, "ComposerID", "Fname");
            return View();
        }

        // POST: Songs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SongID,Title,Genres,ArtistID,AlbumID,ComposerID,Release,Peak_position,Lyric")] Song song)
        {
            if (ModelState.IsValid)
            {
                
                db.Songs.Add(song);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AlbumID = new SelectList(db.Albums, "AlbumID", "Title", song.AlbumID);
            ViewBag.ArtistID = new SelectList(db.Artists, "ArtistID", "Fname", song.ArtistID);
            ViewBag.ComposerID = new SelectList(db.Composers, "ComposerID", "Fname", song.ComposerID);
            return View(song);
        }

        // GET: Songs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            ViewBag.AlbumID = new SelectList(db.Albums, "AlbumID", "Title", song.AlbumID);
            ViewBag.ArtistID = new SelectList(db.Artists, "ArtistID", "Fname", song.ArtistID);
            ViewBag.ComposerID = new SelectList(db.Composers, "ComposerID", "Fname", song.ComposerID);
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SongID,Title,Genres,ArtistID,ComposerID,AlbumID,Release,Peak_position,Lyric")] Song song)
        {
            if (ModelState.IsValid)
            {
                db.Entry(song).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AlbumID = new SelectList(db.Albums, "AlbumID", "Title", song.AlbumID);
            ViewBag.ArtistID = new SelectList(db.Artists, "ArtistID", "Fname", song.ArtistID);
            ViewBag.ComposerID = new SelectList(db.Composers, "ComposerID", "Fname", song.ComposerID);
            return View(song);
        }

        // GET: Songs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!isAdminUser())
                {
                    return Content("Invalid");
                }

            }
            else
            {
                return Content("Invalid");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Song song = db.Songs.Find(id);
            db.Songs.Remove(song);
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
