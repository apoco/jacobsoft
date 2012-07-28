using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using Jacobsoft.Amd.Internals;

namespace Jacobsoft.Amd
{
    public class AmdController : Controller
    {
        private static ObjectFactory factory = new ObjectFactory();

        private readonly IAmdConfiguration config;
        private readonly IModuleResolver resolver;
        private readonly IFileSystem fileSystem;

        public AmdController() : this(
            AmdSystem.Configuration,
            factory.GetModuleResolver(),
            factory.GetFileSystem())
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
        [OutputCache(Location = OutputCacheLocation.ServerAndClient)]
        public FileStreamResult GetLoader()
        {
            return this.File(
                this.fileSystem.Open(this.config.LoaderFilePath, FileMode.Open), 
                "text/javascript");
        }

        [HttpGet]
        [OutputCache(Location = OutputCacheLocation.ServerAndClient)]
        public ContentResult GetModule(string moduleName)
        {
            return this.Content(
                this.resolver.Resolve(moduleName).Content, 
                "text/javascript");
        }
    }
}
