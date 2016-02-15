using IntranetPublic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntranetPublic.Controllers
{
    public class TimeTableController : Controller
    {
        private PubSystemEntities db = new PubSystemEntities();

        // GET: TimeTable
        public ActionResult Index(int? uid)
        {
            int id_item_user = uid ?? 0;

            if(id_item_user != 0)
                ViewBag.UserFullName = db.FormItemFields.SingleOrDefault(fif => fif.id_item == id_item_user && fif.id_fcontrol == 2).strvalue;
            else
                ViewBag.UserFullName = "";

            TimeTableViewModel[] days = new TimeTableViewModel[5];

            for (int day = 1; day <= 5; day++)
            {
                days[day - 1] = GetDay(id_item_user, day);
            }            

            if (ControllerContext.IsChildAction)
            {
                return PartialView(days);
            }
            else
            {
                ViewBag.ddlUser = new SelectList(db.Staff.OrderBy(s => s.PRIJMENI), "id_item", "Jmeno", uid);
                return View(days);
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

            if (timeTableViewModel.Hours == null || timeTableViewModel.Hours.Length == 0)
            {
                timeTableViewModel.Hours = Enumerable.Range(0, 14).Select(h => new TimeTableHour { Text = "", Type = (h > 1 && h < 7 ? 4 : 0) }).ToArray();
            }

            return timeTableViewModel;
        }
    }
}