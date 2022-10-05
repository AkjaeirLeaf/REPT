using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace REPT
{
    
    class Program
    {
        static void Main(string[] args)
        {
            using (REPTsysWindow systemsWindow = new REPTsysWindow(1080, 640, "Rift Engine Planet Tools"))
            {
                systemsWindow.Run(60.0);
                systemsWindow.Dispose();
            }
        }
    }
    

    /*
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
            Application.Run(new MainWindow());
        }
    }
    */
}
