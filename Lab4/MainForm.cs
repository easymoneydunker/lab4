using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

namespace Lab4
{
    /// <summary>
    /// Главная форма. Логика разнесена по частичным классам:
    ///   MainForm.Files.cs     — 4.2.1 файловые операции;
    ///   MainForm.Memory.cs    — 4.2.2 функции для работы с памятью;
    ///   MainForm.Clipboard.cs — 4.2.3 буфер обмена.
    /// </summary>
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            InitFileSteps();
            InitMemory();
            InitClipboard();

            if (IsAdministrator())
            {
                Text += "  [Администратор]";
                mnuRunAsAdmin.Enabled = false;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            RefreshClipboardFormats(true);
            SetStatus("Готово. Выберите команду в меню или нажмите кнопку. F1 — справка.");
        }

        // ------------------------------------------------------------------
        //  Вспомогательные методы
        // ------------------------------------------------------------------

        private void SetStatus(string text)
        {
            lblStatus.Text = text;
        }

        /// <summary>Текст системного сообщения об ошибке (FormatMessage) по коду GetLastError.</summary>
        private static string ErrorText(int code)
        {
            return $"код {code}: {new Win32Exception(code).Message}";
        }

        /// <summary>Сообщение об ошибке вызова системной функции.</summary>
        private void ShowApiError(string function, int code, string hint = null)
        {
            string msg = $"Ошибка при вызове {function}\n\n{ErrorText(code)}";
            if (!string.IsNullOrEmpty(hint))
                msg += "\n\n" + hint;
            SetStatus($"Ошибка {function} ({code})");
            MessageBox.Show(this, msg, "Ошибка Win32 API", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowInfo(string text, string caption = "Информация")
        {
            MessageBox.Show(this, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowWarning(string text, string caption = "Внимание")
        {
            MessageBox.Show(this, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private static string FormatPtr(IntPtr p)
        {
            return IntPtr.Size == 8 ? p.ToInt64().ToString("X16") : p.ToInt32().ToString("X8");
        }

        /// <summary>Размер в удобочитаемом виде: байты, КБ, МБ, ГБ.</summary>
        private static string HumanSize(ulong bytes)
        {
            string[] units = { "байт", "КБ", "МБ", "ГБ", "ТБ" };
            double v = bytes;
            int i = 0;
            while (v >= 1024 && i < units.Length - 1)
            {
                v /= 1024;
                i++;
            }
            return i == 0 ? $"{bytes} {units[0]}" : $"{v:0.##} {units[i]}";
        }

        private static bool IsAdministrator()
        {
            try
            {
                using (var id = WindowsIdentity.GetCurrent())
                    return new WindowsPrincipal(id).IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Перезапуск приложения с повышением прав (нужно для SetVolumeLabel на системном диске).</summary>
        private bool RestartAsAdmin()
        {
            try
            {
                // Повышенный процесс стартует в System32, поэтому текущий каталог передаём аргументом
                var psi = new ProcessStartInfo(Application.ExecutablePath)
                {
                    UseShellExecute = true,
                    Verb = "runas",
                    Arguments = $"{Program.DirArgument} \"{System.IO.Path.Combine(startDir, ".")}\""
                };
                Process.Start(psi);
                Application.Exit();
                return true;
            }
            catch (Win32Exception)
            {
                ShowWarning("Запуск с правами администратора отменён пользователем.");
                return false;
            }
        }

        // ------------------------------------------------------------------
        //  Меню «Файл» и «Справка»
        // ------------------------------------------------------------------

        private void mnuRunAsAdmin_Click(object sender, EventArgs e)
        {
            RestartAsAdmin();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mnuHelpContents_Click(object sender, EventArgs e)
        {
            string topic;
            if (tabControl.SelectedTab == tabMemory) topic = HelpForm.TopicMemory;
            else if (tabControl.SelectedTab == tabClipboard) topic = HelpForm.TopicClipboard;
            else topic = HelpForm.TopicFiles;
            HelpForm.ShowHelp(this, topic);
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this,
                "Лабораторная работа № 4\n" +
                "Создание приложений Windows с использованием средств\n" +
                "прикладного программирования Win32 API\n\n" +
                "4.2.1. Системные средства для выполнения файловых операций\n" +
                "4.2.2. Системные функции для работы с памятью\n" +
                "4.2.3. Системные функции для работы с буфером обмена\n\n" +
                $"C# / Windows Forms, .NET {Environment.Version}\n" +
                $"Процесс: {(Environment.Is64BitProcess ? "64" : "32")}-разрядный, " +
                $"ОС: {(Environment.Is64BitOperatingSystem ? "64" : "32")}-разрядная",
                "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            timerClipboard.Stop();
            if (heapBlock != IntPtr.Zero)
            {
                NativeMethods.GlobalFree(heapBlock);
                heapBlock = IntPtr.Zero;
            }
            base.OnFormClosing(e);
        }
    }
}
