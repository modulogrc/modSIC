using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Modulo.Collect.TestHelper
{
    public class FileContentLoader
    {
        public byte[] GetFileContentBytes(Assembly sourceAssembly, string filename, string directoryName = "")
        {
            var stringBuilder = new StringBuilder(sourceAssembly.GetName().Name);
            if (!String.IsNullOrWhiteSpace(directoryName))
                stringBuilder.AppendFormat(".{0}", directoryName);
            stringBuilder.AppendFormat(".{0}", filename);

            var pathFile = stringBuilder.ToString();
            var certificateStream = sourceAssembly.GetManifestResourceStream(pathFile);
            return this.GetBytesFromStream(certificateStream);
        }

        private byte[] GetBytesFromStream(Stream stream)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        } 

    }
}
