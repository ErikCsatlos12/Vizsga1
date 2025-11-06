using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

// A NÉVTÉRNEK EZ KELL LEGYEN:
namespace UMFST.MIP.Bookstore
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // AZ INDÍTÓ ABLAKNAK EZ KELL LEGYEN:
            Application.Run(new MainWindow());
        }
    }
}