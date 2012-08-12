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
        public Stream Open(
            string loaderPath, 
            FileMode fileMode = FileMode.Open, 
            FileAccess fileAccess = FileAccess.Read,
            FileShare fileShare = FileShare.ReadWrite)
        {
            return File.Open(loaderPath, fileMode, fileAccess, fileShare);
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
