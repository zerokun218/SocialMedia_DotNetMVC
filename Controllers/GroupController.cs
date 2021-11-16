using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace SocialMedia.Controllers
{
    public class GroupController : Controller
    {
        SocialMediaDBEntities db = new SocialMediaDBEntities();
        // GET: Group
        public ActionResult Index()
        {
            var lstGroup = db.tb_Group.ToList();

            //Check Current User Joined in a Group or not
            if (User.Identity.IsAuthenticated) {
                var user = DAL.DBContext.GetUserInfo(User.Identity.GetUserId());
                var UserInGroups = db.tb_UserGroup.Where(m => m.MemberId == user.Id).ToList();
                foreach (var u in UserInGroups) {
                    string str = "Joined_GroupId_" + u.GroupId;
                    ViewData[str] = "true";
                }
            }


            return View(lstGroup);
        }

        public ActionResult Detail(int? id) {
            if (id == null) {
                return RedirectToAction("Index");
            }

            var group = db.tb_Group.Where(g => g.Id == id).FirstOrDefault();

            var lstMember = db.tb_UserGroup.Where(ug => ug.GroupId == id).ToList();
            ViewData["Members"] = lstMember;

            var user = DAL.DBContext.GetUserInfo(User.Identity.GetUserId());
            if (User.Identity.IsAuthenticated && db.tb_UserGroup.Where(m => m.GroupId == id && m.MemberId == user.Id).ToList().Count() != 0) {
                ViewBag.IsUserIn = "true";
            }
                


            //Get Blog list in a Group

            var lstBlog = db.tb_BlogGroup.Where(b => b.GroupId == id).ToList();
            ViewData["BlogList"] = lstBlog;

            return View(group);
        }
        
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(tb_Group group) {
            group.Owner = DAL.DBContext.GetUserInfo(User.Identity.GetUserId()).Id;
            db.tb_Group.Add(group);
            db.SaveChanges();
            var lastGroup = db.tb_Group.ToList().LastOrDefault();
            var UserGroup = new tb_UserGroup();
            UserGroup.MemberId = group.Owner;
            UserGroup.GroupId = lastGroup.Id;
            db.tb_UserGroup.Add(UserGroup);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Delete(int? id) {
            
            if (id == null) {
                return RedirectToAction("Index");
            }
            var group = db.tb_Group.Where(g => g.Id == id).FirstOrDefault();
            var user = DAL.DBContext.GetUserInfo(User.Identity.GetUserId());
            if (user.Id != group.Owner) {
                return RedirectToAction("Detail", new { id = id });
            }
            return View(group);   
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            var group = db.tb_Group.Where(g => g.Id == id).FirstOrDefault();
            var memberList = db.tb_UserGroup.Where(m => m.GroupId == group.Id).ToList();
            foreach (var mem in memberList)
            {
                db.tb_UserGroup.Remove(mem);
                db.SaveChanges();
            }
            db.tb_Group.Remove(group);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        [Authorize]
        public ActionResult DeleteMember(int? id, string memberId, string returnUrl) {
            if (id == null || String.IsNullOrEmpty(memberId)) {
                return RedirectToAction("index");
            }

            if (!String.IsNullOrEmpty(returnUrl)) {
                Session["ReturnUrl"] = returnUrl;
            }

            var user = DAL.DBContext.GetUserInfo(memberId);

            var member = db.tb_UserGroup.Where(m => m.MemberId == user.Id && m.GroupId == id).FirstOrDefault();

            return View(member);
        }

        [HttpPost]
        public ActionResult DeleteMember(int id, string memberId, string returnUrl) {
            var user = DAL.DBContext.GetUserInfo(memberId);
            var m = db.tb_UserGroup.Where(mem => mem.MemberId == user.Id && mem.GroupId == id).FirstOrDefault();
            db.tb_UserGroup.Remove(m);
            db.SaveChanges();
            if (!String.IsNullOrEmpty(returnUrl)) {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index");
        }


        //User request to join in a group
        [Authorize]
        public ActionResult JoinGroup(int? id) {
            if (id == null) {
                return RedirectToAction("Index");
            }

            var user = DAL.DBContext.GetUserInfo(User.Identity.GetUserId());

            if (db.tb_UserGroup.Where(m => m.GroupId == id && m.MemberId == user.Id).ToList().Count() != 0)
            {
                return RedirectToAction("Index");
            }
            var member = new tb_UserGroup();
            member.MemberId = user.Id;
            member.GroupId = Convert.ToInt32(id);
            
            db.tb_UserGroup.Add(member);
            db.SaveChanges();

            return RedirectToAction("Detail", "Group", new { id = id });
        }

        [Authorize]
        public ActionResult CreateBlog(int Id, string returnUrl) {
            var user = DAL.DBContext.GetUserInfo(User.Identity.GetUserId());
            if (db.tb_UserGroup.Where(m => m.GroupId == Id && m.MemberId == user.Id).ToList().Count() == 0) {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Create", "Blog", new { groupId = Id, returnUrl = returnUrl });
        }

    }
}