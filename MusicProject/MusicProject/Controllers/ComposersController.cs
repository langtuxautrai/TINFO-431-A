using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MusicProject.Models;

namespace MusicProject.Controllers
{
    public class ComposersController : Controller
    {
        private MusicContext db = new MusicContext();

        // GET: Composers
        public ActionResult Index()
        {
            var composers = db.Composers.Include(c => c.Companies);
            return View(composers.ToList());
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
            return View(composer);
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
