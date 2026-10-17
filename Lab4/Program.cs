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
            // Подключаем кодировку Windows-1251: в ней 1 символ = 1 байт (нужно для п.4)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Сначала заставка, затем главное окно
            using (var splash = new SplashForm())
                splash.ShowDialog();

            Application.Run(new MainForm());
        }
    }
}
