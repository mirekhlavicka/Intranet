using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Intranet.Models
{
    public class DownloadViewModel
    {
        public DownloadViewModel()
        { }

        public DownloadViewModel(Download d, string course_code_name = "")
        {
            course_code = d.course_code;
            this.course_code_name = course_code_name;
            date = d.date.Value;
            description = d.description;
            File = d.File;
            id_file = d.id_file;
            id_item = d.id_item;
            id_user_item = d.id_user.Value;
            title = d.title;
        }

        public int id_item { get; set; }
        public int id_user_item { get; set; }

        [Required(ErrorMessage = "Musíte zadat název")]
        [Display(Name = "Název")]
        public string title { get; set; }

        [Required(ErrorMessage = "Musíte zadat popis")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Popis")]
        public string description { get; set; }

        [Display(Name = "Soubor")]
        [Required(ErrorMessage = "Musíte vybrat soubor")]
        public int id_file { get; set; }

        [Display(Name = "Datum vložení")]
        public DateTime date { get; set; }

        [Required(ErrorMessage = "Musíte zadat kurz")]
        [Display(Name = "Určeno pro kurz")]
        public string course_code { get; set; }

        [Display(Name = "Určeno pro kurz")]
        public string course_code_name { get; set; }

        public File File { get; set; }

        public string ThumbNailPath
        {
            get
            {

                if (File == null)
                {
                    return "";
                }
                else if (File.image)
                {
                    return String.Format("{0}/getthumbnail.aspx?id_file={1}&width=128&height=128&q=90", Intranet.Properties.Settings.Default.BaseURL, File.id_file);
                }
                else if (File.fileext.ToLower() == ".pdf")
                {
                    return "/Content/Img/Small/Adobe-PDF-Document-icon.png";
                }
                else if (File.fileext.ToLower() == ".zip")
                {
                    return "/Content/Img/Small/folder-zip-icon.png";
                }
                else if (File.fileext.ToLower() == ".html" || File.fileext.ToLower() == ".htm")
                {
                    return "/Content/Img/Small/File-Adobe-Dreamweaver-HTML-01-icon.png";
                }
                else if (File.fileext.ToLower() == ".doc" || File.fileext.ToLower() == ".docx")
                {
                    return "/Content/Img/Small/Document-Microsoft-Word-icon.png";
                }
                else if (File.fileext.ToLower() == ".xls" || File.fileext.ToLower() == ".xlsx")
                {
                    return "/Content/Img/Small/Document-Microsoft-Excel-icon.png";
                }
                else if (File.fileext.ToLower() == ".ppt" || File.fileext.ToLower() == ".pptx" || File.fileext.ToLower() == ".pps")
                {
                    return "/Content/Img/Small/Document-Microsoft-PowerPoint-icon.png";
                }
                else
                {
                    return "/Content/Img/Small/Document-icon.png";
                }
            }
        }
    }
}