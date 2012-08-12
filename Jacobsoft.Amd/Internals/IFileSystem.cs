using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Internals
{
    internal interface IFileSystem
    {
        Stream Open(
            string loaderPath, 
            FileMode fileMode = FileMode.Open, 
            FileAccess fileAccess = FileAccess.Read,
            FileShare fileShare = FileShare.ReadWrite);

        bool FileExists(string filePath);
    }
}
