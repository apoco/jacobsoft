using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Jacobsoft.Amd.Internals;

namespace Jacobsoft.Amd
{
    public static class HtmlHelperExtensions
    {
        private const string OutputContextKey = "Jacobsoft.Amd.Internals.ScriptOutputContext";

        public static IHtmlString InvokeModule(this HtmlHelper helper, string moduleId)
        {
            var stringBuilder = new StringBuilder();

            var outputContext = GetScriptOutputContext(helper.ViewContext.HttpContext);
            
            helper.RequireModuleLoader(outputContext, stringBuilder);
            helper.RequireAmdConfiguration(outputContext, stringBuilder);

            var tagBuilder = new TagBuilder("script");
            tagBuilder.InnerHtml = string.Format(
                "require({0});", 
                new JavaScriptSerializer().Serialize(new[] { moduleId }));
            stringBuilder.Append(tagBuilder.ToString());

            return new HtmlString(stringBuilder.ToString());
        }

        private static void RequireModuleLoader(
            this HtmlHelper helper,
            ScriptOutputContext context, 
            StringBuilder stringBuilder)
        {
            if (!context.IsLoaderScriptWritten)
            {
                var url = VirtualPathUtility.ToAbsolute(
                    AmdConfiguration.Current.LoaderUrl,
                    helper.ViewContext.HttpContext.Request.ApplicationPath);
                WriteScriptTag(stringBuilder, url);
                context.IsLoaderScriptWritten = true;
            }
        }

        private static void RequireAmdConfiguration(
            this HtmlHelper helper,
            ScriptOutputContext context,
            StringBuilder stringBuilder)
        {
            if (!context.IsConfigScriptWritten)
            {
                var urlHelper = new UrlHelper(
                    helper.ViewContext.RequestContext, 
                    RouteTable.Routes);
                var url = urlHelper.Action("Config", "Amd");
                WriteScriptTag(stringBuilder, url);
                context.IsConfigScriptWritten = true;
            }
        }

        private static void WriteScriptTag(
            StringBuilder stringBuilder, 
            string scriptUrl)
        {
            var scriptBuilder = new TagBuilder("script");
            scriptBuilder.Attributes["src"] = scriptUrl;
            stringBuilder.Append(scriptBuilder.ToString(TagRenderMode.Normal));
        }

        private static ScriptOutputContext GetScriptOutputContext(HttpContextBase httpContext)
        {
            if (httpContext.Items.Contains(OutputContextKey))
            {
                return httpContext.Items[OutputContextKey] as ScriptOutputContext;
            }

            var context = new ScriptOutputContext();
            httpContext.Items[OutputContextKey] = context;
            return context;
        }
    }
}
