using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialMedia.Controllers
{
    public class UserInfoController : Controller
    {
        SocialMediaDBEntities db = new SocialMediaDBEntities();
        // GET: UserInfo
        public ActionResult Index()
        {
            List<tb_UserInfo> lst = db.tb_UserInfo.ToList();
            return View(lst);
        }
    }
}