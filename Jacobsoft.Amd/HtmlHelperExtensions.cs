using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Jacobsoft.Amd.Config;
using Jacobsoft.Amd.Exceptions;
using Jacobsoft.Amd.Internals;

namespace Jacobsoft.Amd
{
    public static class HtmlHelperExtensions
    {
        private const string OutputContextKey = "Jacobsoft.Amd.Internals.ScriptOutputContext";

        public static IHtmlString ModuleInvoke(
            this HtmlHelper helper, 
            string moduleId,
            object options = null)
        {
            var modules = new Dictionary<string, object>();
            if (options != null)
            {
                modules["options"] = options;
            }

            return helper.ModuleInvoke(moduleId, modules);
        }

        public static IHtmlString ModuleInvoke(
            this HtmlHelper helper,
            string moduleId,
            IDictionary<string, object> moduleDefinitions)
        {
            var stringBuilder = new StringBuilder();
            var jsSerializer = new JavaScriptSerializer();

            var outputContext = GetScriptOutputContext(helper.ViewContext.HttpContext);

            helper.EnsureAmdSystemInitialized(outputContext, stringBuilder);

            if (outputContext.ScriptLoadingMode == ScriptLoadingMode.Static)
            {
                helper.RequireModuleIncludeRecursive(
                    moduleId, 
                    outputContext, 
                    stringBuilder);
            }

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

        public static IHtmlString ModuleBundle(
            this HtmlHelper helper, 
            params string[] moduleIds)
        {
            var outputContext = GetScriptOutputContext(helper.ViewContext.HttpContext);
            var stringBuilder = new StringBuilder();
            var resolver = ServiceLocator.Instance.Get<IModuleResolver>();

            helper.EnsureAmdSystemInitialized(outputContext, stringBuilder);
            
            var requiredModuleIds =
                from rootId in moduleIds
                from module in resolver.Resolve(rootId).TreeNodes(m => m.Dependencies)
                let moduleId = module.Id
                where !outputContext.WrittenModules.Contains(moduleId)
                select moduleId;
            helper.WriteScriptActionInclude(
                "Bundle", 
                string.Join(",", requiredModuleIds.Distinct().OrderBy(id => id)), 
                stringBuilder);

            outputContext.WrittenModules.UnionWith(requiredModuleIds);

            return new HtmlString(stringBuilder.ToString());
        }

        private static void EnsureAmdSystemInitialized(
            this HtmlHelper helper,
            ScriptOutputContext outputContext,
            StringBuilder stringBuilder)
        {
            helper.RequireModuleLoader(outputContext, stringBuilder);
            helper.RequireAmdConfiguration(outputContext, stringBuilder);
        }

        private static void RequireModuleLoader(
            this HtmlHelper helper,
            ScriptOutputContext context, 
            StringBuilder stringBuilder)
        {
            if (!context.IsLoaderScriptWritten)
            {
                string action;
                var config = AmdConfiguration.Current;
                string loaderUrl;

                if (config.ScriptLoadingMode == ScriptLoadingMode.Dynamic)
                {
                    if (config.LoaderUrl == null)
                    {
                        throw new AmdConfigurationException("Dynamic script loading requires a third-party script loader.");
                    }
                    else
                    {
                        action = "Loader";
                    }
                }
                else
                {
                    action = "LiteLoader";
                }

                helper.WriteScriptActionInclude(action, stringBuilder, false);
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
                helper.WriteScriptActionInclude("config", stringBuilder, false);
                context.IsConfigScriptWritten = true;
            }
        }

        private static void RequireModuleIncludeRecursive(
            this HtmlHelper helper, 
            string moduleId,
            ScriptOutputContext context,
            StringBuilder stringBuilder)
        {
            if (!context.WrittenModules.Contains(moduleId))
            {
                context.WrittenModules.Add(moduleId);

                var module = context.ResolveModule(moduleId);
                if (module != null)
                {
                    foreach (var dependency 
                        in module.Dependencies.OrEmpty().Where(m => m.Content != null))
                    {
                        helper.RequireModuleIncludeRecursive(
                            dependency.Id,
                            context,
                            stringBuilder);
                    }
                }

                helper.WriteScriptActionInclude("Module", moduleId, stringBuilder);
            }
        }

        private static void WriteScriptActionInclude(
            this HtmlHelper helper,
            string action,
            StringBuilder stringBuilder,
            bool defer = true)
        {
            helper.WriteScriptActionInclude(action, null, stringBuilder, defer);
        }

        private static void WriteScriptActionInclude(
            this HtmlHelper helper, 
            string action,
            string id, 
            StringBuilder stringBuilder,
            bool defer = true)
        {
            var urlHelper = helper.GetUrlHelper();
            var url = urlHelper.Action(
                action, 
                "Amd", 
                new { 
                    id = id, 
                    v = AmdConfiguration.Current.VersionProvider.GetVersion() 
                });
            WriteScriptTag(stringBuilder, url, defer);
        }

        private static void WriteScriptTag(
            StringBuilder stringBuilder, 
            string scriptUrl,
            bool defer = false)
        {
            var scriptBuilder = new TagBuilder("script");
            scriptBuilder.Attributes["src"] = scriptUrl;

            if (defer)
            {
                scriptBuilder.Attributes["defer"] = "defer";
                scriptBuilder.Attributes["async"] = "async";
            }

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

        private static UrlHelper GetUrlHelper(this HtmlHelper helper)
        {
            var urlHelper = new UrlHelper(
                helper.ViewContext.RequestContext,
                RouteTable.Routes);
            return urlHelper;
        }
    }
}
