using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Intranet;
using IntranetPublic.Models;
using System.Text.RegularExpressions;
using IntranetPublic;

namespace Intranet.Controllers
{
    public class DownloadsController : Controller
    {
        private PubSystemEntities db = new PubSystemEntities();

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index(int sort = 1, string filter = "", string search = "", int id_item_user = 0)
        {
            var courses = GetCourses();
            var users = GetUsers();

            var downloads = db.Downloads
                .Where(d => id_item_user == 0 || d.id_user == id_item_user)
                .Where(d => filter == "" || d.course_code == filter)
                .Include(d => d.File)
                .AsEnumerable()
                .Where(d => 
                {
                    if (search == "")
                    {
                        return true;
                    }

                    bool res = false;
                    d.title = Regex.Replace(d.title, search, m => { res = true; return String.Format("<mark>{0}</mark>", m.ToString()); }, RegexOptions.IgnoreCase);
                    d.description = Regex.Replace(d.description, search, m => { res = true; return String.Format("<mark>{0}</mark>", m.ToString()); }, RegexOptions.IgnoreCase);

                    return res;
                })
                .OrderBy(d => sort == 0 ? d.title : "")
                .OrderByDescending(d => sort == 1 ? d.date : DateTime.Today)
                .OrderBy(d => sort == 2 ? d.course_code : "")
                .Select(d => new DownloadViewModel(d, 
                    (courses.FirstOrDefault(c => c.Value == d.course_code) ?? new SelectListItem {Value = d.course_code, Text = d.course_code +" - ???" }).Text ,
                    (users.FirstOrDefault(u => u.Value == d.id_user.ToString()) ?? new SelectListItem { Value = d.id_user.ToString(), Text = "???" }).Text));

            if (sort == 3)
            {
                var usersSort = db.Staff.Select(s => new { id_item_user = s.id_item, name = s.PRIJMENI }).ToList();

                downloads = downloads.OrderBy(d => 
                {
                    var usort = usersSort.FirstOrDefault(u => u.id_item_user == d.id_user_item);

                    if (usort != null)
                        return usort.name;
                    else
                        return "ZZZ";
                })
                .ThenByDescending(d => d.date);
            }

            courses.Insert(0, new SelectListItem { Value = "", Text = "Nerozhoduje" });
            ViewBag.ddlFilter = new SelectList(courses, "Value", "Text", filter);

            users.Insert(0, new SelectListItem { Value = "0", Text = "Nerozhoduje" });
            ViewBag.ddlUsers = new SelectList(users, "Value", "Text", id_item_user);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_DownloadList", downloads.ToList());
            }
            if (ControllerContext.IsChildAction)
            {
                return PartialView("_DownloadListUserHomePage", downloads.ToList());
            }
            else
            {
                return View(downloads.ToList());
            }
        }


        private List<SelectListItem> GetCourses()
        {
            List<SelectListItem> courses = db.Courses
                .OrderBy(c => c.name)
                .Select(c => new SelectListItem
                    {
                        Value = c.code,
                        Text = c.code + " - " + c.name
                    })
                .ToList();

            courses.Insert(0, new SelectListItem { Value = "000", Text = "Neurčeno pro předmět FSI" });

            return courses;
        }

        private List<SelectListItem> GetUsers()
        {
            List<SelectListItem> users = db.Staff
                .OrderBy(u => u.PRIJMENI)
                .Select(u => new SelectListItem
                {
                    Value = u.id_item.ToString(),
                    Text =u.JMENO
                })
                .ToList();

            return users;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
