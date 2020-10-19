using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Intranet.Models
{
    public class ArticleViewModel
    {

        public ArticleViewModel()
        { }

        public ArticleViewModel(Article a, bool removeImp = true )
        {
            IdArticle = a.id_article;
            IdUser = a.id_user;
            IdUserAuthor = (a.Users.FirstOrDefault() ?? new User { id_user = 0 }).id_user;
            Title = removeImp ? a.title.Trim('!', ' ') : a.title;
            Description = a.annotation;
            Date = a.datereleased;
            Author = (a.Users.FirstOrDefault() ?? new Intranet.User { full_name = "" }).full_name;
            Body = String.Concat(a.Chapters.Where(ch => !ch.del).OrderBy(ch => ch.order).Select(ch => ch.body));
            Important = a.title.StartsWith("!");
        }

        public int IdArticle { get; set; }

        public int IdUser { get; set; }

        [Display(Name = "Autor")]
        public int IdUserAuthor { get; set; }

        [Required(ErrorMessage = "Musíte zadat titulek")]
        [Display(Name = "Titulek")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Musíte zadat popis")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Popis")]
        public string Description { get; set; }

        public DateTime Date { get; set; }

        public string Author { get; set; }

        [Required(ErrorMessage = "Musíte zadat text zprávy")]
        [Display(Name = "Text zprávy")]
        public string Body { get; set; }

        public bool Important { get; set; }

        [Display(Name = "Zařadit do")]        
        public IEnumerable<Section> Sections { get; set; }

        public IEnumerable<Section> AllSections { get; set; }

        [Required(ErrorMessage = "Musíte zvolit alespoň jednu sekci")]
        public int[] SelectedSections { get; set; }

        public string[] DisabledSections { get; set; }

    }
}