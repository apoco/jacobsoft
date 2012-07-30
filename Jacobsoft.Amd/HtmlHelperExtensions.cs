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

        public static IHtmlString InvokeModule(
            this HtmlHelper helper, 
            string moduleId,
            object options = null)
        {
            var modules = new Dictionary<string, object>();
            if (options != null)
            {
                modules["options"] = options;
            }

            return helper.InvokeModule(moduleId, modules);
        }

        public static IHtmlString InvokeModule(
            this HtmlHelper helper,
            string moduleId,
            IDictionary<string, object> moduleDefinitions)
        {
            var stringBuilder = new StringBuilder();
            var jsSerializer = new JavaScriptSerializer();

            var outputContext = GetScriptOutputContext(helper.ViewContext.HttpContext);

            helper.RequireModuleLoader(outputContext, stringBuilder);
            helper.RequireAmdConfiguration(outputContext, stringBuilder);

            var scripts = string.Concat(
                moduleDefinitions
                    .Select(kvp => string.Format(
                        "define({0}, [], {1});", 
                        jsSerializer.Serialize(kvp.Key),
                        jsSerializer.Serialize(kvp.Value)))
                    .Concat(new[] {
                        string.Format(
                            "require({0});",
                            jsSerializer.Serialize(new[] { moduleId } ) ) }));

            var tagBuilder = new TagBuilder("script");
            tagBuilder.InnerHtml = scripts;
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
