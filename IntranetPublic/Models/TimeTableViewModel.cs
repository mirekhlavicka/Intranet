using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntranetPublic.Models
{
    public class TimeTableViewModel
    {
        public TimeTableViewModel()
        { }

        public int Day { get; set; }

        public TimeTableHour[] Hours { get; set; }

    }

    public class TimeTableHour
    {
        public String Text { get; set; }

        public int Type { get; set; }
    }
}