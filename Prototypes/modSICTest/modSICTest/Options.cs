using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace modSICTest
{
    class Options
    {
        #region Standard Option Attribute
        [Option("t", "teste",
                Required = false,
                HelpText = "modo de teste")]
        public bool testMode = false;
        #endregion

        #region Specialized Option Attribute
        [ValueList(typeof(List<string>), MaximumElements = 0)]
        public IList<string> Items = null;

        [HelpOption(
                HelpText = "Exibe as opções do programa.")]
        public string GetUsage()
        {
            var programName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            HeadingInfo _headingInfo = new HeadingInfo(programName, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            var help = new HelpText(_headingInfo);
            help.AdditionalNewLineAfterOption = true;
            help.Copyright = new CopyrightInfo("Modulo Security Solutions", 2011, 2011);
            help.AddPreOptionsLine("This is free software. You may redistribute copies of it under the terms of");
            help.AddPreOptionsLine("the License <http://modsic.codeplex.com/license>.");
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine(String.Format("Usage: {0}", programName));
            help.AddOptions(this);

            return help;
        }
        #endregion
    }
}
