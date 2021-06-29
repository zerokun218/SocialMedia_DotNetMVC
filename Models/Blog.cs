using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Models
{
    public class Blog
    {

        public int Id { get; set; }
        public string Author { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }

        public Blog() { 
        }

        public Blog(tb_Blog blog) {
            this.Id = blog.Id;
            this.Author = blog.tb_UserInfo.Username;
            this.Title = blog.Title;
            this.Content = blog.Content;
        }

        public static void EditBlogDB(Blog blog) {
            var db = DAL.DBContext.db;
            tb_Blog bl = db.tb_Blog.Where(b => b.Id == blog.Id).FirstOrDefault();
            db.tb_Blog.Remove(bl);
            db.SaveChanges();
            tb_Blog newBlog = new tb_Blog();
            newBlog.Id = blog.Id;
            newBlog.Author = bl.Author;
            newBlog.Title = blog.Title;
            newBlog.Content = blog.Content;
            db.tb_Blog.Add(newBlog);
            db.SaveChanges();
        }
    }
}