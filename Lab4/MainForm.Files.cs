using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Lab4.NativeMethods;

namespace Lab4
{
    /// <summary>
    /// 4.2.1. Системные средства для выполнения файловых операций.
    /// Все операции с файлами, каталогами и дисками выполняются прямыми вызовами Win32 API.
    /// </summary>
    public partial class MainForm
    {
        // Каталожная структура (из лабораторной работы № 3). Корень создаётся в текущем каталоге.
        private const string RootDirName = "TESTDIR";
        private static readonly string[] SubDirs =
        {
            "S1",
            @"S1\S2",
            @"S1\S3",
            @"S1\S2\S4",
            @"S1\S3\S5"
        };

        private const string File1 = "file1.txt";
        private const string File2 = "file2.txt";
        private const string Text1 = "Текстовая строка";
        private const string Text2 = "Дополнительная строка";
        private const string LineBreak = "\r\n";

        /// <summary>Однобайтовая кодировка Windows-1251: 1 символ = 1 байт, как в примерах на Delphi.</summary>
        private static Encoding Ansi => Encoding.GetEncoding(1251);

        private string startDir;   // текущий каталог при запуске программы
        private string rootDir;    // <текущий каталог>\TESTDIR
        private string s1Dir;      // <текущий каталог>\TESTDIR\S1
        private bool batchMode;    // режим «выполнить всё подряд» — без лишних окон

        private sealed class DriveItem
        {
            public string Root;
            public string Text;
            public override string ToString() => Text;
        }

        // ------------------------------------------------------------------
        //  Инициализация: кнопки и пункты меню для п.1–12
        // ------------------------------------------------------------------
        private void InitFileSteps()
        {
            startDir = GetCurDir();
            rootDir = Path.Combine(startDir, RootDirName);
            s1Dir = Path.Combine(rootDir, "S1");

            var steps = new (string Title, Action Run)[]
            {
                ("Создать каталожную структуру (CreateDirectory)", Step01_CreateStructure),
                ("Создать file1.txt в S1 и записать строку (OpenFile, _lwrite)", Step02_CreateFile),
                ("Прочитать file1.txt целиком (_lopen, _lread)", Step03_ReadFile),
                ("Прочитать 8 байт с 5-го байта (_llseek, _lread)", Step04_ReadPart),
                ("Дописать строку в конец (ReadFile, SetFilePointer, LockFile)", Step05_Append),
                ("Атрибуты «только чтение» и «скрытый», удаление", Step06_Attributes),
                ("Снять «скрытый», изменить дату создания (SetFileTime)", Step07_FileTime),
                ("Переименовать file1.txt → file2.txt (MoveFile)", Step08_Rename),
                ("Переслать в S2, скопировать в S3 (MoveFile, CopyFile)", Step09_MoveCopy),
                ("Установить метку тома (SetVolumeLabel)", Step10_SetLabel),
                ("Найти метку тома и сравнить (GetVolumeInformation)", Step11_CheckLabel),
                ("Изменить/стереть метку средствами ОС и проверить", Step12_OsLabel)
            };

            for (int i = 0; i < steps.Length; i++)
            {
                int n = i + 1;
                Action run = steps[i].Run;
                string text = $"{n}. {steps[i].Title}";

                var btn = new Button
                {
                    Text = text,
                    Width = StepButtonWidth(),
                    Height = TextRenderer.MeasureText("Wg", Font).Height * 2 + 12,
                    TextAlign = ContentAlignment.MiddleLeft,
                    UseVisualStyleBackColor = true,
                    Margin = new Padding(3, 2, 3, 2)
                };
                btn.Click += (s, e) => RunStep(run);
                flpSteps.Controls.Add(btn);

                var item = new ToolStripMenuItem(text);
                item.Click += (s, e) => RunStep(run);
                mnuFiles.DropDownItems.Add(item);
            }

            AddExtraFileCommand("▶ Выполнить п.1–9 подряд", RunAllSteps, true);
            AddExtraFileCommand("Показать каталожную структуру", () => { LogHeader("Каталожная структура"); ShowStructure(); }, false);
            AddExtraFileCommand("Удалить созданную структуру", RemoveStructure, false);
            AddExtraFileCommand("Очистить протокол", () => txtLog.Clear(), false);

            FillDrives();
            UpdateCurDirLabel();
            Log("Текущий каталог при запуске: " + startDir);
            Log("Каталожная структура будет создана в: " + rootDir);
        }

        private void AddExtraFileCommand(string text, Action run, bool separatorBefore)
        {
            var btn = new Button
            {
                Text = text,
                Width = StepButtonWidth(),
                Height = TextRenderer.MeasureText("Wg", Font).Height + 14,
                UseVisualStyleBackColor = true,
                Margin = new Padding(3, separatorBefore ? 12 : 2, 3, 2)
            };
            btn.Click += (s, e) => RunStep(run);
            flpSteps.Controls.Add(btn);

            if (separatorBefore)
                mnuFiles.DropDownItems.Add(new ToolStripSeparator());
            var item = new ToolStripMenuItem(text);
            item.Click += (s, e) => RunStep(run);
            mnuFiles.DropDownItems.Add(item);
        }

        /// <summary>Ширина кнопки п.1–12 с учётом масштабирования панели и полосы прокрутки.</summary>
        private int StepButtonWidth()
        {
            return pnlLeft.Width - pnlLeft.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth - 8;
        }

        private void RunStep(Action step)
        {
            tabControl.SelectedTab = tabFiles;
            try
            {
                step();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Непредвиденная ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UpdateCurDirLabel();
            }
        }

        private void RunAllSteps()
        {
            LogHeader("Последовательное выполнение п.1–9");
            batchMode = true;
            try
            {
                Step01_CreateStructure();
                Step02_CreateFile();
                Step03_ReadFile();
                Step04_ReadPart();
                Step05_Append();
                Step06_Attributes();
                Step07_FileTime();
                Step08_Rename();
                Step09_MoveCopy();
            }
            finally
            {
                batchMode = false;
            }
            SetStatus("Пункты 1–9 выполнены");
            ShowInfo("Пункты 1–9 выполнены. Результаты — в протоколе.\n" +
                     "Пункты 10–12 (метка тома) выполняются отдельно.");
        }

        // ------------------------------------------------------------------
        //  Протокол
        // ------------------------------------------------------------------
        private void Log(string text = "")
        {
            txtLog.AppendText(text.Replace("\r\n", "\n").Replace("\n", LineBreak) + LineBreak);
        }

        private void LogHeader(string title)
        {
            if (txtLog.TextLength > 0)
                Log();
            Log(new string('=', 78));
            Log($"[{DateTime.Now:HH:mm:ss}] {title}");
            Log(new string('=', 78));
            SetStatus(title);
        }

        private void UpdateCurDirLabel()
        {
            lblCurDir.Text = "Текущий каталог: " + GetCurDir();
        }

        // ------------------------------------------------------------------
        //  Вспомогательные функции поверх Win32 API
        // ------------------------------------------------------------------
        private static string GetCurDir()
        {
            var sb = new StringBuilder(1024);
            uint len = GetCurrentDirectory((uint)sb.Capacity, sb);
            if (len > sb.Capacity)
            {
                sb = new StringBuilder((int)len);
                GetCurrentDirectory((uint)sb.Capacity, sb);
            }
            return sb.ToString();
        }

        private static bool DirExists(string path)
        {
            uint a = GetFileAttributes(path);
            return a != INVALID_FILE_ATTRIBUTES && (a & FILE_ATTRIBUTE_DIRECTORY) != 0;
        }

        private static bool FileExists(string path)
        {
            uint a = GetFileAttributes(path);
            return a != INVALID_FILE_ATTRIBUTES && (a & FILE_ATTRIBUTE_DIRECTORY) == 0;
        }

        private static string AttrShort(uint a)
        {
            return new string(new[]
            {
                (a & FILE_ATTRIBUTE_READONLY) != 0 ? 'R' : '-',
                (a & FILE_ATTRIBUTE_HIDDEN) != 0 ? 'H' : '-',
                (a & FILE_ATTRIBUTE_SYSTEM) != 0 ? 'S' : '-',
                (a & FILE_ATTRIBUTE_ARCHIVE) != 0 ? 'A' : '-'
            });
        }

        private static string AttrLong(uint a)
        {
            var list = new List<string>();
            if ((a & FILE_ATTRIBUTE_READONLY) != 0) list.Add("только чтение");
            if ((a & FILE_ATTRIBUTE_HIDDEN) != 0) list.Add("скрытый");
            if ((a & FILE_ATTRIBUTE_SYSTEM) != 0) list.Add("системный");
            if ((a & FILE_ATTRIBUTE_DIRECTORY) != 0) list.Add("каталог");
            if ((a & FILE_ATTRIBUTE_ARCHIVE) != 0) list.Add("архивный");
            if ((a & FILE_ATTRIBUTE_NORMAL) != 0) list.Add("обычный");
            return $"${a:X4} ({(list.Count > 0 ? string.Join(", ", list) : "нет")})";
        }

        /// <summary>FILETIME (UTC) → строка локального времени через FileTimeToLocalFileTime/FileTimeToSystemTime.</summary>
        private static string FormatFileTime(long fileTimeUtc)
        {
            if (!FileTimeToLocalFileTime(ref fileTimeUtc, out long local) ||
                !FileTimeToSystemTime(ref local, out SYSTEMTIME st))
                return "?";
            return $"{st.wDay:00}.{st.wMonth:00}.{st.wYear:0000} {st.wHour:00}:{st.wMinute:00}:{st.wSecond:00}";
        }

        /// <summary>Перечисление содержимого каталога функциями FindFirstFile / FindNextFile.</summary>
        private static List<WIN32_FIND_DATA> ListEntries(string dir)
        {
            var result = new List<WIN32_FIND_DATA>();
            IntPtr hFind = FindFirstFile(Path.Combine(dir, "*"), out WIN32_FIND_DATA fd);
            if (hFind == INVALID_HANDLE_VALUE)
                return result;
            try
            {
                do
                {
                    if (fd.cFileName != "." && fd.cFileName != "..")
                        result.Add(fd);
                } while (FindNextFile(hFind, out fd));
            }
            finally
            {
                FindClose(hFind);
            }
            // каталоги — первыми, затем файлы, по алфавиту
            result.Sort((x, y) =>
            {
                bool dx = (x.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0;
                bool dy = (y.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0;
                if (dx != dy) return dx ? -1 : 1;
                return string.Compare(x.cFileName, y.cFileName, StringComparison.OrdinalIgnoreCase);
            });
            return result;
        }

        private static ulong FileSize(WIN32_FIND_DATA fd) => ((ulong)fd.nFileSizeHigh << 32) | fd.nFileSizeLow;

        /// <summary>Вывод каталожной структуры в дерево и в протокол.</summary>
        private void ShowStructure()
        {
            tvStructure.BeginUpdate();
            tvStructure.Nodes.Clear();
            try
            {
                if (!DirExists(rootDir))
                {
                    tvStructure.Nodes.Add("(структура не создана)");
                    Log("Каталог " + rootDir + " не существует.");
                    return;
                }

                var sb = new StringBuilder();
                sb.AppendLine(rootDir + "\\");
                var root = new TreeNode(RootDirName) { ToolTipText = rootDir };
                FillTree(rootDir, root.Nodes, sb, "");
                tvStructure.Nodes.Add(root);
                root.ExpandAll();
                Log(sb.ToString().TrimEnd());
            }
            finally
            {
                tvStructure.EndUpdate();
            }
        }

        private static void FillTree(string dir, TreeNodeCollection nodes, StringBuilder sb, string indent)
        {
            List<WIN32_FIND_DATA> entries = ListEntries(dir);
            for (int i = 0; i < entries.Count; i++)
            {
                WIN32_FIND_DATA fd = entries[i];
                bool last = i == entries.Count - 1;
                bool isDir = (fd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0;
                string name = fd.cFileName;

                if (isDir)
                {
                    sb.AppendLine($"{indent}{(last ? "└── " : "├── ")}{name}\\");
                    var node = new TreeNode("[" + name + "]");
                    nodes.Add(node);
                    FillTree(Path.Combine(dir, name), node.Nodes, sb, indent + (last ? "    " : "│   "));
                }
                else
                {
                    string info = $"{FileSize(fd)} байт, {AttrShort(fd.dwFileAttributes)}";
                    sb.AppendLine($"{indent}{(last ? "└── " : "├── ")}{name}  ({info})");
                    var node = new TreeNode($"{name}  ({info})");
                    if ((fd.dwFileAttributes & FILE_ATTRIBUTE_HIDDEN) != 0)
                        node.ForeColor = Color.Gray;
                    nodes.Add(node);
                }
            }
        }

        /// <summary>Вывод содержимого каталога в виде таблицы.</summary>
        private void LogDirectory(string title, string dir)
        {
            Log($"Содержимое {title} ({dir}):");
            List<WIN32_FIND_DATA> entries = ListEntries(dir);
            if (entries.Count == 0)
            {
                Log("    (пусто)");
                return;
            }
            Log($"    {"Имя",-14} {"Размер",10}  {"Атр.",-4}  {"Создан",-19}  {"Изменён",-19}");
            foreach (WIN32_FIND_DATA fd in entries)
            {
                bool isDir = (fd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0;
                string size = isDir ? "<DIR>" : FileSize(fd).ToString();
                Log($"    {fd.cFileName,-14} {size,10}  {AttrShort(fd.dwFileAttributes),-4}  " +
                    $"{FormatFileTime(fd.ftCreationTime.ToLong()),-19}  {FormatFileTime(fd.ftLastWriteTime.ToLong()),-19}");
            }
        }

        /// <summary>Проверяет наличие S1 и делает его текущим каталогом.</summary>
        private bool EnsureS1()
        {
            if (!DirExists(s1Dir))
            {
                ShowWarning("Каталог S1 не найден:\n" + s1Dir + "\n\nСначала выполните п.1 (создание каталожной структуры).");
                return false;
            }
            if (!SetCurrentDirectory(s1Dir))
            {
                ShowApiError("SetCurrentDirectory", Marshal.GetLastWin32Error());
                return false;
            }
            return true;
        }

        private bool EnsureFile1()
        {
            if (!EnsureS1())
                return false;
            if (FileExists(File1))
                return true;
            string extra = FileExists(File2) || FileExists(@"S2\" + File2)
                ? "\n\nФайл уже переименован/перемещён (п.8–9). Для повторения выполните п.2."
                : "\n\nСначала выполните п.2.";
            ShowWarning($"Файл {File1} не найден в каталоге S1." + extra);
            return false;
        }

        /// <summary>Если файл-приёмник существует — с подтверждения снимаем атрибуты и удаляем его.</summary>
        private bool PrepareTarget(string path)
        {
            if (!FileExists(path))
                return true;
            if (!batchMode &&
                MessageBox.Show(this, $"Файл {path} уже существует.\nУдалить его и продолжить?", "Файл существует",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                Log($"Операция отменена: {path} уже существует.");
                return false;
            }
            SetFileAttributes(path, FILE_ATTRIBUTE_NORMAL);
            if (!DeleteFile(path))
            {
                int err = Marshal.GetLastWin32Error();
                Log($"DeleteFile(\"{path}\") — ошибка {ErrorText(err)}");
                ShowApiError("DeleteFile", err);
                return false;
            }
            Log($"Существующий файл {path} удалён (DeleteFile).");
            return true;
        }

        /// <summary>Чтение файла целиком функциями _lopen / _llseek / _lread / _lclose.</summary>
        private byte[] ReadWholeFile(string name)
        {
            int h = _lopen(name, OF_READ | OF_SHARE_DENY_NONE);
            if (h == HFILE_ERROR)
            {
                int err = Marshal.GetLastWin32Error();
                Log($"_lopen(\"{name}\", OF_READ) — ошибка {ErrorText(err)}");
                ShowApiError("_lopen", err);
                return null;
            }
            try
            {
                int size = _llseek(h, 0, FILE_END);       // размер файла = позиция конца
                _llseek(h, 0, FILE_BEGIN);
                var buf = new byte[Math.Max(size, 0)];
                uint read = _lread(h, buf, (uint)buf.Length);
                if (read == unchecked((uint)HFILE_ERROR))
                {
                    ShowApiError("_lread", Marshal.GetLastWin32Error());
                    return null;
                }
                Array.Resize(ref buf, (int)read);
                return buf;
            }
            finally
            {
                _lclose(h);
            }
        }

        private static string HexDump(byte[] data)
        {
            var sb = new StringBuilder();
            for (int off = 0; off < data.Length; off += 16)
            {
                int n = Math.Min(16, data.Length - off);
                sb.Append($"    {off + 1,4}: ");
                for (int i = 0; i < 16; i++)
                    sb.Append(i < n ? data[off + i].ToString("X2") + " " : "   ");
                sb.Append(" ");
                for (int i = 0; i < n; i++)
                {
                    byte b = data[off + i];
                    sb.Append(b < 32 ? '.' : Ansi.GetString(data, off + i, 1)[0]);
                }
                sb.AppendLine();
            }
            return sb.ToString().TrimEnd();
        }

        // ------------------------------------------------------------------
        //  П.1. Создание каталожной структуры
        // ------------------------------------------------------------------
        private void Step01_CreateStructure()
        {
            LogHeader("П.1. Создание каталожной структуры (CreateDirectory)");
            if (!SetCurrentDirectory(startDir))
            {
                ShowApiError("SetCurrentDirectory", Marshal.GetLastWin32Error());
                return;
            }
            Log("Текущий диск/каталог: " + GetCurDir());

            var dirs = new List<string> { rootDir };
            foreach (string d in SubDirs)
                dirs.Add(Path.Combine(rootDir, d));

            int created = 0, existed = 0;
            foreach (string dir in dirs)
            {
                if (CreateDirectory(dir, IntPtr.Zero))
                {
                    Log("  CreateDirectory: создан     " + dir);
                    created++;
                    continue;
                }
                int err = Marshal.GetLastWin32Error();
                if (err == ERROR_ALREADY_EXISTS)
                {
                    Log("  CreateDirectory: существует " + dir);
                    existed++;
                }
                else
                {
                    Log($"  CreateDirectory: ОШИБКА {ErrorText(err)} — {dir}");
                    ShowApiError("CreateDirectory", err,
                        "Невозможно создать каталоги! Возможно, диск защищён от записи или нет прав доступа.");
                    return;
                }
            }

            if (!SetCurrentDirectory(s1Dir))
            {
                ShowApiError("SetCurrentDirectory", Marshal.GetLastWin32Error());
                return;
            }
            Log("SetCurrentDirectory: текущий каталог → " + GetCurDir());
            Log();
            ShowStructure();

            if (existed > 0 && !batchMode)
                ShowInfo($"Создано каталогов: {created}, уже существовало: {existed}.\nТекущий каталог: S1.",
                    "Каталожная структура");
        }

        // ------------------------------------------------------------------
        //  П.2. Создание file1.txt и запись строки
        // ------------------------------------------------------------------
        private void Step02_CreateFile()
        {
            LogHeader("П.2. Создание файла file1.txt (OpenFile, _lwrite, _lclose)");
            if (!EnsureS1())
                return;

            uint attrs = GetFileAttributes(File1);
            if (attrs != INVALID_FILE_ATTRIBUTES && (attrs & (FILE_ATTRIBUTE_READONLY | FILE_ATTRIBUTE_HIDDEN)) != 0)
            {
                SetFileAttributes(File1, FILE_ATTRIBUTE_NORMAL);
                Log("Файл уже существовал с атрибутами " + AttrLong(attrs) + " — атрибуты сброшены.");
            }

            var ofs = new OFSTRUCT();
            int hFile = OpenFile(File1, ref ofs, OF_CREATE | OF_READWRITE);
            if (hFile == HFILE_ERROR)
            {
                int err = Marshal.GetLastWin32Error();
                if (err == 0) err = ofs.nErrCode;
                Log("OpenFile(OF_CREATE) — ошибка " + ErrorText(err));
                ShowApiError("OpenFile", err);
                return;
            }
            Log($"OpenFile(\"{File1}\", OF_CREATE or OF_READWRITE) → HFILE = {hFile}");
            Log($"  OFSTRUCT.szPathName = {ofs.szPathName}");
            Log($"  OFSTRUCT.fFixedDisk = {ofs.fFixedDisk}");

            byte[] buffer = Ansi.GetBytes(Text1);
            uint written = _lwrite(hFile, buffer, (uint)buffer.Length);
            int errWrite = Marshal.GetLastWin32Error();
            _lclose(hFile);
            Log($"_lclose(HFILE = {hFile})");

            if (written == unchecked((uint)HFILE_ERROR))
            {
                ShowApiError("_lwrite", errWrite);
                return;
            }
            Log($"_lwrite: записано {written} байт: \"{Text1}\"");
            ShowStructure();
            SetStatus("Файл file1.txt создан");
        }

        // ------------------------------------------------------------------
        //  П.3. Чтение файла целиком
        // ------------------------------------------------------------------
        private void Step03_ReadFile()
        {
            LogHeader("П.3. Чтение содержимого file1.txt (_lopen, _llseek, _lread)");
            if (!EnsureFile1())
                return;

            byte[] data = ReadWholeFile(File1);
            if (data == null)
                return;
            Log($"Прочитано {data.Length} байт. Содержимое файла:");
            Log("  \"" + Ansi.GetString(data) + "\"");
            Log("Побайтно (номер байта: hex-коды  символы):");
            Log(HexDump(data));
        }

        // ------------------------------------------------------------------
        //  П.4. Чтение 8 байт начиная с 5-го
        // ------------------------------------------------------------------
        private void Step04_ReadPart()
        {
            LogHeader("П.4. Чтение 8 байт из file1.txt, начиная с байта 5 (_llseek, _lread)");
            if (!EnsureFile1())
                return;

            int hFile = _lopen(File1, OF_READ | OF_SHARE_DENY_NONE);
            if (hFile == HFILE_ERROR)
            {
                ShowApiError("_lopen", Marshal.GetLastWin32Error());
                return;
            }
            try
            {
                const int count = 8;
                int pos = _llseek(hFile, 4, FILE_BEGIN);   // смещение 4 = 5-й байт от начала
                Log($"_llseek(HFILE, 4, FILE_BEGIN) → позиция {pos} (5-й байт)");
                var buff = new byte[count];
                uint read = _lread(hFile, buff, count);
                if (read == unchecked((uint)HFILE_ERROR))
                {
                    ShowApiError("_lread", Marshal.GetLastWin32Error());
                    return;
                }
                string s = Ansi.GetString(buff, 0, (int)read);
                Log($"_lread(HFILE, buff, {count}) → прочитано {read} байт: \"{s}\"");
                if (!batchMode)
                    ShowInfo($"Байты 5–{4 + read} файла file1.txt:\n\n\"{s}\"", "П.4");
            }
            finally
            {
                _lclose(hFile);
            }
        }

        // ------------------------------------------------------------------
        //  П.5. Добавление строки в конец файла
        // ------------------------------------------------------------------
        private void Step05_Append()
        {
            LogHeader("П.5. Добавление строки в конец file1.txt (ReadFile, SetFilePointer, LockFile, _lwrite, UnlockFile)");
            if (!EnsureFile1())
                return;

            int myFile = _lopen(File1, OF_READWRITE | OF_SHARE_DENY_NONE);
            if (myFile == HFILE_ERROR)
            {
                int err = Marshal.GetLastWin32Error();
                ShowApiError("_lopen(OF_READWRITE)", err,
                    err == ERROR_ACCESS_DENIED ? "Возможно, у файла установлен атрибут «только чтение» (п.6). Выполните п.2 заново." : null);
                return;
            }
            IntPtr hFile = new IntPtr(myFile);   // HFILE из _lopen — это дескриптор Win32
            try
            {
                var buff = new byte[4096];
                if (!ReadFile(hFile, buff, (uint)buff.Length, out uint dwBytesRead, IntPtr.Zero))
                {
                    ShowApiError("ReadFile", Marshal.GetLastWin32Error());
                    return;
                }
                Log($"ReadFile: прочитано {dwBytesRead} байт: \"{Ansi.GetString(buff, 0, (int)dwBytesRead)}\"");

                uint dwPos = SetFilePointer(hFile, 0, IntPtr.Zero, FILE_END);
                if (dwPos == INVALID_SET_FILE_POINTER)
                {
                    ShowApiError("SetFilePointer", Marshal.GetLastWin32Error());
                    return;
                }
                Log($"SetFilePointer(0, FILE_END) → позиция {dwPos}");

                byte[] buffer = Ansi.GetBytes(LineBreak + Text2);
                uint count = (uint)buffer.Length;
                if (!LockFile(hFile, dwPos, 0, count, 0))
                {
                    ShowApiError("LockFile", Marshal.GetLastWin32Error());
                    return;
                }
                Log($"LockFile: заблокирована область [{dwPos}; {dwPos + count}) — {count} байт");

                uint written = _lwrite(myFile, buffer, count);
                if (written == unchecked((uint)HFILE_ERROR))
                {
                    int err = Marshal.GetLastWin32Error();
                    UnlockFile(hFile, dwPos, 0, count, 0);
                    ShowApiError("_lwrite", err);
                    return;
                }
                Log($"_lwrite: дописано {written} байт: \"\\r\\n{Text2}\"");

                DemonstrateLock(dwPos, count);

                if (!UnlockFile(hFile, dwPos, 0, count, 0))
                {
                    ShowApiError("UnlockFile", Marshal.GetLastWin32Error());
                    return;
                }
                Log("UnlockFile: область разблокирована");
            }
            finally
            {
                _lclose(myFile);
            }

            byte[] data = ReadWholeFile(File1);
            if (data == null)
                return;
            Log($"Содержимое файла после добавления ({data.Length} байт):");
            foreach (string line in Ansi.GetString(data).Split(new[] { LineBreak }, StringSplitOptions.None))
                Log("  " + line);
        }

        /// <summary>
        /// Пока область заблокирована, пробуем прочитать её через другой дескриптор —
        /// система должна отказать с ошибкой ERROR_LOCK_VIOLATION.
        /// </summary>
        private void DemonstrateLock(uint pos, uint count)
        {
            IntPtr h2 = CreateFile(File1, GENERIC_READ, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero,
                OPEN_EXISTING, 0, IntPtr.Zero);
            if (h2 == INVALID_HANDLE_VALUE)
            {
                Log("  (проверка блокировки: второй дескриптор открыть не удалось — " +
                    ErrorText(Marshal.GetLastWin32Error()) + ")");
                return;
            }
            try
            {
                SetFilePointer(h2, (int)pos, IntPtr.Zero, FILE_BEGIN);
                var tmp = new byte[count];
                if (ReadFile(h2, tmp, count, out uint n, IntPtr.Zero))
                    Log($"  Проверка: чтение заблокированной области через другой дескриптор — прочитано {n} байт");
                else
                    Log("  Проверка: чтение заблокированной области через другой дескриптор — ОТКАЗ, " +
                        ErrorText(Marshal.GetLastWin32Error()));
            }
            finally
            {
                CloseHandle(h2);
            }
        }

        // ------------------------------------------------------------------
        //  П.6. Атрибуты «только чтение» + «скрытый», попытка удаления
        // ------------------------------------------------------------------
        private void Step06_Attributes()
        {
            LogHeader("П.6. Установка атрибутов «только чтение» и «скрытый» (SetFileAttributes), удаление (DeleteFile)");
            if (!EnsureFile1())
                return;

            Log("Атрибуты до:    " + AttrLong(GetFileAttributes(File1)));
            uint attributes = FILE_ATTRIBUTE_HIDDEN | FILE_ATTRIBUTE_READONLY;
            if (!SetFileAttributes(File1, attributes))
            {
                ShowApiError("SetFileAttributes", Marshal.GetLastWin32Error());
                return;
            }
            Log("SetFileAttributes(FILE_ATTRIBUTE_HIDDEN or FILE_ATTRIBUTE_READONLY) — успешно");
            Log("Атрибуты после: " + AttrLong(GetFileAttributes(File1)));

            Log($"Попытка удаления: DeleteFile(\"{File1}\")");
            if (DeleteFile(File1))
            {
                Log("  Файл удалён!");
                ShowWarning("Файл был удалён, несмотря на атрибут «только чтение».");
            }
            else
            {
                int err = Marshal.GetLastWin32Error();
                Log("  Удаление не выполнено: " + ErrorText(err));
                if (!batchMode)
                    ShowWarning("Удалить файл file1.txt не удалось!\n\n" + ErrorText(err) +
                                "\n\nФайл защищён атрибутом «только чтение».", "П.6. Удаление файла");
            }
            ShowStructure();
        }

        // ------------------------------------------------------------------
        //  П.7. Снять «скрытый», изменить дату и время создания
        // ------------------------------------------------------------------
        private void Step07_FileTime()
        {
            LogHeader("П.7. Снятие атрибута «скрытый» и изменение даты/времени создания (SetFileTime)");
            if (!EnsureFile1())
                return;

            uint attrs = GetFileAttributes(File1);
            Log("Атрибуты до:    " + AttrLong(attrs));
            if (!SetFileAttributes(File1, attrs & ~FILE_ATTRIBUTE_HIDDEN))
            {
                ShowApiError("SetFileAttributes", Marshal.GetLastWin32Error());
                return;
            }
            Log("Атрибуты после: " + AttrLong(GetFileAttributes(File1)));

            // FILE_WRITE_ATTRIBUTES позволяет менять время даже у файла «только для чтения»
            IntPtr hFile = CreateFile(File1, FILE_WRITE_ATTRIBUTES | GENERIC_READ, FILE_SHARE_READ | FILE_SHARE_WRITE,
                IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);
            if (hFile == INVALID_HANDLE_VALUE)
            {
                ShowApiError("CreateFile", Marshal.GetLastWin32Error());
                return;
            }
            try
            {
                GetFileTime(hFile, out long created, out long accessed, out long modified);
                Log("Дата создания до:    " + FormatFileTime(created));

                DateTime dt = dtpCreation.Value;
                var st = new SYSTEMTIME
                {
                    wYear = (ushort)dt.Year, wMonth = (ushort)dt.Month, wDay = (ushort)dt.Day,
                    wHour = (ushort)dt.Hour, wMinute = (ushort)dt.Minute, wSecond = (ushort)dt.Second
                };
                // локальное время → FILETIME (локальный) → FILETIME (UTC)
                if (!SystemTimeToFileTime(ref st, out long localFt) ||
                    !LocalFileTimeToFileTime(ref localFt, out long utcFt))
                {
                    ShowApiError("SystemTimeToFileTime", Marshal.GetLastWin32Error());
                    return;
                }
                if (!SetFileTime(hFile, ref utcFt, IntPtr.Zero, IntPtr.Zero))
                {
                    ShowApiError("SetFileTime", Marshal.GetLastWin32Error());
                    return;
                }
                GetFileTime(hFile, out created, out accessed, out modified);
                Log("SetFileTime — успешно");
                Log("Дата создания после: " + FormatFileTime(created));
                Log("Дата изменения:      " + FormatFileTime(modified));
                Log("Дата доступа:        " + FormatFileTime(accessed));
            }
            finally
            {
                CloseHandle(hFile);
            }
            ShowStructure();
        }

        // ------------------------------------------------------------------
        //  П.8. Переименование
        // ------------------------------------------------------------------
        private void Step08_Rename()
        {
            LogHeader("П.8. Переименование file1.txt → file2.txt (MoveFile)");
            if (!EnsureFile1())
                return;
            if (!PrepareTarget(File2))
                return;

            if (!MoveFile(File1, File2))
            {
                int err = Marshal.GetLastWin32Error();
                Log("MoveFile — ошибка " + ErrorText(err));
                ShowApiError("MoveFile", err);
                return;
            }
            Log($"MoveFile(\"{File1}\", \"{File2}\") — файл переименован");
            ShowStructure();
        }

        // ------------------------------------------------------------------
        //  П.9. Пересылка в S2 и копирование в S3
        // ------------------------------------------------------------------
        private void Step09_MoveCopy()
        {
            LogHeader("П.9. Пересылка file2.txt в S2 (MoveFile) и копирование в S3 (CopyFile)");
            if (!EnsureS1())
                return;

            string inS2 = Path.Combine("S2", File2);
            string inS3 = Path.Combine("S3", File2);

            if (FileExists(File2))
            {
                if (!PrepareTarget(inS2))
                    return;
                if (!MoveFile(File2, inS2))
                {
                    ShowApiError("MoveFile", Marshal.GetLastWin32Error());
                    return;
                }
                Log($"MoveFile(\"{File2}\", \"{inS2}\") — файл переслан в S2");
            }
            else if (FileExists(inS2))
            {
                Log($"Файл {File2} уже находится в S2 — пересылка не требуется.");
            }
            else
            {
                ShowWarning($"Файл {File2} не найден ни в S1, ни в S2.\nСначала выполните п.8.");
                return;
            }

            if (!PrepareTarget(inS3))
                return;
            if (!CopyFile(inS2, inS3, true))
            {
                ShowApiError("CopyFile", Marshal.GetLastWin32Error());
                return;
            }
            Log($"CopyFile(\"{inS2}\", \"{inS3}\") — файл скопирован в S3");
            Log();

            LogDirectory("S1", s1Dir);
            LogDirectory("S2", Path.Combine(s1Dir, "S2"));
            LogDirectory("S3", Path.Combine(s1Dir, "S3"));
            Log();
            ShowStructure();
        }

        // ------------------------------------------------------------------
        //  П.10–12. Метка тома
        // ------------------------------------------------------------------
        private void FillDrives()
        {
            cmbDrive.Items.Clear();
            var buf = new char[512];
            uint len = GetLogicalDriveStrings((uint)buf.Length, buf);
            string currentRoot = Path.GetPathRoot(startDir);
            int selected = -1;

            foreach (string root in new string(buf, 0, (int)len).Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string type;
                switch (GetDriveType(root))
                {
                    case DRIVE_REMOVABLE: type = "съёмный"; break;
                    case DRIVE_FIXED: type = "жёсткий"; break;
                    case DRIVE_REMOTE: type = "сетевой"; break;
                    case DRIVE_CDROM: type = "CD/DVD"; break;
                    case DRIVE_RAMDISK: type = "RAM-диск"; break;
                    default: type = "?"; break;
                }
                int idx = cmbDrive.Items.Add(new DriveItem { Root = root, Text = $"{root}  ({type})" });
                if (string.Equals(root, currentRoot, StringComparison.OrdinalIgnoreCase))
                    selected = idx;
            }
            if (cmbDrive.Items.Count > 0)
                cmbDrive.SelectedIndex = selected >= 0 ? selected : 0;
            toolTip.SetToolTip(cmbDrive, "По умолчанию — текущий диск. Для системного диска нужны права администратора;\n" +
                                         "удобнее использовать флешку.");
        }

        private string SelectedRoot()
        {
            if (cmbDrive.SelectedItem is DriveItem d)
                return d.Root;
            ShowWarning("Не выбран диск.");
            return null;
        }

        private void Step10_SetLabel()
        {
            LogHeader("П.10. Установка метки тома (SetVolumeLabel)");
            string root = SelectedRoot();
            if (root == null)
                return;
            string label = txtLabel.Text.Trim();
            if (label.Length == 0)
            {
                ShowWarning("Введите метку тома (по заданию — LABEL).");
                return;
            }

            Log($"SetVolumeLabel(\"{root}\", \"{label}\")");
            if (SetVolumeLabel(root, label))
            {
                Log("  Метка тома установлена.");
                ShowInfo($"Метка тома диска {root} установлена: {label}", "П.10");
                return;
            }

            int err = Marshal.GetLastWin32Error();
            Log("  Ошибка: " + ErrorText(err));
            if (err == ERROR_ACCESS_DENIED && !IsAdministrator())
            {
                if (MessageBox.Show(this,
                        $"Не удалось установить метку диска {root}:\n{ErrorText(err)}\n\n" +
                        "Изменение метки требует прав администратора.\n" +
                        "Перезапустить программу от имени администратора?",
                        "П.10. Отказано в доступе", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    RestartAsAdmin();
            }
            else
            {
                ShowApiError("SetVolumeLabel", err, "Диск может быть защищён от записи или не поддерживать метки.");
            }
        }

        /// <summary>Читает метку тома и сравнивает её с заданной в п.10. Возвращает метку или null при ошибке.</summary>
        private string CheckVolumeLabel(string caption)
        {
            string root = SelectedRoot();
            if (root == null)
                return null;

            var volName = new StringBuilder(261);
            var fsName = new StringBuilder(261);
            if (!GetVolumeInformation(root, volName, (uint)volName.Capacity, out uint serial, out uint maxComp,
                    out uint flags, fsName, (uint)fsName.Capacity))
            {
                int err = Marshal.GetLastWin32Error();
                Log("GetVolumeInformation — ошибка " + ErrorText(err));
                ShowApiError("GetVolumeInformation", err, "Возможно, в дисководе нет носителя.");
                return null;
            }

            string label = volName.ToString();
            string expected = txtLabel.Text.Trim();
            Log($"GetVolumeInformation(\"{root}\"):");
            Log($"  Метка тома:              {(label.Length > 0 ? label : "(нет)")}");
            Log($"  Серийный номер тома:     {serial >> 16:X4}-{serial & 0xFFFF:X4}");
            Log($"  Файловая система:        {fsName}");
            Log($"  Макс. длина имени файла: {maxComp}");
            Log($"  Флаги файловой системы:  ${flags:X8}");

            if (label.Length == 0)
            {
                Log($"  Результат: метка тома ОТСУТСТВУЕТ (ожидалась «{expected}»).");
                ShowWarning($"На диске {root} метка тома отсутствует.\nОжидалась метка «{expected}».", caption);
            }
            else if (string.Equals(label, expected, StringComparison.OrdinalIgnoreCase))
            {
                Log($"  Результат: метка СОВПАДАЕТ с заданной в п.10 («{expected}»).");
                ShowInfo($"Метка тома диска {root}: «{label}»\n\nМетка совпадает с заданной в п.10.", caption);
            }
            else
            {
                Log($"  Результат: метка НЕ совпадает с заданной («{expected}»).");
                ShowWarning($"Метка тома диска {root}: «{label}»\n\nМетка НЕ совпадает с заданной в п.10 («{expected}»).",
                    caption);
            }
            return label;
        }

        private void Step11_CheckLabel()
        {
            LogHeader("П.11. Поиск метки тома и сравнение с заданной (GetVolumeInformation)");
            CheckVolumeLabel("П.11. Метка тома");
        }

        private async void Step12_OsLabel()
        {
            // async void: исключения после await не дойдут до RunStep, поэтому перехватываем их здесь
            try
            {
                await Step12Core();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Непредвиденная ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task Step12Core()
        {
            LogHeader("П.12. Изменение/удаление метки средствами ОС и повторная проверка");
            string root = SelectedRoot();
            if (root == null)
                return;
            string drive = root.TrimEnd('\\');

            DialogResult r = MessageBox.Show(this,
                $"Измените метку диска {drive} средствами операционной системы:\n" +
                $"  • Проводник → правая кнопка на диске → Свойства → поле метки;\n" +
                $"  • или команда LABEL {drive} в командной строке (от администратора).\n" +
                "Чтобы стереть метку: очистите поле в свойствах диска, либо в утилите LABEL\n" +
                "нажмите Enter на пустой строке и подтвердите удаление (Y).\n\n" +
                "Да — запустить утилиту LABEL сейчас (с последующей проверкой);\n" +
                "Нет — метка уже изменена/стёрта, сразу проверить;\n" +
                "Отмена — ничего не делать.",
                "П.12. Изменение метки средствами ОС", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (r == DialogResult.Cancel)
                return;

            if (r == DialogResult.Yes)
            {
                Process p;
                try
                {
                    p = Process.Start(new ProcessStartInfo("cmd.exe", $"/c vol {drive} & label {drive} & vol {drive} & pause")
                    {
                        UseShellExecute = true,
                        Verb = "runas"
                    });
                }
                catch (Win32Exception ex)
                {
                    Log("Запуск утилиты LABEL отменён: " + ex.Message);
                    ShowWarning("Запуск утилиты LABEL отменён.");
                    return;
                }
                Log($"Запущена утилита: cmd /c label {drive}. Ожидание завершения...");
                SetStatus("Ожидание завершения утилиты LABEL...");
                if (p != null)
                {
                    try
                    {
                        await Task.Run(() => p.WaitForExit());
                    }
                    catch (Exception ex)
                    {
                        Log("Не удалось дождаться завершения: " + ex.Message);
                    }
                    finally
                    {
                        p.Dispose();
                    }
                }
                Log("Утилита LABEL завершена.");
            }

            Log("Проверка метки после изменения средствами ОС:");
            string label = CheckVolumeLabel("П.12. Проверка метки");
            if (label == null)
                return;
            if (label.Length == 0)
                Log("Вывод: метка стёрта — программа корректно сообщает об отсутствии метки.");
            else if (!string.Equals(label, txtLabel.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                Log($"Вывод: метка изменена средствами ОС на «{label}» — программа обнаруживает несовпадение.");
            else
                Log("Вывод: метка не изменялась. Измените или сотрите её и повторите п.12.");
            UpdateCurDirLabel();
        }

        // ------------------------------------------------------------------
        //  Удаление созданной структуры
        // ------------------------------------------------------------------
        private void RemoveStructure()
        {
            LogHeader("Удаление каталожной структуры");
            SetCurrentDirectory(startDir);
            if (!DirExists(rootDir))
            {
                Log("Структура не найдена.");
                ShowInfo("Каталожная структура не найдена — удалять нечего.");
                ShowStructure();
                return;
            }
            if (MessageBox.Show(this, "Удалить каталог " + rootDir + " со всем содержимым?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            bool ok = DeleteTree(rootDir);
            ShowStructure();
            if (ok)
                ShowInfo("Каталожная структура удалена.");
            else
                ShowWarning("Не все объекты удалось удалить. Подробности — в протоколе.");
        }

        private bool DeleteTree(string dir)
        {
            bool ok = true;
            foreach (WIN32_FIND_DATA fd in ListEntries(dir))
            {
                string path = Path.Combine(dir, fd.cFileName);
                if ((fd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0)
                {
                    ok &= DeleteTree(path);
                    continue;
                }
                SetFileAttributes(path, FILE_ATTRIBUTE_NORMAL);
                if (DeleteFile(path))
                {
                    Log("  DeleteFile:      " + path);
                }
                else
                {
                    Log($"  DeleteFile:      ОШИБКА {ErrorText(Marshal.GetLastWin32Error())} — {path}");
                    ok = false;
                }
            }
            if (RemoveDirectory(dir))
            {
                Log("  RemoveDirectory: " + dir);
            }
            else
            {
                Log($"  RemoveDirectory: ОШИБКА {ErrorText(Marshal.GetLastWin32Error())} — {dir}");
                ok = false;
            }
            return ok;
        }
    }
}
