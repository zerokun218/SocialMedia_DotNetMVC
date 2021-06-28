using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SocialMedia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialMedia.Controllers
{
    //[Authorize]
    public class UsersController : Controller
    {

        public Boolean isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ApplicationDbContext context = new ApplicationDbContext();
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

        // GET: Users
        public ActionResult Index()
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
                var userList = new ApplicationDbContext().Users.ToList();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                List<string> roleList = new List<string>();
                foreach (var u in userList) {
                    if (UserManager.GetRoles(u.Id) != null)
                    {
                        roleList.Add(UserManager.GetRoles(u.Id)[0].ToString());
                        System.Diagnostics.Debug.WriteLine(UserManager.GetRoles(u.Id)[0].ToString());
                    }
                    else {
                        roleList.Add("null");
                    }
                    
                }
                ViewBag.RoleList = roleList;
                return View(userList);
            }
            else
            {
                ViewBag.Name = "Not Logged IN";
            }
            return View();
        }
    }
}