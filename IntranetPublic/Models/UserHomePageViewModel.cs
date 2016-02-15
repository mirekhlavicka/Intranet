using System.Collections.Generic;

namespace IntranetPublic.Models
{
    public class UserHomePageViewModel
    {
        public User User { get; set; }

        public int IdItem { get; set; }

        public string CurrentBody { get; set; }

        public int CurrentPageId { get; set; }

        public string CurrentPageTitle { get; set; }

        public IEnumerable<UserPage> UserPages { get; set; }

        public StaffItem StaffInfo { get; set; }
    }
}