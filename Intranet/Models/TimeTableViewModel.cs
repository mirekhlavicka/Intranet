using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Intranet.Models
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
        [MaxLength(40, ErrorMessage = "Maximálně 40 znaků")]
        public String Text { get; set; }

        public int Type { get; set; }
    }
}