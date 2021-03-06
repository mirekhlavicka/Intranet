﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using IntranetPublic.Utils;

namespace IntranetPublic.Controllers
{
    public class StaffController : Controller
    {
        private PubSystemEntities db = new PubSystemEntities();

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index(string search = "", string lang = "cz")
        {
            //var staff = db.Staff.AsQueryable();

            //ViewBag.departments = db.Departments.ToList();

            //return View(staff.ToList());

            var staff = db.Staff.AsEnumerable();

            if (!String.IsNullOrEmpty(search))
            {
                staff = staff.Where(s =>
                {
                    bool res = false;
                    s.JMENO = Regex.Replace(s.JMENO, search.ToAccentInsensitiveRegex(), m => { res = true; return String.Format("<mark>{0}</mark>", m.ToString()); }, RegexOptions.IgnoreCase);
                    if((s.PRIJMENI + " " + s.KRESTNI).ToLower().StartsWith(search.ToLower()))
                    {
                        res = true;
                    }
                    return res;
                });
            }

            ViewBag.lang = lang;
            ViewBag.departments = db.Departments.ToList();
            ViewBag.tbSearch = search;

            if (Request.IsAjaxRequest())
            {
                return PartialView("_StaffList", staff.ToList());
            }
            else
            {
                return View(staff.ToList());
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