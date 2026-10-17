using System;
using System.Text;
using System.Windows.Forms;

namespace Lab4
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var splash = new SplashForm())
                splash.ShowDialog();

            Application.Run(new MainForm());
        }
    }
}
