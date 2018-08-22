using System;
using System.Windows.Forms;
using DevExpress.Skins;

namespace LTFYYJXT
{
    internal static class Program
    {
        /// <summary>
        ///     应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
            SkinManager.EnableFormSkins();

            Application.Run(new Form1());
            //ffff
        }
    }
}