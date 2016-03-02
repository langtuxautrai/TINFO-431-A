using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MusicProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicProject.Controllers
{
    public class HelpController : Controller
    {
        private MusicContext db = new MusicContext();
        ApplicationDbContext context;
        public HelpController()
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
                else
                {
                    return false;
                }
            }
            return false;
        }

        public ActionResult sortAndSearch()
        {
            return View();
        }
    }
}