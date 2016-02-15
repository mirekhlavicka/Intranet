using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Intranet.Models
{
    public class FileViewModel
    {
        public int Id { get; set; }

        public bool Image { get; set; }

        public bool Folder { get; set; }

        public DateTime UploadDate { get; set; }

        public string URL { get; set; }

        public string Thumbnail { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public bool CanDelete { get; set; }

        public bool NewItem { get; set; }

        public bool ClipboardItem { get; set; }
    }
}