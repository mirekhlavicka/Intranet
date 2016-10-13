using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IntranetPublic.Models
{
    public class CourseViewModel
    {
        public String Name { get; set; }

        public String NameEng { get; set; }

        public String Code { get; set; }

        public String CodeSearch { get; set; }

        public String Semester { get; set; }

        public String CVISID { get; set; }

        public CourseViewModel()
        {

        }

        public CourseViewModel(Cours cours)
        {
            Name = cours.name;
            NameEng = cours.name_eng;
            Code = cours.code.ToUpper();
            CodeSearch = Code;
            Semester = cours.semester;
            CVISID = cours.cvis_id;
        }

    }
}