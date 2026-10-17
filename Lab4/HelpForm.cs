using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab4
{
    /// <summary>
    /// Справочная система приложения: дерево разделов и текст выбранного раздела.
    /// Вызывается клавишей F1 или через меню «Справка → Содержание».
    /// </summary>
    public class HelpForm : Form
    {
        public const string TopicAbout = "about";
        public const string TopicFiles = "files";
        public const string TopicLabel = "label";
        public const string TopicMemory = "memory";
        public const string TopicClipboard = "clipboard";
        public const string TopicErrors = "errors";
        public const string TopicQuestions = "questions";

        private static HelpForm instance;

        private readonly TreeView tree;
        private readonly RichTextBox text;

        public static void ShowHelp(IWin32Window owner, string topic)
        {
            if (instance == null || instance.IsDisposed)
                instance = new HelpForm();
            instance.SelectTopic(topic);
            if (!instance.Visible)
                instance.Show(owner);
            instance.Activate();
        }

        private HelpForm()
        {
            Text = "Справка — Лабораторная работа 4";
            Size = new Size(900, 620);
            MinimumSize = new Size(600, 400);
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F);
            ShowInTaskbar = false;
            KeyPreview = true;
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) Close(); };

            text = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F),
                DetectUrls = false
            };
            var textPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12, 8, 8, 8), BackColor = Color.White };
            textPanel.Controls.Add(text);

            tree = new TreeView { Dock = DockStyle.Left, Width = 260, HideSelection = false, Font = new Font("Segoe UI", 10F) };
            tree.AfterSelect += (s, e) => ShowTopic(e.Node.Tag as string);

            var root = new TreeNode("Лабораторная работа 4") { Tag = TopicAbout };
            root.Nodes.Add(new TreeNode("4.2.1. Файловые операции") { Tag = TopicFiles });
            root.Nodes.Add(new TreeNode("Метка тома (п.10–12)") { Tag = TopicLabel });
            root.Nodes.Add(new TreeNode("4.2.2. Работа с памятью") { Tag = TopicMemory });
            root.Nodes.Add(new TreeNode("4.2.3. Буфер обмена") { Tag = TopicClipboard });
            root.Nodes.Add(new TreeNode("Сообщения и особые ситуации") { Tag = TopicErrors });
            root.Nodes.Add(new TreeNode("Контрольные вопросы") { Tag = TopicQuestions });
            tree.Nodes.Add(root);
            root.Expand();

            Controls.Add(textPanel);
            Controls.Add(new Splitter { Dock = DockStyle.Left, Width = 4 });
            Controls.Add(tree);
        }

        private void SelectTopic(string topic)
        {
            foreach (TreeNode n in tree.Nodes[0].Nodes)
            {
                if ((string)n.Tag == topic)
                {
                    tree.SelectedNode = n;
                    return;
                }
            }
            tree.SelectedNode = tree.Nodes[0];
        }

        /// <summary>
        /// Простейшая разметка: строка «# …» — заголовок, «## …» — подзаголовок, «• …» — пункт списка.
        /// </summary>
        private void ShowTopic(string topic)
        {
            text.Clear();
            var h1 = new Font("Segoe UI", 14F, FontStyle.Bold);
            var h2 = new Font("Segoe UI", 11F, FontStyle.Bold);
            var normal = new Font("Segoe UI", 10F);
            foreach (string raw in GetTopicText(topic).Replace("\r", "").Split('\n'))
            {
                string line = raw;
                Font f = normal;
                Color c = Color.Black;
                if (line.StartsWith("## "))
                {
                    line = line.Substring(3);
                    f = h2;
                    c = Color.FromArgb(28, 52, 92);
                }
                else if (line.StartsWith("# "))
                {
                    line = line.Substring(2);
                    f = h1;
                    c = Color.FromArgb(28, 52, 92);
                }
                text.SelectionStart = text.TextLength;
                text.SelectionFont = f;
                text.SelectionColor = c;
                text.AppendText(line + "\n");
            }
            text.SelectionStart = 0;
            text.ScrollToCaret();
        }

        private static string GetTopicText(string topic)
        {
            switch (topic)
            {
                case TopicFiles: return FilesText;
                case TopicLabel: return LabelText;
                case TopicMemory: return MemoryText;
                case TopicClipboard: return ClipboardText;
                case TopicErrors: return ErrorsText;
                case TopicQuestions: return QuestionsText;
                default: return AboutText;
            }
        }

        private const string AboutText =
@"# Лабораторная работа № 4
Создание приложений Windows с использованием средств прикладного программирования Win32 API.

Приложение демонстрирует прямой вызов системных функций Win32 API из программы на C# (Windows Forms) с помощью механизма Platform Invoke (атрибут DllImport). Все объявления функций, констант и структур собраны в файле NativeMethods.cs.

## Структура окна
• Вкладка «4.2.1. Файловые операции» — пункты 1–12 задания: кнопки слева, дерево каталогов и протокол выполнения справа.
• Вкладка «4.2.2. Память» — GetSystemInfo, GlobalMemoryStatus, VirtualQuery и карта адресного пространства.
• Вкладка «4.2.3. Буфер обмена» — определение форматов буфера обмена (по примеру из методички), запись и чтение данных.

## Меню
• Файл — перезапуск от имени администратора (нужен для смены метки системного диска), выход.
• Файловые операции — пункты 1–12, выполнение п.1–9 подряд, удаление созданной структуры.
• Память — пункты 1)–3) части 4.2.2.
• Буфер обмена — определение форматов (F5), автообновление по таймеру, запись/чтение.
• Справка — эта справка (F1), сведения о программе.

Клавиша F1 открывает раздел справки, соответствующий активной вкладке.";

        private const string FilesText =
@"# 4.2.1. Файловые операции
Каталожная структура создаётся в текущем каталоге программы:
TESTDIR\S1, TESTDIR\S1\S2, TESTDIR\S1\S3, TESTDIR\S1\S2\S4, TESTDIR\S1\S3\S5.
После п.1 текущим каталогом становится S1 (SetCurrentDirectory). Состав каталогов задаётся массивом SubDirs в файле MainForm.Files.cs — его можно изменить под структуру из лабораторной работы № 3.

## Пункты задания и используемые функции
• 1. CreateDirectory — создание каталогов; SetCurrentDirectory; FindFirstFile/FindNextFile — вывод структуры.
• 2. OpenFile(OF_CREATE) — создание file1.txt в S1; _lwrite — запись строки «Текстовая строка»; _lclose.
• 3. _lopen(OF_READ), _llseek(FILE_END) — определение размера, _lread — чтение файла целиком.
• 4. _llseek(hFile, 4, FILE_BEGIN) — позиционирование на 5-й байт, _lread — чтение 8 байт («товая ст»).
• 5. _lopen(OF_READWRITE), ReadFile — чтение, SetFilePointer(FILE_END) — переход в конец, LockFile — блокировка области, _lwrite — запись «Дополнительная строка», UnlockFile. Пока область заблокирована, программа пытается прочитать её через второй дескриптор и получает отказ ERROR_LOCK_VIOLATION.
• 6. SetFileAttributes(FILE_ATTRIBUTE_HIDDEN or FILE_ATTRIBUTE_READONLY), затем DeleteFile — удаление невозможно (Отказано в доступе).
• 7. SetFileAttributes — снятие атрибута «скрытый»; CreateFile(FILE_WRITE_ATTRIBUTES) + SetFileTime — изменение даты и времени создания (значение задаётся в поле «Дата создания» слева).
• 8. MoveFile(file1.txt, file2.txt) — переименование.
• 9. MoveFile(file2.txt, S2\file2.txt) — пересылка, CopyFile(S2\file2.txt, S3\file2.txt) — копирование; вывод содержимого S1, S2, S3.
• 10–12. Метка тома — см. отдельный раздел.

Текст пишется в однобайтовой кодировке Windows-1251, поэтому каждый символ занимает ровно один байт — как в примерах на Delphi.

## Дополнительные команды
• «Выполнить п.1–9 подряд» — все файловые операции без промежуточных окон.
• «Удалить созданную структуру» — снимает атрибуты, удаляет файлы (DeleteFile) и каталоги (RemoveDirectory).
• Повторное выполнение пунктов допускается: если файл-приёмник уже существует, программа предложит его удалить.";

        private const string LabelText =
@"# Метка тома (п.10–12)
Диск выбирается в списке «Диск» (по умолчанию — текущий). Метка задаётся в поле «Метка тома» (по заданию — LABEL).

## П.10. SetVolumeLabel
Устанавливает метку тома. Изменение метки — привилегированная операция: для системного диска (обычно C:) программа должна быть запущена от имени администратора. При отказе в доступе программа предложит перезапуск с повышенными правами. Удобнее всего выполнять п.10–12 на флешке.

## П.11. GetVolumeInformation
Читает метку тома, серийный номер, имя файловой системы, максимальную длину имени и флаги. В файловой системе FAT метка хранится как специальная запись корневого каталога с атрибутом «метка тома» ($08); функция GetVolumeInformation возвращает её независимо от типа файловой системы. Программа сравнивает найденную метку с заданной в п.10 и сообщает результат.

## П.12. Изменение метки средствами ОС
1. Нажмите кнопку п.12 и выберите «Да» — откроется командная строка с утилитой LABEL (от имени администратора). Введите новую метку и нажмите Enter.
   Другой способ — Проводник → правая кнопка мыши на диске → Свойства → поле метки.
2. После закрытия окна утилиты программа автоматически проверит метку и сообщит, что она не совпадает с заданной.
3. Снова выполните п.12: в утилите LABEL нажмите Enter на пустой строке и подтвердите удаление метки (Y). Программа сообщит, что метка отсутствует.";

        private const string MemoryText =
@"# 4.2.2. Функции для работы с памятью
## 1) GetSystemInfo
Заполняет структуру SYSTEM_INFO: архитектура процессора, размер страницы (dwPageSize, на x86/x64 — 4 КБ), минимальный и максимальный адреса, доступные приложению, маска и число процессоров, гранулярность выделения памяти (64 КБ), уровень и модификация процессора. Строки вывода формируются по шаблонам из методички (SProc, SPageSize, SMinAddr …).

## 2) GlobalMemoryStatus
Заполняет структуру MEMORYSTATUS (перед вызовом поле dwLength = SizeOf(MEMORYSTATUS)): загрузка памяти в процентах, общий и свободный объём физической памяти, файла подкачки и виртуального адресного пространства процесса. Дополнительно вызывается GlobalMemoryStatusEx, корректно работающая с объёмами более 4 ГБ.

## 3) VirtualQuery
Возвращает MEMORY_BASIC_INFORMATION для региона, содержащего адрес lpAddress: BaseAddress, AllocationBase, AllocationProtect, RegionSize, State (MEM_COMMIT / MEM_RESERVE / MEM_FREE), Protect (PAGE_*), Type (MEM_IMAGE / MEM_MAPPED / MEM_PRIVATE).
Адрес выбирается из списка, построенного по результатам п.1 и п.2 (минимальный/максимальный адрес приложения, Min + гранулярность, Min + TotalVirtual/2), а также адреса образа программы, kernel32.dll, переменной в стеке и блока в куче; можно ввести адрес вручную (шестнадцатеричный).
Кнопка «Карта адресного пространства» обходит всё пространство от lpMinimumApplicationAddress до lpMaximumApplicationAddress, переходя к адресу BaseAddress + RegionSize, и сравнивает суммарный объём свободных регионов с ullAvailVirtual из п.2.";

        private const string ClipboardText =
@"# 4.2.3. Буфер обмена
## Определение форматов
Реализация примера из методички: CountClipboardFormats — число форматов; OpenClipboard; EnumClipboardFormats(0) и далее EnumClipboardFormats(предыдущий) — перебор форматов; GetClipboardFormatName — имя зарегистрированного формата (для предопределённых возвращает 0, и имя берётся из таблицы CF_TEXT, CF_BITMAP …); CloseClipboard.
Список обновляется по таймеру раз в секунду (флажок «Таймер» или пункт меню), но буфер открывается только при изменении номера последовательности GetClipboardSequenceNumber, чтобы не мешать другим программам. F5 — принудительное обновление. Также выводится владелец буфера (GetClipboardOwner).

## Проверка
Скопируйте что-нибудь в другой программе (текст в Блокноте или Word, рисунок в Paint, файл в Проводнике) — список форматов обновится автоматически.

## Запись и чтение
• SetClipboardData: OpenClipboard → EmptyClipboard → GlobalAlloc/GlobalLock/копирование/GlobalUnlock → SetClipboardData(CF_UNICODETEXT) → CloseClipboard.
• «+ собственный формат»: дополнительно RegisterClipboardFormat и запись данных в зарегистрированном формате (номер $C000–$FFFF).
• GetClipboardData: IsClipboardFormatAvailable → GetClipboardData → GlobalLock → чтение → GlobalUnlock для CF_UNICODETEXT, CF_TEXT и CF_OEMTEXT. Хотя записан только CF_UNICODETEXT, текст доступен и в CF_TEXT/CF_OEMTEXT — это автоматическое преобразование форматов (табл. 2).
• EmptyClipboard — очистка буфера обмена.";

        private const string ErrorsText =
@"# Сообщения и особые ситуации
Каждая функция Win32 API проверяется на ошибку; код ошибки получается через GetLastError (Marshal.GetLastWin32Error), а текст — системным сообщением (FormatMessage).

## Типичные ситуации
• Каталог уже существует (ERROR_ALREADY_EXISTS, 183) — сообщается в протоколе, работа продолжается.
• Диск защищён от записи / нет прав — сообщение об ошибке CreateDirectory.
• Каталог S1 или файл file1.txt не найден — программа подскажет, какой пункт выполнить раньше.
• Отказано в доступе (ERROR_ACCESS_DENIED, 5) при удалении файла с атрибутом «только чтение» (п.6) — ожидаемая ситуация.
• Нарушение блокировки (ERROR_LOCK_VIOLATION, 33) при чтении заблокированной области (п.5) — демонстрация LockFile.
• Файл-приёмник уже существует (п.8, п.9) — запрос на удаление существующего файла.
• Отказано в доступе при SetVolumeLabel (п.10) — предложение перезапуска от имени администратора.
• Нет носителя в дисководе — ошибка GetVolumeInformation.
• Метка тома отсутствует или не совпадает с заданной (п.11, п.12) — предупреждение.
• Недопустимый адрес для VirtualQuery (вне диапазона адресов приложения) — ошибка ERROR_INVALID_PARAMETER.
• Буфер обмена открыт другой программой — OpenClipboard возвращает False, список обновится позже.";

        private const string QuestionsText =
@"# Контрольные вопросы
## 1. Основные функции Windows для файловых операций
CreateDirectory, RemoveDirectory, SetCurrentDirectory/GetCurrentDirectory; CreateFile, OpenFile, _lopen, _lcreat; ReadFile, _lread; WriteFile, _lwrite; SetFilePointer, _llseek; CloseHandle, _lclose; LockFile/UnlockFile; GetFileAttributes/SetFileAttributes; GetFileTime/SetFileTime; CopyFile, MoveFile, DeleteFile; FindFirstFile/FindNextFile/FindClose; GetVolumeInformation, SetVolumeLabel, GetLogicalDriveStrings, GetDriveType.

## 2. Формат функций чтения и записи
_lread(hFile, lpBuffer, uBytes) и _lwrite(hFile, lpBuffer, uBytes): дескриптор файла, адрес буфера, число байт; возвращают число прочитанных/записанных байт или HFILE_ERROR.
ReadFile(hFile, lpBuffer, nNumberOfBytesToRead, lpNumberOfBytesRead, lpOverlapped) и WriteFile(…) — дополнительно возвращают фактическое число байт через указатель и поддерживают асинхронный ввод-вывод (lpOverlapped; при синхронном — nil). Чтение/запись идут с текущей позиции, которую меняют _llseek/SetFilePointer.

## 3. Назначение LockFile/UnlockFile
Блокируют область файла (смещение + длина) для монопольного доступа владельца дескриптора: другие процессы/дескрипторы при попытке чтения или записи этой области получают ERROR_LOCK_VIOLATION. Используются для согласованного совместного доступа к файлу (базы данных, журналы, общие файлы в сети). Каждую блокировку нужно снять UnlockFile с теми же параметрами.

## 4. Параметры GetSystemInfo и VirtualQuery
GetSystemInfo(var lpSystemInfo: TSystemInfo) — единственный параметр-структура, которую функция заполняет (см. раздел «Работа с памятью»).
VirtualQuery(lpAddress, lpBuffer, dwLength): адрес запроса; указатель на структуру MEMORY_BASIC_INFORMATION; размер этой структуры. Возвращает число скопированных байт (0 — ошибка).

## 5. Последовательность работы с буфером обмена
Запись: OpenClipboard(hWnd) → EmptyClipboard → GlobalAlloc, GlobalLock, копирование, GlobalUnlock → SetClipboardData(формат, hMem) (желательно в нескольких форматах) → CloseClipboard. После SetClipboardData памятью владеет система.
Чтение: IsClipboardFormatAvailable (или EnumClipboardFormats) → OpenClipboard → GetClipboardData(формат) → GlobalLock, копирование данных, GlobalUnlock → CloseClipboard.

## 6. Преобразование форматов
Windows автоматически синтезирует данные в родственных форматах (CF_TEXT ↔ CF_OEMTEXT ↔ CF_UNICODETEXT, CF_BITMAP ↔ CF_DIB ↔ CF_DIBV5, CF_METAFILEPICT ↔ CF_ENHMETAFILE). Программа-источник может поместить данные в одном формате, а получатель запросить их в другом — система выполнит преобразование при вызове GetClipboardData. Это расширяет круг программ, способных принять данные. Дополнительно приложение может регистрировать собственные форматы функцией RegisterClipboardFormat.

## 7. Демонстрация
Откройте вкладку «4.2.3. Буфер обмена», скопируйте данные в разных программах и наблюдайте список поддерживаемых форматов.";
    }
}
