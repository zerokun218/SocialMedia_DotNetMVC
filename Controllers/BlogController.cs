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
            if (!User.Identity.IsAuthenticated) {
                return false;
            }
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

        [Authorize]
        public ActionResult Create(int? groupId, string returnUrl) {
            var UserId = User.Identity.GetUserId();
            ViewBag.UserId = UserId;

            if (groupId != null) {
                Session["GroupId"] = groupId;
            }
            if (!String.IsNullOrEmpty(returnUrl)) {
                Session["returnUrl"] = returnUrl;
            }

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
            tb_Blog addedBlog = DAL.DBContext.addNewBlog(b);

            if (Session["GroupId"] != null) {
                tb_BlogGroup blogGroup = new tb_BlogGroup();
                blogGroup.BlogId = addedBlog.Id;
                blogGroup.GroupId = Convert.ToInt32(Session["GroupId"]);
                db.tb_BlogGroup.Add(blogGroup);
                db.SaveChanges();

                Session["GroupId"] = null;
            }

            if (Session["returnUrl"] != null) {
                var url = Session["returnUrl"].ToString();
                Session["returnUrl"] = null;
                return Redirect(url);
            }

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
        public ActionResult Delete(int? id, string returnUrl) {
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
        public ActionResult Delete(int id, string returnUrl) {
            
            DAL.DBContext.deleteBlog(id);
            if (!String.IsNullOrEmpty(returnUrl)) {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Detail(int id, string comment, int? pageNumber) {
            //Add a comment to a Blog with Current User
            if (!String.IsNullOrEmpty(comment) && User.Identity.IsAuthenticated)
            {
                tb_Comment cmt = new tb_Comment();
                cmt.BlogId = id;
                var currentUserId = User.Identity.GetUserId();
                cmt.UserId = db.tb_UserInfo.Where(u => u.UserId == currentUserId).FirstOrDefault().Id;
                cmt.Content = comment;
                db.tb_Comment.Add(cmt);
                db.SaveChanges();
            }
            if (pageNumber == null) {
                pageNumber = 1;
            }
            int itemsPerPage = 3;
            tb_Blog bl = db.tb_Blog.Where(b => b.Id == id).FirstOrDefault();
            Blog blog = new Blog(bl);

            List<tb_Comment> lstComment = db.tb_Comment.Where(c => c.BlogId == id).ToList();
            lstComment.Reverse();
            int pageCount = lstComment.Count() / itemsPerPage;
            if (lstComment.Count() % itemsPerPage != 0) {
                pageCount += 1;
            }

            lstComment = lstComment.Skip(Convert.ToInt32(pageNumber - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            if (checkUserLikeBlog(id))
            {
                ViewBag.IsLike = "Like";
            }

            ViewBag.PageCount = pageCount;
            ViewBag.CurrentPage = pageNumber;
            ViewData["Comments"] = lstComment;
            return View(blog);
        }

        //Check Current User Like a blog: take Blog's Id to return user like or not
        public bool checkUserLikeBlog(int BlogId) {
            if (!User.Identity.IsAuthenticated) {
                return false;
            }
            var currentUserId = User.Identity.GetUserId();
            var userInfoId = db.tb_UserInfo.Where(u => u.UserId == currentUserId).FirstOrDefault().Id;
            var obj = db.tb_Favorite.Where(o => o.BlogId == BlogId && o.UserId == userInfoId).FirstOrDefault();
            if (obj != null) {
                return true;
            }
            return false;
        }

        [Authorize]
        public ActionResult Like(int id, string returnUrl) {
            tb_Favorite favor = new tb_Favorite();
            favor.BlogId = id;
            var user = DAL.DBContext.GetUserInfo(User.Identity.GetUserId());
            favor.UserId = user.Id;
            db.tb_Favorite.Add(favor);
            db.SaveChanges();
            if (!String.IsNullOrEmpty(returnUrl)) {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Detail", new { id = id });
        }

        [Authorize]
        public ActionResult Unlike(int id, string returnUrl) {
            var userId = DAL.DBContext.GetUserInfo(User.Identity.GetUserId()).Id;
            tb_Favorite favor = db.tb_Favorite.Where(f => f.BlogId == id && f.UserId == userId).FirstOrDefault();
            db.tb_Favorite.Remove(favor);
            db.SaveChanges();
            if (!String.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Detail", new { id = id });
        }
    }
}