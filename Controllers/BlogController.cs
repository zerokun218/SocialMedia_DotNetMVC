using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SocialMedia.Models;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SocialMedia.Controllers
{
    public class BlogController : Controller
    {
        // GET: Blog
        SocialMediaDBEntities db = new SocialMediaDBEntities();

        //Check Current User is the Author of a Blog
        public bool CheckUserAuthor(tb_Blog b) {
            var CurrentUserId = User.Identity.GetUserId();
            var user = db.tb_UserInfo.Where(u => u.UserId == CurrentUserId).FirstOrDefault();
            if (user.Id != b.Author)
            {
                return false;
            }
            return true;
        }
        public ActionResult Index()
        {
            List<Blog> lstBlog = new List<Blog>();
            
            foreach (var item in db.tb_Blog.ToList()) {
                Blog b = new Blog(item);
                lstBlog.Add(b);
            }
            return View(lstBlog);
        }

        public ActionResult Create() {
            var UserId = User.Identity.GetUserId();
            ViewBag.UserId = UserId;
            return View();
        }

        
        [HttpPost]
        [Authorize]
        public ActionResult Create(Blog blog)
        {
            if (!ModelState.IsValid) {
                return View(blog);
            }
            tb_Blog b = new tb_Blog();
            var UserId = User.Identity.GetUserId();
            b.Author = db.tb_UserInfo.Where(u => u.UserId == UserId).FirstOrDefault().Id;
            b.Title = blog.Title;
            b.Content = blog.Content;
            DAL.DBContext.addNewBlog(b);
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Edit(int? id) {
            if (id == null) {
                return RedirectToAction("Index");
            }
            
            tb_Blog b = db.tb_Blog.Where(bl => bl.Id == id).FirstOrDefault();
            if (!CheckUserAuthor(b)) {
                return RedirectToAction("Index");
            }
            Blog blog = new Blog(b);
            return View(blog);
        }

        [HttpPost]
        public ActionResult Edit(Blog blog) {
            if (!ModelState.IsValid) {
                return View(blog);
            }
            Blog.EditBlogDB(blog);
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Delete(int? id) {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            tb_Blog b = db.tb_Blog.Where(bl => bl.Id == id).FirstOrDefault();
            if (!CheckUserAuthor(b))
            {
                return RedirectToAction("Index");
            }
            Blog blog = new Blog(b);
            return View(blog);
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            tb_Blog bl = db.tb_Blog.Where(b => b.Id == id).FirstOrDefault();
            db.tb_Blog.Remove(bl);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}