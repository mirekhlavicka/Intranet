using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Intranet.Models
{
    public class UserHomePageViewModel
    {
        public User User { get; set; }

        public string CurrentBody { get; set; }

        public UserPage CurrentPage { get; set; }

        public int CurrentPageId { get; set; }

        public string CurrentPageUrl { get; set; }

        public IEnumerable<UserPage> UserPages { get; set; }

        [Display(Name = "Zobrazovat hlavičku na úvodní stánce")]
        public bool ShowHeader { get; set; }

        [Display(Name = "Zobrazovat fotografii v hlavičce")]
        public bool ShowPhoto { get; set; }

        [Display(Name = "Použít téma")]
        public string BootstrapTheme { get; set; }

        [Display(Name = "Celé jméno")]
        public string FullName { get; set; }

        [Display(Name = "Telefon")]
        public string Phone { get; set; }

        [Display(Name = "Místnost")]
        public string Room { get; set; }

    }
}