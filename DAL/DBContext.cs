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

        public static void addNewBlog(tb_Blog blog) {
            db.tb_Blog.Add(blog);
            db.SaveChanges();
        }

    }
}