using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace IntranetPublic.Utils
{
    public static class MyExtensions
    {
        public static string MakeActiveClass(this UrlHelper urlHelper, string controller, int section = -1)
        {
            string result = "active";

            string controllerName = urlHelper.RequestContext.RouteData.Values["controller"].ToString();

            if (!controllerName.Equals(controller, StringComparison.OrdinalIgnoreCase))
            {
                result = null;
            }
            else if (section != -1 && (urlHelper.RequestContext.RouteData.Values["section"] ?? 0).ToString() != section.ToString())
            {
                result = null;
            }

            return result;
        }

        // As the text the: "<span class='glyphicon glyphicon-plus'></span>" can be entered
        public static MvcHtmlString NoEncodeActionLink(this HtmlHelper htmlHelper,
                                             string text, string title, string action,
                                             string controller,
                                             object routeValues = null,
                                             object htmlAttributes = null)
        {
            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            TagBuilder builder = new TagBuilder("a");
            builder.InnerHtml = text;
            builder.Attributes["title"] = title;
            builder.Attributes["href"] = urlHelper.Action(action, controller, routeValues);
            builder.MergeAttributes(new RouteValueDictionary(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)));

            return MvcHtmlString.Create(builder.ToString());
        }

        public static string ToAccentInsensitiveRegex(this string s)
        {
            return s
                .Replace("e", "[eéě]")
                .Replace("s", "[sš]")
                .Replace("c", "[cč]")
                .Replace("r", "[rř]")
                .Replace("z", "[zž]")
                .Replace("y", "[yý]")
                .Replace("a", "[aáä]")
                .Replace("i", "[ií]")
                .Replace("u", "[uúůü]")
                .Replace("n", "[nň]")
                .Replace("o", "[oóö]")
                .Replace("d", "[dď]")
                .Replace("t", "[tť]");
        }
    }
}