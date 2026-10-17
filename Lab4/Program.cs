using System;
using System.Text;
using System.Windows.Forms;

namespace Lab4
{
    internal static class Program
    {
        /// <summary>Аргумент командной строки: каталог, который нужно сделать текущим при запуске.</summary>
        public const string DirArgument = "--dir";

        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length >= 2 && args[0] == DirArgument)
                NativeMethods.SetCurrentDirectory(args[1]);

            // Нужен для кодировок Windows-1251 / CP866 (ANSI/OEM-тексты файлов и буфера обмена)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Заставка
            using (var splash = new SplashForm())
            {
                splash.ShowDialog();
            }

            Application.Run(new MainForm());
        }
    }
}
