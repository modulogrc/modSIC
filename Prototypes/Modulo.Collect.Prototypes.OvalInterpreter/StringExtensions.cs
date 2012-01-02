using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace ModuloOvalInterpreter
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns the original string for a compressed base64 encoded string
        /// </summary>
        public static string ToUncompressedString(this string source)
        {
            // get the byte array representation for the compressed string
            var compressedBytes = Convert.FromBase64String(source);
            // load the byte array into a memory stream
            using (var inMemStream = new MemoryStream(compressedBytes))
                // and decompress the memory stream into the original string
                using (var decompressionStream = new DeflateStream(inMemStream, CompressionMode.Decompress))
                    using (var streamReader = new StreamReader(decompressionStream))
                        return streamReader.ReadToEnd();
        }
    }
}
