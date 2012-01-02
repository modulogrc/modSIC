using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace ModSicApiTests
{
    public class ResourceLoader
    {
        public String GetDocumentContents(string resourceName)
        {
            var documentAsStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            return new StreamReader(documentAsStream).ReadToEnd();
        }
    }
}
