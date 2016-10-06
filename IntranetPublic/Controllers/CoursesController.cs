using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using IntranetPublic.Models;

namespace IntranetPublic.Controllers
{
    public class CoursesController : Controller
    {
        private PubSystemEntities db = new PubSystemEntities();

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index(int sort = 0, string filter = "", string search = "")
        {
            var courses = db.Courses.AsQueryable();

            if (!String.IsNullOrEmpty(filter))
            {
                courses = courses.Where(c => c.semester == filter);
            }

            switch (sort)
            {
                case 1:
                    courses = courses.OrderBy(c => c.code);
                    break;
                case 2:
                    courses = courses.OrderByDescending(c => c.semester).ThenBy(c => c.name);
                    break;
                default:
                    courses = courses.OrderBy(c => c.name);
                    break;
            }

            var courses1 = courses.AsEnumerable().Select(c => new CourseViewModel(c));

            if (!String.IsNullOrEmpty(search))
            {
                courses1 = courses1.Where(d =>
                            {
                                bool res = false;
                                d.CodeSearch = Regex.Replace(d.CodeSearch, search, m => { res = true; return String.Format("<mark>{0}</mark>", m.ToString()); }, RegexOptions.IgnoreCase);
                                d.Name = Regex.Replace(d.Name, search, m => { res = true; return String.Format("<mark>{0}</mark>", m.ToString()); }, RegexOptions.IgnoreCase);
                                return res;
                            });
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_CourseList", courses1.ToList());
            }
            else
            {
                return View(courses1.ToList());
            }
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