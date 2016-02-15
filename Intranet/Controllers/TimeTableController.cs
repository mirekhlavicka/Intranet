using Intranet.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Intranet.Controllers
{
    //[Authorize]
    public class TimeTableController : Controller
    {
        private PubSystemEntities db = new PubSystemEntities();

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index()
        {
            int id_item_user = GetIdItemUser();

            if (id_item_user == 0)
            {
                return RedirectToAction("Login", "Account", new { ReturnUrl = "/TimeTable" });
            }

            ViewBag.UserFullName = db.FormItemFields.SingleOrDefault(fif => fif.id_item == id_item_user && fif.id_fcontrol == 2).strvalue;

            TimeTableViewModel[] days = new TimeTableViewModel[5];

            for (int day = 1; day <= 5; day++)
            {
                days[day - 1] = GetDay(id_item_user, day);
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_DayList", days);
            }
            else
            {
                ViewBag.TimeTableUrl = GetTimeTableURL();
                return View(days);
            }
        }

        public ActionResult Edit(int? day)
        {
            if (day == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int id_item_user = GetIdItemUser();

            if (id_item_user == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            return PartialView("_Edit", GetDay(id_item_user, day.Value));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Day,Hours")] TimeTableViewModel timeTable)
        {
            if (ModelState.IsValid)
            {
                int id_item_user = GetIdItemUser();

                if (id_item_user == 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                string sid_item_user = id_item_user.ToString();
                string sday = timeTable.Day.ToString();

                FormItem dayItem = db.FormItems
                    .SingleOrDefault(fi => fi.FormItemFields.Any(fif => fif.id_fcontrol == 11 && fif.strvalue == sid_item_user) &&
                                           fi.FormItemFields.Any(fif => fif.id_fcontrol == 13 && fif.strvalue == sday));

                if (dayItem == null)
                {
                    dayItem = new FormItem
                    {
                        createdate = DateTime.Now,
                        editdate = DateTime.Now,
                        id_form = 2,
                        id_group = 0,
                        id_item_version = 0,
                        id_user = 0,
                        released = true,
                        saved = true,
                        version = 0
                    };

                    db.FormItems.Add(dayItem);

                    dayItem.FormItemFields.Add(new FormItemField
                    {
                        id_fcontrol = 11,
                        id_form = 2,
                        datetype = 0,
                        datevalue = new DateTime(1900, 1, 1),
                        intvalue = 0,
                        moneyvalue = 0,
                        numvalue = 0,
                        richvalue = "",
                        strvalue = sid_item_user
                    });

                    dayItem.FormItemFields.Add(new FormItemField
                    {
                        id_fcontrol = 13,
                        id_form = 2,
                        datetype = 0,
                        datevalue = new DateTime(1900, 1, 1),
                        intvalue = 0,
                        moneyvalue = 0,
                        numvalue = 0,
                        richvalue = "",
                        strvalue = sday
                    });
                }


                for (int hour = 0; hour < timeTable.Hours.Length; hour++)
                {
                    FormItemField hourFif = dayItem.FormItemFields.SingleOrDefault(fif => fif.id_fcontrol == 14 + hour);

                    if (hourFif == null)
                    {
                        hourFif = new FormItemField
                        {
                            id_fcontrol = 14 + hour,
                            id_form = 2,
                            datetype = 0,
                            datevalue = new DateTime(1900, 1, 1),
                            intvalue = 0,
                            moneyvalue = 0,
                            numvalue = 0,
                            richvalue = "",
                            strvalue = ""
                        };

                        dayItem.FormItemFields.Add(hourFif);
                    }

                    if (String.IsNullOrEmpty(timeTable.Hours[hour].Text))
                    {
                        hourFif.strvalue = "&nbsp;";
                    }
                    else
                    {
                        hourFif.strvalue = String.Format("{0}|{1}", timeTable.Hours[hour].Text, timeTable.Hours[hour].Type);
                    }
                }


                db.SaveChanges();

                if (User.Identity.IsAuthenticated)
                {
                    return Json(new { success = true, url = Url.Action("Index") });
                }
                else
                {
                    return Json(new { success = true, url = Url.Action("Index", new { uid = Request.QueryString["uid"], pin = Request.QueryString["pin"] }) });
                }
            }

            return PartialView("_Edit", timeTable);
        }



        private int GetIdItemUser()
        {
            if (User.Identity.IsAuthenticated)
            {

                var userId = User.Identity.GetUserId();
                var user = db.Users.SingleOrDefault(u => u.id_user.ToString() == userId);

                return db.FormItemFields.SingleOrDefault(fif => fif.id_fcontrol == 3 && fif.strvalue == user.email).id_item;
            }
            else
            {
                int id = 0;
                int pin = 0;
                Int32.TryParse(Request.QueryString["uid"], out id);
                Int32.TryParse(Request.QueryString["pin"], out pin);

                if (((1357823 + 317651 * (long)id) % 11177711) != pin)
                    id = 0;

                return id;
            }
        }

        private TimeTableViewModel GetDay(int id_item_user, int day)
        {
            TimeTableViewModel timeTableViewModel = new TimeTableViewModel();

            string sid_item_user = id_item_user.ToString();
            string sday = day.ToString();

            timeTableViewModel.Day = day;

            var item = db.FormItems
                .SingleOrDefault(fi => fi.FormItemFields.Any(fif => fif.id_fcontrol == 11 && fif.strvalue == sid_item_user) &&
                                       fi.FormItemFields.Any(fif => fif.id_fcontrol == 13 && fif.strvalue == sday));
            int id_item = 0;

            if (item != null)
            {
                id_item = item.id_item;
            }

            if (id_item != 0)
            {
                timeTableViewModel.Hours = db.FormItemFields
                    .Where(fif => fif.id_item == id_item && fif.id_fcontrol >= 14 && fif.id_fcontrol <= 27)
                    .OrderBy(fif => fif.id_fcontrol)
                    .AsEnumerable()
                    .Select((fif, h) =>
                    {
                        string val = fif.strvalue.Replace("&nbsp;", "");
                        string[] tmp = val.Split('|');

                        if (tmp.Length == 2)
                        {
                            return new TimeTableHour { Text = tmp[0], Type = Int32.Parse(tmp[1]) };
                        }
                        else
                        {
                            return new TimeTableHour { Text = val, Type = (h > 1 && h < 7 ? 4 : 0) };
                        }
                    })
                    .ToArray();
            }

            if(timeTableViewModel.Hours == null || timeTableViewModel.Hours.Length == 0)
            {
                timeTableViewModel.Hours = Enumerable.Range(0, 14).Select(h => new TimeTableHour { Text = "", Type = (h > 1 && h < 7 ? 4 : 0) }).ToArray();
            }

            return timeTableViewModel;
        }

        private string GetTimeTableURL()
        {
            if (User.Identity.IsAuthenticated)
            {

                var userId = User.Identity.GetUserId();
                var user = db.Users.SingleOrDefault(u => u.id_user.ToString() == userId);
                return Intranet.Properties.Settings.Default.BaseURL + "/" + Intranet.Properties.Settings.Default.HomePath + "/" + user.login_name.ToLower() + "/rozvrh";
            }
            else
            {
                return Intranet.Properties.Settings.Default.BaseURL + "/" + Intranet.Properties.Settings.Default.HomePath + "/timetable/" + GetIdItemUser();
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