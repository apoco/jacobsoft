using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Jacobsoft.Amd.Internals
{
    internal static class FileSystemExtensions
    {
        public static string ReadToEnd(this IFileSystem fileSystem, string filePath)
        {
            using (var stream = fileSystem.Open(filePath))
            {
                return stream.ReadToEnd();
            }
        }

        public static string ReadToEnd(this Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
