using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Practices.Unity;
using Quartz;
using Quartz.Impl;

using System.Configuration;


namespace ModuloOvalInterpreter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var MOVI_LOG = "c:\\movilog.log";
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain(MOVI_LOG));
        }
    }
}
