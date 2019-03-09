using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RMERP.Helpers
{
    public static class ImageHelper 
    {
        public static IHtmlContent HelloWorldHTMLString(this IHtmlHelper htmlHelper)
           => new HtmlString("<strong>Hello World</strong>");

        public static IHtmlContent Image(this IHtmlHelper helper, string src, string altText, string title = "image", string Id = "ImageHelper", string Name = "ImageHelper", string classname = "image-helper", string width = "100", string height = "100")
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", src);
            builder.MergeAttribute("title", title);
            builder.MergeAttribute("alt", altText);
            builder.MergeAttribute("width", width);
            builder.MergeAttribute("height", height);
            builder.MergeAttribute("Id", Id);
            builder.MergeAttribute("Name", Name);
            builder.MergeAttribute("class", classname);
            return builder;
        }
    }
}
