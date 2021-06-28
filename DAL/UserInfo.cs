using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialMedia.DAL
{
    public class UserInfo
    {
        private int Id { get; set; }
        private string UserId { get; set; }
        private string Username { get; set; }
        private int Age { get; set; }

        public UserInfo() { 
        }
        public UserInfo(string UserId) {
            this.UserId = UserId;
            this.Username = "No Name";
            this.Age = 0;
        }
        public UserInfo(string UserId, string Username) {
            this.UserId = UserId;
            this.Username = Username;
            this.Age = 0;
        }
    }
}