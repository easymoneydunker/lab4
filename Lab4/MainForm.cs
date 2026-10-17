using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;

namespace Lab4
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            AddCommands(tabFiles, flpFiles, mnuFiles, new (string, Action)[]
            {
                ("1. Создать каталоги", Step1),
                ("2. Создать file1.txt", Step2),
                ("3. Прочитать file1.txt", Step3),
                ("4. Прочитать 8 байт с 5-го", Step4),
                ("5. Дописать строку в конец", Step5),
                ("6. «Только чтение» + «скрытый», удалить", Step6),
                ("7. Снять «скрытый», сменить дату", Step7),
                ("8. Переименовать в file2.txt", Step8),
                ("9. Переместить в S2, копировать в S3", Step9),
                ("10. Установить метку LABEL", Step10),
                ("11. Проверить метку", Step11),
                ("12. Сменить метку в Windows и проверить", Step12),
                ("Выполнить п.1–9", RunSteps1To9),
                ("Удалить TESTDIR", DeleteTestDir),
            });
            AddCommands(tabMemory, flpMemory, mnuMemory, new (string, Action)[]
            {
                ("1) Информация о системе", ShowSystemInfo),
                ("2) Состояние памяти", ShowMemoryStatus),
                ("3) Виртуальное адресное пространство", ShowVirtualMemory),
                ("Очистить", () => txtMemory.Clear()),
            });
            AddCommands(tabClipboard, flpClipboard, mnuClipboard, new (string, Action)[]
            {
                ("Обновить список форматов", () => RefreshFormats(true)),
                ("Поместить текст в буфер", () => { Clipboard.SetText(txtClipText.Text); RefreshFormats(true); }),
                ("Очистить буфер", () => { Clipboard.Clear(); RefreshFormats(true); }),
            });

            InitFiles();
            chkTimer.Checked = true;
        }

        private void AddCommands(TabPage tab, FlowLayoutPanel panel, ToolStripMenuItem menu, (string Text, Action Run)[] commands)
        {
            foreach (var (text, run) in commands)
            {
                void Execute()
                {
                    tabControl.SelectedTab = tab;
                    try
                    {
                        run();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                var button = new Button { Text = text, AutoSize = true, UseVisualStyleBackColor = true };
                if (panel.FlowDirection == FlowDirection.TopDown)
                {
                    button.AutoSize = false;
                    button.Size = new System.Drawing.Size(300, 32);
                    button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                }
                button.Click += (s, e) => Execute();
                panel.Controls.Add(button);

                menu.DropDownItems.Add(text, null, (s, e) => Execute());
            }
        }

        private void Info(string text) =>
            MessageBox.Show(this, text, "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void Warning(string text) =>
            MessageBox.Show(this, text, "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        private void mnuExit_Click(object sender, EventArgs e) => Close();

        private void mnuHelpContents_Click(object sender, EventArgs e)
        {
            using (var help = new HelpForm())
                help.ShowDialog(this);
        }

        private void mnuAbout_Click(object sender, EventArgs e) =>
            Info("Лабораторная работа № 4\n\n" +
                 "4.2.1. Файловые операции\n4.2.2. Работа с памятью\n4.2.3. Буфер обмена\n\n" +
                 $"C#, Windows Forms, .NET {Environment.Version}");

        private static readonly string[] Dirs = { @"S1\S2\S4", @"S1\S3\S5" };

        private const string File1 = "file1.txt";
        private const string File2 = "file2.txt";
        private const string VolumeLabel = "LABEL";
        private static readonly DateTime NewCreationTime = new DateTime(2001, 1, 1, 10, 0, 0);

        private static Encoding Ansi => Encoding.GetEncoding(1251);

        private string rootDir;
        private string s1Dir;
        private bool runningAll;

        private void InitFiles()
        {
            rootDir = Path.Combine(Directory.GetCurrentDirectory(), "TESTDIR");
            s1Dir = Path.Combine(rootDir, "S1");

            foreach (DriveInfo d in DriveInfo.GetDrives().Where(d => d.IsReady))
                cmbDrive.Items.Add(d.Name);
            cmbDrive.SelectedItem = Path.GetPathRoot(rootDir);
            if (cmbDrive.SelectedIndex < 0 && cmbDrive.Items.Count > 0)
                cmbDrive.SelectedIndex = 0;

            Log("Текущий каталог: " + Directory.GetCurrentDirectory());
        }

        private void Log(string text = "") => txtFiles.AppendText(text + Environment.NewLine);

        private void Title(string text)
        {
            Log();
            Log("=== " + text + " ===");
        }

        private void GoToS1()
        {
            if (!Directory.Exists(s1Dir))
                throw new DirectoryNotFoundException("Каталог S1 не найден. Сначала выполните п.1.");
            Directory.SetCurrentDirectory(s1Dir);
        }

        private void RequireFile1()
        {
            GoToS1();
            if (!File.Exists(File1))
                throw new FileNotFoundException("Файл file1.txt не найден в S1. Сначала выполните п.2.");
        }

        private static void DeleteIfExists(string path)
        {
            if (!File.Exists(path))
                return;
            File.SetAttributes(path, FileAttributes.Normal);
            File.Delete(path);
        }

        private void PrintTree(string dir, string indent)
        {
            foreach (string sub in Directory.GetDirectories(dir))
            {
                Log(indent + Path.GetFileName(sub) + @"\");
                PrintTree(sub, indent + "    ");
            }
            foreach (string file in Directory.GetFiles(dir))
            {
                var info = new FileInfo(file);
                Log($"{indent}{info.Name}  ({info.Length} байт, {info.Attributes})");
            }
        }

        private void Step1()
        {
            Title("П.1. Создание каталожной структуры");
            foreach (string dir in Dirs)
            {
                string path = Path.Combine(rootDir, dir);
                Log((Directory.Exists(path) ? "Уже существует: " : "Создан: ") + path);
                Directory.CreateDirectory(path);
            }

            Directory.SetCurrentDirectory(s1Dir);
            Log("Текущий каталог: " + Directory.GetCurrentDirectory());
            Log();
            Log(rootDir + @"\");
            PrintTree(rootDir, "    ");
        }

        private void Step2()
        {
            Title("П.2. Создание file1.txt");
            GoToS1();
            DeleteIfExists(File1);
            File.WriteAllText(File1, "Текстовая строка", Ansi);
            Log($"Создан {Path.GetFullPath(File1)}, записано {new FileInfo(File1).Length} байт");
        }

        private void Step3()
        {
            Title("П.3. Чтение file1.txt");
            RequireFile1();
            string text = File.ReadAllText(File1, Ansi);
            Log($"Содержимое: \"{text}\"");
        }

        private void Step4()
        {
            Title("П.4. Чтение 8 байт с 5-го байта");
            RequireFile1();
            using (FileStream fs = File.OpenRead(File1))
            {
                fs.Seek(4, SeekOrigin.Begin);
                var buffer = new byte[8];
                int count = fs.Read(buffer, 0, buffer.Length);
                Log($"Прочитано {count} байт: \"{Ansi.GetString(buffer, 0, count)}\"");
            }
        }

        private void Step5()
        {
            Title("П.5. Добавление строки в конец file1.txt");
            RequireFile1();
            using (var fs = new FileStream(File1, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                long end = fs.Seek(0, SeekOrigin.End);
                byte[] data = Ansi.GetBytes("\r\nДополнительная строка");

                fs.Lock(end, data.Length);
                fs.Write(data, 0, data.Length);
                fs.Flush();
                Log($"Записано {data.Length} байт с позиции {end} (участок заблокирован)");

                try
                {
                    using (var other = new FileStream(File1, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        other.Seek(end, SeekOrigin.Begin);
                        other.Read(new byte[data.Length], 0, data.Length);
                    }
                    Log("Проверка блокировки — участок прочитан другим потоком (блокировка не сработала)");
                }
                catch (IOException ex)
                {
                    Log("Проверка блокировки — чтение через другой поток: " + ex.Message);
                }

                fs.Unlock(end, data.Length);
                Log("Блокировка снята");
            }
            Log("Содержимое файла:");
            Log(File.ReadAllText(File1, Ansi));
        }

        private void Step6()
        {
            Title("П.6. Атрибуты и попытка удаления");
            RequireFile1();
            File.SetAttributes(File1, FileAttributes.ReadOnly | FileAttributes.Hidden);
            Log("Атрибуты: " + File.GetAttributes(File1));
            try
            {
                File.Delete(File1);
                Log("Файл удалён");
            }
            catch (UnauthorizedAccessException ex)
            {
                Log("Удалить не удалось: " + ex.Message);
                if (!runningAll)
                    Warning("Файл не удаляется, потому что у него атрибут «только чтение».\n\n" + ex.Message);
            }
        }

        private void Step7()
        {
            Title("П.7. Снятие атрибута «скрытый», изменение даты создания");
            RequireFile1();
            File.SetAttributes(File1, File.GetAttributes(File1) & ~FileAttributes.Hidden);
            Log("Атрибуты: " + File.GetAttributes(File1));

            Log("Дата создания была:  " + File.GetCreationTime(File1));
            File.SetCreationTime(File1, NewCreationTime);
            Log("Дата создания стала: " + File.GetCreationTime(File1));
        }

        private void Step8()
        {
            Title("П.8. Переименование file1.txt → file2.txt");
            RequireFile1();
            DeleteIfExists(File2);
            File.Move(File1, File2);
            Log("Файл переименован: " + Path.GetFullPath(File2));
        }

        private void Step9()
        {
            Title("П.9. Перемещение в S2 и копирование в S3");
            GoToS1();
            if (!File.Exists(File2))
                throw new FileNotFoundException("Файл file2.txt не найден в S1. Сначала выполните п.8.");

            string inS2 = Path.Combine("S2", File2);
            string inS3 = Path.Combine("S3", File2);
            DeleteIfExists(inS2);
            File.Move(File2, inS2);
            Log("Перемещён в " + inS2);
            DeleteIfExists(inS3);
            File.Copy(inS2, inS3);
            Log("Скопирован в " + inS3);

            foreach (string name in new[] { "S1", @"S1\S2", @"S1\S3" })
            {
                Log();
                Log("Содержимое " + name + ":");
                foreach (FileSystemInfo item in new DirectoryInfo(Path.Combine(rootDir, name)).GetFileSystemInfos())
                {
                    string size = item is FileInfo f ? f.Length + " байт" : "<DIR>";
                    Log($"    {item.Name,-12} {size,-10} {item.Attributes,-25} создан {item.CreationTime}");
                }
            }
        }

        private DriveInfo SelectedDrive()
        {
            if (cmbDrive.SelectedItem == null)
                throw new InvalidOperationException("Не выбран диск.");
            return new DriveInfo((string)cmbDrive.SelectedItem);
        }

        private void Step10()
        {
            Title("П.10. Установка метки тома");
            DriveInfo drive = SelectedDrive();
            try
            {
                drive.VolumeLabel = VolumeLabel;
                Log($"Диску {drive.Name} установлена метка {VolumeLabel}");
                Info($"Диску {drive.Name} установлена метка {VolumeLabel}");
            }
            catch (UnauthorizedAccessException)
            {
                Log("Нет прав на изменение метки");
                Warning($"Нет прав на изменение метки диска {drive.Name}.\n\n" +
                        "Запустите программу от имени администратора или выберите флешку.");
            }
        }

        private void Step11()
        {
            Title("П.11. Проверка метки тома");
            CheckLabel();
        }

        private void CheckLabel()
        {
            DriveInfo drive = SelectedDrive();
            string label = drive.VolumeLabel;
            Log($"Диск {drive.Name}: метка «{label}», файловая система {drive.DriveFormat}, тип {drive.DriveType}");

            if (label == "")
                Warning($"У диска {drive.Name} нет метки.");
            else if (string.Equals(label, VolumeLabel, StringComparison.OrdinalIgnoreCase))
                Info($"Метка диска {drive.Name} — «{label}», совпадает с заданной в п.10.");
            else
                Warning($"Метка диска {drive.Name} — «{label}», НЕ совпадает с «{VolumeLabel}».");
        }

        private void Step12()
        {
            Title("П.12. Изменение метки средствами Windows");
            DialogResult answer = MessageBox.Show(this,
                "Измените метку диска средствами Windows:\n" +
                "Проводник → правая кнопка на диске → Свойства → поле с меткой.\n" +
                "Чтобы стереть метку, очистите это поле.\n\n" +
                "Когда закончите, нажмите ОК — программа проверит метку.",
                "П.12", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (answer == DialogResult.OK)
                CheckLabel();
        }

        private void RunSteps1To9()
        {
            runningAll = true;
            try
            {
                Step1(); Step2(); Step3(); Step4(); Step5(); Step6(); Step7(); Step8(); Step9();
            }
            finally
            {
                runningAll = false;
            }
            Info("Пункты 1–9 выполнены, результаты — в окне вывода.");
        }

        private void DeleteTestDir()
        {
            Title("Удаление TESTDIR");
            Directory.SetCurrentDirectory(Path.GetDirectoryName(rootDir));
            if (!Directory.Exists(rootDir))
            {
                Log("Каталога нет");
                return;
            }
            foreach (string file in Directory.GetFiles(rootDir, "*", SearchOption.AllDirectories))
                File.SetAttributes(file, FileAttributes.Normal);
            Directory.Delete(rootDir, true);
            Log("Каталог удалён: " + rootDir);
        }

        private void Mem(string text = "") => txtMemory.AppendText(text + Environment.NewLine);

        private static string Mb(double bytes) => $"{bytes / 1024 / 1024:N0} МБ";

        private static string Hex(IntPtr p) => "0x" + p.ToInt64().ToString("X12");

        private void ShowSystemInfo()
        {
            Mem("=== 1) Информация о системе ===");
            Mem($"Операционная система:   {RuntimeInformation.OSDescription}");
            Mem($"Архитектура процессора: {RuntimeInformation.ProcessArchitecture}");
            Mem($"Процессор:              {Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER")}");
            Mem($"Уровень процессора:     {Environment.GetEnvironmentVariable("PROCESSOR_LEVEL")}");
            Mem($"Модификация процессора: {Environment.GetEnvironmentVariable("PROCESSOR_REVISION")}");
            Mem($"Число процессоров:      {Environment.ProcessorCount}");
            Mem($"Размер страницы:        {Environment.SystemPageSize} байт");
            Mem($"Разрядность процесса:   {(Environment.Is64BitProcess ? 64 : 32)} бит");
            Mem();
        }

        private void ShowMemoryStatus()
        {
            var info = new ComputerInfo();
            ulong total = info.TotalPhysicalMemory;
            ulong free = info.AvailablePhysicalMemory;

            Mem("=== 2) Состояние памяти ===");
            Mem($"Используется памяти:          {100 - free * 100 / total}%");
            Mem($"Всего физической памяти:      {Mb(total)}");
            Mem($"Свободно физической памяти:   {Mb(free)}");
            Mem($"Всего виртуальной памяти:     {Mb(info.TotalVirtualMemory)}");
            Mem($"Свободно виртуальной памяти:  {Mb(info.AvailableVirtualMemory)}");

            using (Process p = Process.GetCurrentProcess())
            {
                Mem("Память этого процесса:");
                Mem($"    виртуальная:              {Mb(p.VirtualMemorySize64)}");
                Mem($"    рабочий набор (в ОЗУ):    {Mb(p.WorkingSet64)}");
                Mem($"    в файле подкачки:         {Mb(p.PagedMemorySize64)}");
            }
            Mem();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public UIntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [DllImport("kernel32.dll")]
        private static extern UIntPtr VirtualQuery(IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, UIntPtr dwLength);

        private void ShowVirtualMemory()
        {
            Mem("=== 3) Виртуальное адресное пространство ===");
            using (Process p = Process.GetCurrentProcess())
            {
                Mem("Модули, загруженные в адресное пространство процесса:");
                foreach (ProcessModule m in p.Modules.Cast<ProcessModule>().OrderBy(m => m.BaseAddress.ToInt64()))
                    Mem($"    {Hex(m.BaseAddress)}  {m.ModuleMemorySize / 1024,7} КБ  {m.ModuleName}");

                Mem();
                Mem("VirtualQuery для разных адресов:");
                QueryAddress("Начало программы (EXE)", p.MainModule.BaseAddress);

                IntPtr block = Marshal.AllocHGlobal(4096);
                QueryAddress("Блок, выделенный AllocHGlobal", block);
                Marshal.FreeHGlobal(block);
            }
            Mem();
        }

        private void QueryAddress(string title, IntPtr address)
        {
            var size = (UIntPtr)Marshal.SizeOf<MEMORY_BASIC_INFORMATION>();
            if (VirtualQuery(address, out MEMORY_BASIC_INFORMATION mbi, size) == UIntPtr.Zero)
            {
                Mem($"    {title}: VirtualQuery не выполнилась");
                return;
            }

            string state = mbi.State switch
            {
                0x1000 => "MEM_COMMIT (память выделена)",
                0x2000 => "MEM_RESERVE (зарезервирована)",
                0x10000 => "MEM_FREE (свободна)",
                _ => mbi.State.ToString()
            };
            string type = mbi.Type switch
            {
                0x1000000 => "MEM_IMAGE (образ EXE/DLL)",
                0x40000 => "MEM_MAPPED (проецируемый файл)",
                0x20000 => "MEM_PRIVATE (закрытая память)",
                _ => "—"
            };
            string protect = mbi.Protect switch
            {
                0x01 => "PAGE_NOACCESS",
                0x02 => "PAGE_READONLY",
                0x04 => "PAGE_READWRITE",
                0x08 => "PAGE_WRITECOPY",
                0x20 => "PAGE_EXECUTE_READ",
                0x40 => "PAGE_EXECUTE_READWRITE",
                _ => "0x" + mbi.Protect.ToString("X")
            };

            Mem($"  {title}: адрес {Hex(address)}");
            Mem($"    BaseAddress    = {Hex(mbi.BaseAddress)}");
            Mem($"    AllocationBase = {Hex(mbi.AllocationBase)}");
            Mem($"    RegionSize     = {mbi.RegionSize} байт");
            Mem($"    State          = {state}");
            Mem($"    Protect        = {protect}");
            Mem($"    Type           = {type}");
        }

        private string lastFormats = "";

        private void chkTimer_CheckedChanged(object sender, EventArgs e) => timerClipboard.Enabled = chkTimer.Checked;

        private void timerClipboard_Tick(object sender, EventArgs e) => RefreshFormats(false);

        private void RefreshFormats(bool force)
        {
            IDataObject data;
            try
            {
                data = Clipboard.GetDataObject();
            }
            catch (ExternalException)
            {
                return;
            }
            if (data == null)
                return;

            string[] stored = data.GetFormats(false);
            string[] all = data.GetFormats(true);

            string key = string.Join("|", all);
            if (!force && key == lastFormats)
                return;
            lastFormats = key;

            lblCount.Text = $"Всего форматов доступно: {stored.Length}   (с учётом автопреобразования: {all.Length})";
            lstFormats.BeginUpdate();
            lstFormats.Items.Clear();
            foreach (string format in all)
                lstFormats.Items.Add(stored.Contains(format) ? format : format + "   (автопреобразование)");
            if (all.Length == 0)
                lstFormats.Items.Add("(буфер обмена пуст)");
            lstFormats.EndUpdate();
        }
    }
}
