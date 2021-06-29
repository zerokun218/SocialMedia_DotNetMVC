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
        public ApplicationDbContext context = new ApplicationDbContext();
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
                
                var userList = context.Users.ToList();
                return View(userList);
            }
            else
            {
                ViewBag.Name = "Not Logged IN";
            }
            return View();
        }

        [Authorize]
        public ActionResult Detail(string Id) {
            if (String.IsNullOrEmpty(Id)) {
                Id = User.Identity.GetUserId();
                ViewBag.AllowChangePassword = "true";
            }
            var user = (context).Users.Where(u => u.Id == Id).FirstOrDefault();
            return View(user);
        }

        [Authorize]
        public ActionResult Edit(string Id) {
            var user = (context).Users.Where(u => u.Id == Id).FirstOrDefault();
            string roleId = user.Roles.Where(r => r.UserId == user.Id).FirstOrDefault().RoleId;
            string role = context.Roles.Where(r => r.Id == roleId).FirstOrDefault().Name;
            ViewBag.CurrentRole = role;
            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(ApplicationUser user, string role)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            var oldUser = context.Users.Where(u => u.Id == user.Id).FirstOrDefault();
            //Delete current User
            var result = context.Users.Remove(oldUser);
            context.SaveChanges();
            //Add Edited User
            context.Users.Add(user);
            context.SaveChanges();
            //Add role to Edited User
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            UserManager.AddToRole(user.Id, role);
            return RedirectToAction("Index");

        }

    }
}