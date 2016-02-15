using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Intranet;
using Microsoft.AspNet.Identity;
using Intranet.Models;
using System.Text.RegularExpressions;

namespace Intranet.Controllers
{
    [Authorize]
    public class DownloadsController : Controller
    {
        private PubSystemEntities db = new PubSystemEntities();

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index(int sort = 1, string filter = "", string search = "")
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.SingleOrDefault(u => u.id_user.ToString() == userId);
            var id_item_user = db.FormItemFields.SingleOrDefault(fif => fif.id_fcontrol == 3 && fif.strvalue == user.email).id_item;

            var courses = GetCourses();            

            var downloads = db.Downloads
                .Where(d => d.id_user == id_item_user)
                .Where(d => filter == "" || d.course_code == filter)
                .OrderBy(d => d.title)
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
                .Select(d => new DownloadViewModel(d, (courses.FirstOrDefault(c => c.Value == d.course_code) ?? new SelectListItem {Value = d.course_code, Text = d.course_code +" - ???" }).Text ));

            courses.Insert(0, new SelectListItem { Value = "", Text = "Nerozhoduje" });

            ViewBag.ddlFilter = new SelectList(courses, "Value", "Text", "");

            if (Request.IsAjaxRequest())
            {
                return PartialView("_DownloadList", downloads.ToList());
            }
            else
            {
                ViewBag.DownloadsUrl = GetDownloadsURL();
                return View(downloads.ToList());
            }
        }

        // GET: Downloads/Create
        public ActionResult Create()
        {
            ViewBag.course_code = GetCourses();
            return PartialView("_Create");
        }

        // POST: Downloads/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "title,description,id_file,course_code")] DownloadViewModel download)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.SingleOrDefault(u => u.id_user.ToString() == userId);
                var id_item_user = db.FormItemFields.SingleOrDefault(fif => fif.id_fcontrol == 3 && fif.strvalue == user.email).id_item;

                FormItem newFormItem = new FormItem
                {
                    createdate = DateTime.Now,
                    editdate = DateTime.Now,
                    id_form = 4,
                    id_group = 0,
                    id_item_version = 0,
                    id_user = user.id_user,
                    released = true,
                    saved = true,
                    version = 0
                };

                newFormItem.FormItemFields.Add(new FormItemField
                {
                    id_fcontrol = 38,
                    id_form = 4,
                    datetype = 0,
                    datevalue = new DateTime(1900, 1, 1),
                    intvalue = 0,
                    moneyvalue = 0,
                    numvalue = 0,
                    richvalue = "",
                    strvalue = id_item_user.ToString()
                });

                newFormItem.FormItemFields.Add(new FormItemField
                {
                    id_fcontrol = 39,
                    id_form = 4,
                    datetype = 0,
                    datevalue = new DateTime(1900, 1, 1),
                    intvalue = 0,
                    moneyvalue = 0,
                    numvalue = 0,
                    richvalue = "",
                    strvalue = download.title
                });

                newFormItem.FormItemFields.Add(new FormItemField
                {
                    id_fcontrol = 40,
                    id_form = 4,
                    datetype = 0,
                    datevalue = new DateTime(1900, 1, 1),
                    intvalue = 0,
                    moneyvalue = 0,
                    numvalue = 0,
                    richvalue = "",
                    strvalue = download.description
                });

                newFormItem.FormItemFields.Add(new FormItemField
                {
                    id_fcontrol = 41,
                    id_form = 4,
                    datetype = 9,
                    datevalue = new DateTime(1900, 1, 1),
                    intvalue = download.id_file,
                    moneyvalue = 0,
                    numvalue = 0,
                    richvalue = "",
                    strvalue = ""
                });

                newFormItem.FormItemFields.Add(new FormItemField
                {
                    id_fcontrol = 42,
                    id_form = 4,
                    datetype = 3,
                    datevalue = DateTime.Now,
                    intvalue = 0,
                    moneyvalue = 0,
                    numvalue = 0,
                    richvalue = "",
                    strvalue = ""
                });

                newFormItem.FormItemFields.Add(new FormItemField
                {
                    id_fcontrol = 44,
                    id_form = 4,
                    datetype = 0,
                    datevalue = new DateTime(1900, 1, 1),
                    intvalue = 0,
                    moneyvalue = 0,
                    numvalue = 0,
                    richvalue = "",
                    strvalue = user.lastname
                });

                newFormItem.FormItemFields.Add(new FormItemField
                {
                    id_fcontrol = 95,
                    id_form = 4,
                    datetype = 0,
                    datevalue = new DateTime(1900, 1, 1),
                    intvalue = 0,
                    moneyvalue = 0,
                    numvalue = 0,
                    richvalue = "",
                    strvalue = download.course_code
                });


                db.FormItems.Add(newFormItem);


                db.SaveChanges();
                //return RedirectToAction("Index");

                //string url = Url.Action("Index", "Downloads", new { /*id = address.PersonID*/ });
                return Json(new { success = true/*, url = url*/ });

            }

            ViewBag.course_code = GetCourses();
            return PartialView("_Create", download);
        }

        // GET: Downloads/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Download download = db.Downloads.Find(id);
            if (download == null)
            {
                return HttpNotFound();
            }
            ViewBag.course_code = GetCourses();

            //System.Threading.Thread.Sleep(2000);

            return PartialView("_Edit", new DownloadViewModel(download));
        }

        // POST: Downloads/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_item,title,description,id_file,course_code")] DownloadViewModel download)
        {
            if (ModelState.IsValid)
            {

                db.FormItemFields.SingleOrDefault(fif => fif.id_item == download.id_item && fif.id_fcontrol == 39).strvalue = download.title;
                db.FormItemFields.SingleOrDefault(fif => fif.id_item == download.id_item && fif.id_fcontrol == 40).strvalue = download.description;
                db.FormItemFields.SingleOrDefault(fif => fif.id_item == download.id_item && fif.id_fcontrol == 41).intvalue = download.id_file;
                db.FormItemFields.SingleOrDefault(fif => fif.id_item == download.id_item && fif.id_fcontrol == 95).strvalue= download.course_code;

                db.SaveChanges();

                //string url = Url.Action("Index", "Downloads", new { /*id = address.PersonID*/ });
                return Json(new { success = true/*, url = url*/ });
            }
            ViewBag.course_code = GetCourses();
            return PartialView("_Edit", download);
        }

        // GET: Downloads/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Download download = db.Downloads.Find(id);
            if (download == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Delete", new DownloadViewModel(download));
        }

        // POST: Downloads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FormItem formItem = db.FormItems.Find(id);
            db.FormItems.Remove(formItem);

            db.SaveChanges();

            //string url = Url.Action("Index", "Downloads", new { /*id = address.PersonID*/ });
            return Json(new { success = true/*, url = url*/ });
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

        private string GetDownloadsURL()
        {
            if (User.Identity.IsAuthenticated)
            {

                var userId = User.Identity.GetUserId();
                var user = db.Users.SingleOrDefault(u => u.id_user.ToString() == userId);
                return Intranet.Properties.Settings.Default.BaseURL + "/" + Intranet.Properties.Settings.Default.HomePath + "/" + user.login_name.ToLower() + "/soubory-ke-stazeni";
            }
            else
            {
                return "";
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
