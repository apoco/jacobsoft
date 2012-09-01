using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.UI;
using Jacobsoft.Amd.Config;
using Jacobsoft.Amd.Internals;

namespace Jacobsoft.Amd
{
    public class AmdController : Controller
    {
        private const int ScriptCacheDuration = 60 * 60 * 24 * 30;

        private static ServiceLocator factory = new ServiceLocator();

        private readonly IAmdConfiguration config;
        private readonly IModuleResolver resolver;
        private readonly IFileSystem fileSystem;

        public AmdController() : this(
            ServiceLocator.Instance.Get<IAmdConfiguration>(),
            ServiceLocator.Instance.Get<IModuleResolver>(),
            ServiceLocator.Instance.Get<IFileSystem>())
        {
        }

        internal AmdController(
            IAmdConfiguration configuration,
            IModuleResolver resolver,
            IFileSystem fileSystem)
        {
            this.config = configuration;
            this.resolver = resolver;
            this.fileSystem = fileSystem;
        }

        /// <summary>
        /// Registers preferred routes for the AmdController
        /// </summary>
        /// <param name="routes">Route collection to augment</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "Jacobsoft.Amd",
                "Amd/{action}/{*id}",
                new { controller = "Amd" });
        }

        [HttpGet]
        [OutputCache(
            Location = OutputCacheLocation.ServerAndClient,
            Duration = ScriptCacheDuration,
            VaryByParam = "*")]
        public JavaScriptResult Loader([Bind(Prefix = "v")] string version = null)
        {
            return this.MinifiedJavaScript(this.fileSystem.ReadToEnd(
                this.HttpContext
                    .Server
                    .MapPath(this.config.LoaderUrl)));
        }

        [HttpGet]
        [OutputCache(
            Location = OutputCacheLocation.ServerAndClient,
            Duration = ScriptCacheDuration,
            VaryByParam = "*")]
        public JavaScriptResult LiteLoader([Bind(Prefix = "v")] string version = null)
        {
            return this.MinifiedJavaScript(
                this.GetType()
                    .Assembly
                    .GetManifestResourceStream("Jacobsoft.Amd.Scripts.liteloader.js")
                    .ReadToEnd());
        }

        [HttpGet]
        [OutputCache(
            Location = OutputCacheLocation.ServerAndClient, 
            Duration = ScriptCacheDuration,
            VaryByParam = "*")]
        public JavaScriptResult Config([Bind(Prefix="v")] string version = null)
        {
            var baseUrl = VirtualPathUtility.ToAbsolute(
                this.config.ModuleRootUrl ?? "~/Scripts",
                this.Request.ApplicationPath);
            return this.MinifiedJavaScript(string.Format(
                "require.config({0});",
                new JavaScriptSerializer().Serialize(
                    new
                    {
                        baseUrl = baseUrl,
                        shim = this.config.Shims.OrEmpty().Values.ToDictionary(
                            s => s.Id, 
                            s => new { deps = s.Dependencies, exports = s.Export })
                    }
                )
            ));
        }

        [HttpGet]
        [OutputCache(
            Location = OutputCacheLocation.ServerAndClient, 
            Duration = ScriptCacheDuration,
            VaryByParam = "*")]
        public JavaScriptResult Module(
            string id, 
            [Bind(Prefix = "v")] string version = null)
        {
            return this.MinifiedJavaScript(this.resolver.Resolve(id).Content);
        }

        /// <summary>
        /// Gets a concatenated bundle of scripts.
        /// </summary>
        /// <param name="id">A list of module IDs. Separate modules with spaces, commas, or plus signs.</param>
        /// <returns>A JavaScriptResult</returns>
        [HttpGet]
        [OutputCache(
            Location = OutputCacheLocation.ServerAndClient, 
            Duration = ScriptCacheDuration,
            VaryByParam = "*")]
        public JavaScriptResult Bundle(
            string id, 
            [Bind(Prefix = "v")] string version = null)
        {
            return this.MinifiedJavaScript(string.Join(
                ";", 
                from moduleId in id.Split(new[] { '+', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                let module = this.resolver.Resolve(moduleId)
                where module != null
                select module.Content
            ));
        }

        private JavaScriptResult MinifiedJavaScript(string content)
        {
            if (this.config.Minifier != null)
            {
                content = this.config.Minifier.Minify(content);
            }

            return this.JavaScript(content);
        }
    }
}
