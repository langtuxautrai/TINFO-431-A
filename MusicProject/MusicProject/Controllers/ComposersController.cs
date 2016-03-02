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
                else
                {
                    return false;
                }
            }
            return false;
        }

        // GET: Composers
        public ActionResult Index(string name, string searchString)
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

            var composer = from s in db.Composers select s;

            //Searching the song by song title
            if (!String.IsNullOrEmpty(searchString))
            {
                composer = composer.Where(s => s.FullName.Contains(searchString));
            }

            //Set up sorting cases 
            ViewBag.FnameSorting = String.IsNullOrEmpty(name) ? "Fname_DESC" : "";
            ViewBag.LnameSorting = name == "Lname" ? "Lname_DESC" : "Lname";
            ViewBag.GenreSorting = name == "Genres" ? "Genres_DESC" : "Genres";
            ViewBag.CompanySorting = name == "Company" ? "Company_DESC" : "Company";
           
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
            Composer composer = db.Composers.Find(id);
            if (composer == null)
            {
                return HttpNotFound();
            }

            var songList = db.Composers.Include(s => s.Songs).Single(s => s.ComposerID == id);

            return View(songList);
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
        public ActionResult Create([Bind(Include = "ComposerID,Fname,Lname,Genres,CompanyID,Rewards")] Composer composer)
        {
            if (ModelState.IsValid)
            {
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
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name", composer.CompanyID);
            return View(composer);
        }

        // POST: Composers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ComposerID,Fname,Lname,Genres,CompanyID,Rewards")] Composer composer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(composer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyID = new SelectList(db.Companies, "CompanyID", "Name", composer.CompanyID);
            return View(composer);
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
            return View(composer);
        }

        // POST: Composers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
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
