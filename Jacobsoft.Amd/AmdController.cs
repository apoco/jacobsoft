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
        [OutputCache(Location = OutputCacheLocation.ServerAndClient, Duration = ScriptCacheDuration)]
        public FileStreamResult Loader()
        {
            return this.File(
                this.fileSystem.Open(this.config.LoaderUrl, FileMode.Open), 
                "text/javascript");
        }

        [HttpGet]
        [OutputCache(Location = OutputCacheLocation.ServerAndClient, Duration = ScriptCacheDuration)]
        public FileResult LiteLoader()
        {
            return this.File(
                this.GetType().Assembly.GetManifestResourceStream("Jacobsoft.Amd.Scripts.liteloader.js"),
                "text/javascript");
        }

        [HttpGet]
        [OutputCache(Location = OutputCacheLocation.ServerAndClient, Duration = ScriptCacheDuration)]
        public ContentResult Config()
        {
            var baseUrl = VirtualPathUtility.ToAbsolute(
                this.config.ModuleRootUrl,
                this.Request.ApplicationPath);
            return this.Content(
                string.Format(
                    "require.config({0});",
                    new JavaScriptSerializer().Serialize(new { baseUrl = baseUrl })),
                "text/javascript");
        }

        [HttpGet]
        [OutputCache(Location = OutputCacheLocation.ServerAndClient, Duration = ScriptCacheDuration)]
        public ContentResult Module(string id)
        {
            return this.Content(
                this.resolver.Resolve(id).Content, 
                "text/javascript");
        }

        /// <summary>
        /// Gets a concatenated bundle of scripts.
        /// </summary>
        /// <param name="id">A list of module IDs. Separate modules with spaces, commas, or plus signs.</param>
        /// <returns>A JavaScriptResult</returns>
        [HttpGet]
        [OutputCache(Location = OutputCacheLocation.ServerAndClient, Duration = ScriptCacheDuration)]
        public JavaScriptResult Bundle(string id)
        {
            return this.JavaScript(string.Join(
                ";", 
                from moduleId in id.Split(new[] { '+', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                let module = this.resolver.Resolve(moduleId)
                where module != null
                select module.Content
            ));
        }
    }
}
