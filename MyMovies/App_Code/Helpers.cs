using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMovies.App_Code
{
    public static class Helpers
    {
        public static IHtmlString DisplayRORating(int rating)
        {
            string result = "<div class=\"rateit\" data-rateit-value=\"" + rating + "\" data-rateit-ispreset=\"true\" data-rateit-readonly=\"true\"></div>";
            return new HtmlString(result);
        }

        public static IHtmlString DisplayRating(int rating)
        {
            string result = "<input type=\"range\" min=\"0\" max=\"10\" value=\"" + rating + "\" step=\"1\" id=\"rating\"><div class=\"rateit\" data-rateit-value=\"" + rating + "\" data-rateit-ispreset=\"true\" data-rateit-backingfld=\"#rating\"></div>";
            return new HtmlString(result);
        }

        public static IHtmlString DisplayVideo(string id)
        {
            string result = "<iframe width=\"560\" height=\"315\" src=\"" + string.Format("http://www.youtube.com/embed/{0}?rel=0", id) + "frameborder=\"0\" allowfullscreen></iframe>";
            return new HtmlString(result);
        }
    }
}