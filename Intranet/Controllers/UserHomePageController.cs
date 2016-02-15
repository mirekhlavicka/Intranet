using Intranet.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Intranet.Controllers
{
    [Authorize]
    public class UserHomePageController : Controller
    {
        PubSystemEntities data = new PubSystemEntities();

        // GET: UserHomePage/Edit
        public ActionResult Edit()
        {
            string userId = User.Identity.GetUserId();

            User user = data.Users.SingleOrDefault(u => u.id_user.ToString() == userId);

            int id_userpage = 0;

            if (Session["id_userpage"] != null)
            {
                id_userpage = (int)(Session["id_userpage"]);
            }

            string currentBody = "";
            UserPage currentPage = null;
            int id_item = data.FormItemFields.SingleOrDefault(fif => fif.id_fcontrol == 3 && fif.strvalue == user.email).id_item;

            if (id_userpage == 0)
            {
                currentBody = data.FormItemFields.SingleOrDefault(fif => fif.id_item == id_item && fif.id_fcontrol == 45).richvalue;
            }
            else
            {
                currentPage = data.UserPages.SingleOrDefault(up => up.id_user == user.id_user && up.id_userpage == id_userpage);
                currentBody = currentPage.body;
            }

            return View(new UserHomePageViewModel
            {
                User = user,
                CurrentBody = currentBody,
                CurrentPage = currentPage,
                CurrentPageId = id_userpage,
                CurrentPageUrl = GetPageURL(user, currentPage),
                ShowHeader = data.FormItemFields.SingleOrDefault(fif => fif.id_item == id_item && fif.id_fcontrol == 48).strvalue != "none",
                ShowPhoto = data.FormItemFields.SingleOrDefault(fif => fif.id_item == id_item && fif.id_fcontrol == 46).strvalue != "none",
                BootstrapTheme = data.FormItemFields.SingleOrDefault(fif => fif.id_item == id_item && fif.id_fcontrol == 47).strvalue,
                FullName = data.FormItemFields.SingleOrDefault(fif => fif.id_item == id_item && fif.id_fcontrol == 2).strvalue,
                Phone = data.FormItemFields.SingleOrDefault(fif => fif.id_item == id_item && fif.id_fcontrol == 4).strvalue,
                Room = data.FormItemFields.SingleOrDefault(fif => fif.id_item == id_item && fif.id_fcontrol == 5).strvalue
            });

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SaveAndGet(int id_userpage_save, int id_userpage_get, string body)
        {
            var userId = User.Identity.GetUserId();
            var user = data.Users.SingleOrDefault(u => u.id_user.ToString() == userId);

            if (id_userpage_save == 0)
            {
                var id_item = data.FormItemFields.SingleOrDefault(fif => fif.id_fcontrol == 3 && fif.strvalue == user.email).id_item;
                data.FormItemFields.SingleOrDefault(fif => fif.id_item == id_item && fif.id_fcontrol == 45).richvalue = body;
            }
            else if (id_userpage_save > 0)
            {
                data.UserPages.SingleOrDefault(up => up.id_userpage == id_userpage_save).body = body;
            }

            data.SaveChanges();

            UserPage userPage = data.UserPages.SingleOrDefault(up => up.id_userpage == id_userpage_get);

            if (id_userpage_get == 0)
            {
                var id_item = data.FormItemFields.SingleOrDefault(fif => fif.id_fcontrol == 3 && fif.strvalue == user.email).id_item;
                body = data.FormItemFields.SingleOrDefault(fif => fif.id_item == id_item && fif.id_fcontrol == 45).richvalue;
            }
            else
            {
                body = userPage.body;
            }

            Session["id_userpage"] = id_userpage_get;

            return Json(new {
                id_userpage = id_userpage_get,
                body = body,
                url = GetPageURL(user, userPage),
                name = userPage != null ? userPage.name : "Úvod",
                show = userPage != null ? userPage.show : true,
            });
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult NewPage(string name)
        {
            var userId = User.Identity.GetUserId();
            var user = data.Users.SingleOrDefault(u => u.id_user.ToString() == userId);

            int order = (data.UserPages.Where(up => up.id_user == user.id_user && up.show).OrderBy(up => up.order).Max(up => (int?)up.order) ?? 0) + 10;

            UserPage newPage = new UserPage
            {
                body = "",
                id_user = user.id_user,
                name = name,
                order = order,
                show = true,
                url = PubSystem.SEO.Utility.GetSafeString(name)
            };

            data.UserPages.Add(newPage);

            data.SaveChanges();

            return Json(new { id_userpage_new = newPage.id_userpage });
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DeletePage(int id_userpage)
        {
            var userId = Int32.Parse(User.Identity.GetUserId());

            UserPage delPage = data.UserPages.SingleOrDefault(up => up.id_userpage == id_userpage && up.id_user == userId);

            data.UserPages.Remove(delPage);

            data.SaveChanges();

            return Json(new {  });
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RenamePage(int id_userpage, string name)
        {
            var userId = Int32.Parse(User.Identity.GetUserId());

            UserPage renamePage = data.UserPages.SingleOrDefault(up => up.id_userpage == id_userpage && up.id_user == userId);

            renamePage.name = name;
            renamePage.url = PubSystem.SEO.Utility.GetSafeString(name);

            data.SaveChanges();

            return Json(new { });
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ShowHidePage(int id_userpage)
        {
            var userId = Int32.Parse(User.Identity.GetUserId());

            UserPage showHidePage = data.UserPages.SingleOrDefault(up => up.id_userpage == id_userpage && up.id_user == userId);

            if (showHidePage.show)
            {
                showHidePage.show = false;
                showHidePage.order = 0;
            }
            else
            {
                showHidePage.show = true;
                showHidePage.order = (data.UserPages.Where(up => up.id_user == userId && up.show).OrderBy(up => up.order).Max(up => (int?)up.order) ?? 0) + 10;
            }


            data.SaveChanges();

            return Json(new {show = showHidePage.show });
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult MovePage(int id_userpage, int direction)
        {
            var userId = Int32.Parse(User.Identity.GetUserId());

            UserPage movePage = data.UserPages.SingleOrDefault(up => up.id_userpage == id_userpage && up.id_user == userId);

            if (!movePage.show)
            {
                return Json(new { notShown = true });
            }

            UserPage secondPage = null;

            if (direction == 1)
            {
                secondPage = data.UserPages.Where(up => up.id_user == userId && up.show).OrderBy(up => up.order).FirstOrDefault(up => up.order > movePage.order);
            }
            else
            {
                secondPage = data.UserPages.Where(up => up.id_user == userId && up.show).OrderByDescending(up => up.order).FirstOrDefault(up => up.order < movePage.order);
            }

            if (secondPage != null)
            {
                var tmp = secondPage.order;
                secondPage.order = movePage.order;
                movePage.order = tmp;
            }

            data.SaveChanges();

            return Json(new { notShown = false, moved = secondPage != null });
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult SaveSettings(int id_userpage, bool showHeader, bool showPhoto, string bootstrapTheme, string fullName, string phone, string room)
        {
            var userId = User.Identity.GetUserId();
            var user = data.Users.SingleOrDefault(u => u.id_user.ToString() == userId);

            var id_item = data.FormItemFields.SingleOrDefault(fif => fif.id_fcontrol == 3 && fif.strvalue == user.email).id_item;
            data.FormItemFields.SingleOrDefault(fif => fif.id_item == id_item && fif.id_fcontrol == 48).strvalue = showHeader ? "block" : "none";
            data.FormItemFields.SingleOrDefault(fif => fif.id_item == id_item && fif.id_fcontrol == 46).strvalue = showPhoto ? "block" : "none";
            data.FormItemFields.SingleOrDefault(fif => fif.id_item == id_item && fif.id_fcontrol == 47).strvalue = bootstrapTheme;
            data.FormItemFields.SingleOrDefault(fif => fif.id_item == id_item && fif.id_fcontrol == 2).strvalue = fullName;
            data.FormItemFields.SingleOrDefault(fif => fif.id_item == id_item && fif.id_fcontrol == 4).strvalue = phone;
            data.FormItemFields.SingleOrDefault(fif => fif.id_item == id_item && fif.id_fcontrol == 5).strvalue = room;

            data.SaveChanges();


            return Json(new {});
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post), ValidateInput(false)]
        public ActionResult RefreshPageList(int id_userpage_current)
        {
            var userId = User.Identity.GetUserId();
            var user = data.Users.SingleOrDefault(u => u.id_user.ToString() == userId);


            ViewData["CurrentPage"] = id_userpage_current;

            return PartialView("_PageList", data.UserPages.Where(up => up.id_user == user.id_user /*&& up.show*/).OrderBy(up => up.order).ToArray());
        }

        private string GetPageURL(User user, UserPage up)
        {
            return Intranet.Properties.Settings.Default.BaseURL + "/" + Intranet.Properties.Settings.Default.HomePath + "/" + user.login_name.ToLower() + (up == null ? "" : "/" + up.url);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                data.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}