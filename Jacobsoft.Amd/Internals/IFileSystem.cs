using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Internals
{
    internal interface IFileSystem
    {
        Stream Open(string filePath, FileMode fileMode);

        bool FileExists(string filePath);
    }
}
