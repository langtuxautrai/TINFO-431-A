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
            ViewBag.NameSorting = String.IsNullOrEmpty(name) ? "Name_DESC" : "";
            ViewBag.AddressSorting = name == "Address" ? "Address_DESC" : "Address";
            ViewBag.PhoneSorting = name == "Phone" ? "Phone_DESC" : "Phone";
            ViewBag.WebsiteSorting = name == "Website" ? "Website_DESC" : "Website";
            ViewBag.DateSorting = name == "Date" ? "Date_DESC" : "Date";
            ViewBag.CreateSorting = name == "Create" ? "Create_DESC" : "Create";

            if (searchString == null)
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var company = from s in db.Companies select s;

            //Searching the song by song title
            if (!String.IsNullOrEmpty(searchString))
            {
                company = company.Where(s => s.Name.Contains(searchString));
            }
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
                case "Create":
                    company = company.OrderBy(s => s.CreateOrUpdate);
                    break;
                case "Create_DESC":
                    company = company.OrderByDescending(s => s.CreateOrUpdate);
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
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.displayMenu = "Member";
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            getImage(company);

            var companyModel = db.Companies.Include(s => s.Albums).Include(s => s.Artists).Include(s => s.Composers).Single(g => g.CompanyID == id);

            return View(companyModel);
        }

        //Convert the image data in the database into image to display on the page
        public void getImage(Company company)
        {
            if (company.Image != null)
            {
                byte[] imageByteData = company.Image;
                string imageBase64Data = Convert.ToBase64String(imageByteData);
                string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                ViewBag.ImageData = imageDataURL;
            }
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
        public ActionResult Create([Bind(Include = "CompanyID,Name,Address,phone,Website,Found")] Company company, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                    {
                        company.Image = reader.ReadBytes(upload.ContentLength);
                        company.ImageName = upload.FileName;
                    }
                }
                company.CreateOrUpdate = System.DateTime.Now;

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
            getImage(company);
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, HttpPostedFileBase upload)
        {
            Company company = db.Companies.FirstOrDefault(i => i.CompanyID == id);

            if (upload != null && upload.ContentLength > 0)
            {
                using (var reader = new System.IO.BinaryReader(upload.InputStream))
                {
                    company.Image = reader.ReadBytes(upload.ContentLength);
                    company.ImageName = upload.FileName;
                }
            }
            company.CreateOrUpdate = System.DateTime.Now;
            UpdateModel(company);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = company.CompanyID });
        }

        // GET: Companies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            //var album = from s in db.Albums select s;
            //album = album.Where(s => s.CompanyID == id);
            Album album = db.Albums.FirstOrDefault(s => s.CompanyID == id);
            if (album != null)
            {
                ViewBag.Delete = "No";
            }
            if (company == null)
            {
                return HttpNotFound();
            }
            getImage(company);
            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            Company company = db.Companies.Find(id);

            //var album = from s in db.Albums select s;
            //album = album.Where(s => s.CompanyID == id);

            //if (album != null)
            //{
            //    return RedirectToAction("Delete", new { id = company.CompanyID });
            //}

            //remove CompanyID from Artist table
            var artists = from a in db.Artists select a;
            artists = artists.Where(a => a.CompanyID == id);
            foreach (var a in artists)
            {
                a.CompanyID = null;
            }

            //remove CompanyID from Composer table
            var comp = from a in db.Composers select a;
            comp = comp.Where(a => a.CompanyID == id);
            foreach (var a in comp)
            {
                a.CompanyID = null;
            }

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
