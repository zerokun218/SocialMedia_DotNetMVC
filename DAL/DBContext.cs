using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SocialMedia.Models;

namespace SocialMedia.DAL
{
    public static class DBContext
    {
        public static SocialMediaDBEntities db = new SocialMediaDBEntities();

        public static void addNewUserInfo(ApplicationUser user) {
            tb_UserInfo userInfo = new tb_UserInfo();
            userInfo.UserId = user.Id;
            userInfo.Username = user.UserName;
            userInfo.Role = user.Roles.FirstOrDefault().RoleId;
            db.tb_UserInfo.Add(userInfo);
            db.SaveChanges();
        }

        public static tb_Blog addNewBlog(tb_Blog blog) {
            db.tb_Blog.Add(blog);
            db.SaveChanges();
            tb_Blog b = db.tb_Blog.ToList().Last();
            return b;
        }

        public static void deleteBlog(int id) {
            tb_Blog blog = db.tb_Blog.Where(b => b.Id == id).FirstOrDefault();
            var lstCmt = db.tb_Comment.Where(c => c.BlogId == blog.Id).ToList();
            foreach (var item in lstCmt) {
                db.tb_Comment.Remove(item);
                db.SaveChanges();
            }

            var lstFavor = db.tb_Favorite.Where(c => c.BlogId == blog.Id).ToList();
            foreach (var item in lstFavor)
            {
                db.tb_Favorite.Remove(item);
                db.SaveChanges();
            }

            var lstGroup = db.tb_BlogGroup.Where(c => c.BlogId == blog.Id).ToList();
            foreach (var item in lstGroup)
            {
                db.tb_BlogGroup.Remove(item);
                db.SaveChanges();
            }

            db.tb_Blog.Remove(blog);
            db.SaveChanges();

        }

        public static tb_UserInfo GetUserInfo(string userId)
        {
            var user = db.tb_UserInfo.Where(u => u.UserId == userId).FirstOrDefault();
            return user;
        }

    }
}