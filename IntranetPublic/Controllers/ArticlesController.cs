using IntranetPublic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace IntranetPublic.Controllers
{
    public class ArticlesController : Controller
    {
        private PubSystemEntities db = new PubSystemEntities();

        public ActionResult Index(int section = 0, string lang = "cz", int page = 1, string search = "")
        {
            int pageSize = 8;

            search = search.ToAccentInsensitiveRegex();

            var qarticles = db.Articles
                .Where(a => a.released && !a.del && a.complete && a.datereleased <= DateTime.Now)
                .Where(a => search == "" || (a.title.Contains(search) || a.annotation.Contains(search)));

            if (section == -1)
                qarticles = qarticles
                    .Where(a => a.id_arttype == 4);
            else
                qarticles = qarticles
                    .Where(a => a.ArticleSections.Any(ase => ase.id_section == section));

            qarticles = qarticles
                .OrderByDescending(a => a.datereleased)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var articles = qarticles
                .AsEnumerable()
                .ToList();

            if (search != "")
            {
                articles.ForEach(a =>
                {
                    a.title = Regex.Replace(a.title, search, m => { return String.Format("<mark>{0}</mark>", m.ToString()); }, RegexOptions.IgnoreCase);
                    a.annotation = Regex.Replace(a.annotation, search, m => { return String.Format("<mark>{0}</mark>", m.ToString()); }, RegexOptions.IgnoreCase);

                });
            }

            ViewBag.lang = lang;
            ViewBag.section = section;

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ArticleList", articles);
            }
            else
            {

                if (section == -1)
                {
                    ViewBag.SectionName = "Aktuální informace";
                }
                else
                {
                    ViewBag.SectionName = db.Sections.SingleOrDefault(s => s.id_section == section).name;
                }
                return View(articles);
            }
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
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