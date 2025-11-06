using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vizsga1;

namespace UMFST.MIP.Bookstore // ÁTÍRJUK A NÉVTERET
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow()); // ÁTÍRJUK Form1-ről MainWindow-ra
        }
    }
}
