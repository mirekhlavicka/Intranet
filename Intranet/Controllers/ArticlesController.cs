using Intranet.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Intranet.Controllers
{
    [Authorize]
    public class ArticlesController : Controller
    {
        private PubSystemEntities db = new PubSystemEntities();

        public ActionResult Index(int? id = 36, int page = 1, string search = "", int id_article = 0, int skip_id_article = 0)
        {
            int pageSize = 8;

            var qarticles = db.Articles
                .Where(a => a.released && !a.del && a.complete && a.datereleased <= DateTime.Now) 
                .Where(a => search == "" || (a.title.Contains(search) || a.annotation.Contains(search)));

            if (id == -1)
                qarticles = qarticles
                    .Where(a => a.id_arttype == 4);
            else
                qarticles = qarticles
                    .Where(a => a.ArticleSections.Any(ase => ase.id_section == id));

            if (skip_id_article != 0)
            {
                qarticles = qarticles
                    .Where(a => a.id_article != skip_id_article);
            }

            qarticles = qarticles
                .OrderByDescending(a => a.datereleased)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var articles = qarticles
                .AsEnumerable()
                .Select(a => new ArticleViewModel(a)).ToList();

            if (!Request.IsAjaxRequest() && id_article != 0 && !articles.Any(a => a.IdArticle == id_article))
            {
                articles.Add(new ArticleViewModel(db.Articles.SingleOrDefault(a => a.id_article == id_article)));
                ViewBag.skip_id_article = id_article;
            }
            else
            {
                ViewBag.skip_id_article = 0;
            }

            if (search != "")
            {
                articles.ForEach(a =>
                {
                    a.Title = Regex.Replace(a.Title, search, m => { return String.Format("<mark>{0}</mark>", m.ToString()); }, RegexOptions.IgnoreCase);
                    a.Description = Regex.Replace(a.Description, search, m => { return String.Format("<mark>{0}</mark>", m.ToString()); }, RegexOptions.IgnoreCase);

                });
            }

            ObjectParameter canEdit = new ObjectParameter("canEdit", false);
            ObjectParameter canEditAll = new ObjectParameter("canEditAll", false);

            int id_user = Int32.Parse(User.Identity.GetUserId());

            db.spIntranetGetSectionRights(id_user, id, canEdit, canEditAll);

            ViewBag.CanEdit = (bool)canEdit.Value;
            ViewBag.CanEditAll = (bool)canEditAll.Value;
            ViewBag.IdUser = id_user;
            ViewBag.IdSection = id;
            ViewBag.IdArticle = id_article;

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ArticleList", articles);
            }
            else
            {

                if (id == -1)
                {
                    ViewBag.SectionName = "Aktuální informace";
                }
                else
                {
                    var section = db.Sections.SingleOrDefault(s => s.id_section == id);

                    if (section != null)
                    {
                        ViewBag.SectionName = section.name.Replace("Intranet ÚM", "Nástěnka");
                    }
                }
                return View(articles);
            }
        }

        public ActionResult Edit(int? id, int id_section)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var article = db.Articles.SingleOrDefault(a => a.id_article == id);

            ViewBag.IdSection = id_section;

            AddAuthorsTioViewBag();

            ArticleViewModel avm = new ArticleViewModel(article, false);

            SetSections(avm, article);

            return View(avm);
        }

        //http://mvccbl.com/
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id_section, [Bind(Include = "IdArticle,IdUserAuthor,Title,Description,Body,SelectedSections")] ArticleViewModel avm)
        {
            Article article = db.Articles.SingleOrDefault(a => a.id_article == avm.IdArticle);

            if (ModelState.IsValid)
            {

                article.title = avm.Title;
                article.annotation = avm.Description;

                article.Users.Clear();
                article.Users.Add(db.Users.SingleOrDefault(u => u.id_user == avm.IdUserAuthor));

                article.Chapters.First(ch => !ch.del).body = avm.Body; //!!! ostatni promazat

                if (article.id_arttype != 4)
                {
                    article.ArticleSections.Clear();
                    foreach (int idsec in avm.SelectedSections)
                    {
                        article.ArticleSections.Add(new ArticleSection
                        {
                            id_section = idsec,
                            priority = 0
                        });
                    }
                }

                db.SaveChanges();
                //return RedirectToAction("Index", new { id = id_section, id_article = article.id_article,  _t = new Random().Next(10000000) });
                return new RedirectResult(Url.Action("Index", new { id = id_section, id_article = article.id_article, _t = new Random().Next(10000000) }) + "#art" + article.id_article);
            }

            ViewBag.IdSection = id_section;

            AddAuthorsTioViewBag();
            SetSections(avm, article);

            return View(avm);
        }

        public ActionResult Create(int id_section)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}

            var article = new Article
            {
                annotation = "",
                title = ""
            };

            if (id_section > 0)
            {
                article.ArticleSections.Add(new ArticleSection { id_section = id_section, Section = db.Sections.Single(s => s.id_section == id_section) });
            }
            else
            {
                article.id_arttype = 4;
            }

            int id_user = Int32.Parse(User.Identity.GetUserId());

            User user = db.Users.Single(u => u.id_user == id_user);
            article.User = user;
            article.Users.Add(user);

            ViewBag.IdSection = id_section;

            AddAuthorsTioViewBag();

            ArticleViewModel avm = new ArticleViewModel(article, false);

            SetSections(avm, article);

            return View("Edit", avm);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int id_section, [Bind(Include = "IdArticle,IdUserAuthor,Title,Description,Body,SelectedSections")] ArticleViewModel avm)
        {
            if (ModelState.IsValid)
            {
                Article article = new Article
                {
                    annotation = avm.Description,
                    complete = true,
                    created = DateTime.Now,
                    datereleased = DateTime.Now.AddMinutes(-1),
                    datestop = DateTime.Today.AddYears(20),
                    del = false,
                    edited = DateTime.Now,
                    forum = false,
                    forumitems = 0,
                    forumlastitem = new DateTime(1900, 1, 1),
                    chaptercount = 1,
                    id_arttype = id_section < 0 ? 4 : 3,
                    id_metaarticle = 0,
                    id_server = 1,
                    id_user = Int32.Parse(User.Identity.GetUserId()),
                    imageurl = "0",
                    keywords = "",
                    redirurl = "",
                    released = true, 
                    title = avm.Title
                };
                
                article.Users.Clear();
                article.Users.Add(db.Users.SingleOrDefault(u => u.id_user == avm.IdUserAuthor));

                article.Chapters.Add(new Chapter
                {
                    body = avm.Body,
                    del = false,
                    name = "",
                    order = 1
                });

                if (article.id_arttype != 4)
                {
                    article.ArticleSections.Clear();
                    foreach (int idsec in avm.SelectedSections)
                    {
                        article.ArticleSections.Add(new ArticleSection
                        {
                            id_section = idsec,
                            priority = 0
                        });
                    }
                }

                db.Articles.Add(article);

                db.SaveChanges();

                //return RedirectToAction("Index", new { id = id_section, id_article = article.id_article, _t = new Random().Next(10000000) });
                return new RedirectResult(Url.Action("Index", new { id = id_section, id_article = article.id_article, _t = new Random().Next(10000000) }) + "#art" + article.id_article);
            }

            ViewBag.IdSection = id_section;

            AddAuthorsTioViewBag();

            var tmparticle = new Article
            {
                annotation = "",
                title = ""
            };

            if (id_section > 0)
            {
                tmparticle.ArticleSections.Add(new ArticleSection { id_section = id_section, Section = db.Sections.Single(s => s.id_section == id_section) });
            }
            else
            {
                tmparticle.id_arttype = 4;
            }

            SetSections(avm, tmparticle); 

            return View("Edit", avm);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult Delete(int id_article)
        {
            db.Articles.Single(a => a.id_article == id_article).del = true;
            db.SaveChanges();

            return Json(new object { });
        }


        private void AddAuthorsTioViewBag()
        {
            ViewBag.IdUserAuthor = new SelectList(db.Users
                .Where(u => !u.del && u.id_user != 1)
                .OrderBy(u => u.lastname)
                .Select(u => new { id_user = u.id_user, full_name = u.lastname.ToUpper() + " " + u.firstname })
                , "id_user", "full_name");
        }

        private void SetSections(ArticleViewModel avm, Article article)
        {
            int id_section_current = (int)ViewBag.IdSection;

            if (article.id_arttype != 4)
            {
                avm.Sections = article.ArticleSections.Select(ase => ase.Section).ToList();
                avm.AllSections = db.Sections.Where(s => !s.del && s.visible && (s.options & 4096) > 0).OrderBy(s => s.id_section == id_section_current  ? 0 : 1) .ThenBy(s => s.name).ToList();
            }
            else
            {
                avm.Sections = new Section[] { new Section { id_section = -1, name = "Aktuální informace" } };
                avm.AllSections = avm.Sections;
            }

            int id_user = Int32.Parse(User.Identity.GetUserId());
            var usec = db.spIntranetGetUserSections(id_user).Select(id => id.Value).ToArray();

            avm.DisabledSections = avm
                .AllSections
                .Where(s => !usec.Contains(s.id_section))
                .Select(s => s.id_section)
                .Union(new int[] { id_section_current })
                .Select(id => id.ToString())
                .ToArray();
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