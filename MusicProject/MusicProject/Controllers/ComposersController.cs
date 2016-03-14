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
   
    public class ComposersController : Controller
    {
        private MusicContext db = new MusicContext();
        ApplicationDbContext context;

        public ComposersController()
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
            }
            return false;
        }

        // do a quick search the return a json
        public ActionResult QuickSearch(string term)
        {
            var composers = GetComposers(term).Select(a => new { value = a.Lname });
            return Json(composers, JsonRequestBehavior.AllowGet);
        }

        //get the songs in db
        private List<Composer> GetComposers(string searchString)
        {
            return db.Composers
                .Where(a => a.Lname.Contains(searchString) || a.Fname.Contains(searchString))
                .ToList();
        }

        // GET: Composers
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
            ViewBag.FnameSorting = String.IsNullOrEmpty(name) ? "Fname_DESC" : "";
            ViewBag.LnameSorting = name == "Lname" ? "Lname_DESC" : "Lname";
            ViewBag.GenreSorting = name == "Genres" ? "Genres_DESC" : "Genres";
            ViewBag.CompanySorting = name == "Company" ? "Company_DESC" : "Company";
            ViewBag.CreateSorting = name == "Create" ? "Create_DESC" : "Create";

            if (searchString == null)
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var composer = from s in db.Composers select s;

            //Searching the song by song title
            if (!String.IsNullOrEmpty(searchString))
            {
                composer = composer.Where(s => s.Lname.Contains(searchString)
                            || s.Fname.Contains(searchString));
            }

            //Sorting by title, genre, or artist first name
            switch (name)
            {
                case "Fname_DESC":
                    composer = composer.OrderByDescending(s => s.Fname);
                    break;
                case "Lname":
                    composer = composer.OrderBy(s => s.Lname);
                    break;
                case "Genres":
                    composer = composer.OrderBy(s => s.Genres);
                    break;
                case "Genres_DESC":
                    composer = composer.OrderByDescending(s => s.Genres);
                    break;
                case "Company":
                    composer = composer.OrderBy(s => s.Companies.Name);
                    break;
                case "Company_DESC":
                    composer = composer.OrderByDescending(s => s.Companies.Name);
                    break;
                case "Create":
                    composer = composer.OrderBy(s => s.CreateOrUpdate);
                    break;
                case "Create_DESC":
                    composer = composer.OrderByDescending(s => s.CreateOrUpdate);
                    break;
                default:
                    composer = composer.OrderBy(s => s.Fname);
                    break;
            }

            return View(composer.ToList());
        }

        // GET: Composers/Details/5
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
            Composer composer = db.Composers.Find(id);
            if (composer == null)
            {
                return HttpNotFound();
            }
            getImage(composer);

            var songList = db.Composers.Include(s => s.Songs).Single(s => s.ComposerID == id);

            return View(songList);
        }

        public void getImage(Composer composer)
        {
            if (composer.Image != null)
            {
                byte[] imageByteData = composer.Image;
                string imageBase64Data = Convert.ToBase64String(imageByteData);
                string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                ViewBag.ImageData = imageDataURL;
            }
        }

        // GET: Composers/Create
        public ActionResult Create()
        {
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name");
            return View();
        }

        // POST: Composers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ComposerID,Fname,Lname,Genres,CompanyID,Rewards")] Composer composer, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                    {
                        composer.Image = reader.ReadBytes(upload.ContentLength);
                        composer.ImageName = upload.FileName;
                    }
                }
                composer.CreateOrUpdate = System.DateTime.Now;

                db.Composers.Add(composer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name", composer.CompanyID);
            return View(composer);
        }

        // GET: Composers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Composer composer = db.Composers.Find(id);

            if (composer == null)
            {
                return HttpNotFound();
            }

            getImage(composer);

            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name", composer.CompanyID);
            return View(composer);
        }

        // POST: Composers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, HttpPostedFileBase upload)
        {
            Composer composer = db.Composers.FirstOrDefault(i => i.ComposerID == id);

            if (upload != null && upload.ContentLength > 0)
            {
                using (var reader = new System.IO.BinaryReader(upload.InputStream))
                {
                    composer.Image = reader.ReadBytes(upload.ContentLength);
                    composer.ImageName = upload.FileName;
                }
            }
            composer.CreateOrUpdate = System.DateTime.Now;
            UpdateModel(composer);
            db.SaveChanges();

            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name", composer.CompanyID);

            return RedirectToAction("Details", new { id = composer.ComposerID });
        }

        // GET: Composers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Composer composer = db.Composers.Find(id);
            if (composer == null)
            {
                return HttpNotFound();
            }
            getImage(composer);
            return View(composer);
        }

        // POST: Composers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var songs = from s in db.Songs select s;
            songs = songs.Where(s => s.ComposerID == id);

            foreach (var s in songs)
            {
                s.ComposerID = null;
            }

            Composer composer = db.Composers.Find(id);
            db.Composers.Remove(composer);
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
