using System;
using System.Windows.Forms;

namespace VendingMachine
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

            var mainForm = new VendingMachineForm();
            var vendingMachine = new VendingMachine(mainForm);
            Application.Run(mainForm);
        }
    }
}
