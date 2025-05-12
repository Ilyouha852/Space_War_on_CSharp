using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Programming_technologies_6
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var menu = new MainMenuForm();
            if (menu.ShowDialog() == DialogResult.OK)
            {
                var modeForm = new GameModeForm();
                if (modeForm.ShowDialog() == DialogResult.OK && modeForm.SelectedMode.HasValue)
                {
                    Application.Run(new Form1(modeForm.SelectedMode.Value));
                }
            }
        }
    }
}
