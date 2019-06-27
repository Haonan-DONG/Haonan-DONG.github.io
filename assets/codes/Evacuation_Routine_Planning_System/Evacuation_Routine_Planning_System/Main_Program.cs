using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Evacuation_Routine_Planning_System
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            E_R_P_Interface erpInterface = new E_R_P_Interface();
            Application.Run(erpInterface);
            /*
            Application.Run(new Load_Interface());
            
            // also in the main thread.
            if(Global.Login)
            {
                E_R_P_Interface erpInterface = new E_R_P_Interface();
                Application.Run(erpInterface);
            }
            */
        }
    }
}
