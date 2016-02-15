using IntranetPublic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntranetPublic.Controllers
{
    public class UserHomePageController : Controller
    {
        PubSystemEntities data = new PubSystemEntities();

        // GET: UserHomePage
        public ActionResult Index(string username, string page = "")
        {
            User user = data.Users.SingleOrDefault(u => u.login_name == username);

            if (user == null)
            {
                return HttpNotFound();
            }

            StaffItem staffInfo = data.Staff.SingleOrDefault(s => s.EMAIL == user.email);

            var userPages = data.UserPages.Where(up => up.id_user == user.id_user);
            UserPage currentPage = userPages.FirstOrDefault(up => up.url == page);
            
            string currentBody = "";
            
            if (currentPage == null)
            {
                currentBody = staffInfo.INFO;
            }
            else
            {
                currentBody = currentPage.body;
            }
            
            return View(new UserHomePageViewModel
            {
                User = user,
                IdItem = staffInfo.id_item,
                CurrentBody = currentBody,
                CurrentPageId = (currentPage != null ? currentPage.id_userpage : 0),
                CurrentPageTitle = (currentPage != null ? currentPage.name : ""),
                UserPages = userPages.Where(up => up.show).OrderBy(up => up.order).ToArray(),
                StaffInfo = staffInfo
            });
        }

        public ActionResult TimeTable(string username)
        {
            User user = data.Users.SingleOrDefault(u => u.login_name == username);

            if (user == null)
            {
                return HttpNotFound();
            }

            StaffItem staffInfo = data.Staff.SingleOrDefault(s => s.EMAIL == user.email);

            var userPages = data.UserPages.Where(up => up.id_user == user.id_user);

            return View(new UserHomePageViewModel
            {
                User = user,
                IdItem = staffInfo.id_item,
                CurrentBody = "",
                CurrentPageId = -1,
                CurrentPageTitle = "",
                UserPages = userPages.Where(up => up.show).OrderBy(up => up.order).ToArray(),
                StaffInfo = staffInfo
            });
        }

        public ActionResult Downloads(string username)
        {
            User user = data.Users.SingleOrDefault(u => u.login_name == username);

            if (user == null)
            {
                return HttpNotFound();
            }

            StaffItem staffInfo = data.Staff.SingleOrDefault(s => s.EMAIL == user.email);

            var userPages = data.UserPages.Where(up => up.id_user == user.id_user);

            return View(new UserHomePageViewModel
            {
                User = user,
                IdItem = staffInfo.id_item,
                CurrentBody = "",
                CurrentPageId = -2,
                CurrentPageTitle = "",
                UserPages = userPages.Where(up => up.show).OrderBy(up => up.order).ToArray(),
                StaffInfo = staffInfo
            });
        }
    }
}