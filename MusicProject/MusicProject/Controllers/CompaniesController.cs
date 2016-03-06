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
    public class CompaniesController : Controller
    {
        private MusicContext db = new MusicContext();
        ApplicationDbContext context;

        public CompaniesController()
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

        // do a quick search the return a json
        public ActionResult QuickSearch(string term)
        {
            var companies = GetCompanies(term).Select(a => new { value = a.Name });
            return Json(companies, JsonRequestBehavior.AllowGet);
        }

        //get the songs in db
        private List<Company> GetCompanies(string searchString)
        {
            return db.Companies
                .Where(a => a.Name.Contains(searchString))
                .ToList();
        }

        // GET: Companies
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

            var company = from s in db.Companies select s;

            //Searching the song by song title
            if (!String.IsNullOrEmpty(searchString))
            {
                company = company.Where(s => s.Name.Contains(searchString));
            }

            //Set up sorting cases 
            ViewBag.TitleSorting = String.IsNullOrEmpty(name) ? "Name_DESC" : "";
            ViewBag.AddressSorting = name == "Address" ? "Address_DESC" : "Address";
            ViewBag.PhoneSorting = name == "Phone" ? "Phone_DESC" : "Phone";
            ViewBag.WebsiteSorting = name == "Website" ? "Website_DESC" : "Website";            
            ViewBag.DateSorting = name == "Date" ? "Date_DESC" : "Date";

            //Sorting by title, genre, or artist first name
            switch (name)
            {
                case "Name_DESC":
                    company = company.OrderByDescending(s => s.Name);
                    break;
                case "Address":
                    company = company.OrderBy(s => s.Address);
                    break;
                case "Address_DESC":
                    company = company.OrderByDescending(s => s.Address);
                    break;
                case "Phone":
                    company = company.OrderBy(s => s.phone);
                    break;
                case "Phone_DESC":
                    company = company.OrderByDescending(s => s.phone);
                    break;
                case "Website":
                    company = company.OrderBy(s => s.Website);
                    break;
                case "Website_DESC":
                    company = company.OrderByDescending(s => s.Website);
                    break;
                case "Date":
                    company = company.OrderBy(s => s.Found);
                    break;
                case "Date_DESC":
                    company = company.OrderByDescending(s => s.Found);
                    break;
                default:
                    company = company.OrderBy(s => s.Name);
                    break;
            }

            return View(company.ToList());
        }

        // GET: Companies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // GET: Companies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CompanyID,Name,Address,phone,Website,Found")] Company company)
        {
            if (ModelState.IsValid)
            {
                db.Companies.Add(company);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(company);
        }

        // GET: Companies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CompanyID,Name,Address,phone,Website,Found")] Company company)
        {
            if (ModelState.IsValid)
            {
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(company);
        }

        // GET: Companies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Company company = db.Companies.Find(id);
            db.Companies.Remove(company);
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
