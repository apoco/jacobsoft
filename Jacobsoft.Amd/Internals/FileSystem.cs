using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Internals
{
    [ExcludeFromCodeCoverage]
    public class FileSystem : IFileSystem
    {
        public Stream Open(string loaderPath, FileMode fileMode)
        {
            return File.Open(loaderPath, fileMode);
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
