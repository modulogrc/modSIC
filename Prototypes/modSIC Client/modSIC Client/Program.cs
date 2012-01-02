using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace modSIC_Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainView view = new MainView();
            MainModel model = new MainModel();

            MainPresenter presenter = new MainPresenter(view, model);

            Application.Run(view);
        }
    }
}
