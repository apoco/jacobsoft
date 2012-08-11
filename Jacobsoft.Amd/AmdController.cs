using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
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
    }
}
