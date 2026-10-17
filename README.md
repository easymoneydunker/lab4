# Лабораторная работа № 4 — подробный разбор

**Тема:** создание приложений Windows с использованием средств прикладного программирования Win32 API.

**Реализация:** C#, Windows Forms, .NET 8. Все действия из задания выполняются **прямыми вызовами
функций Win32 API** (`kernel32.dll`, `user32.dll`). Обёртки .NET (`System.IO.File`,
`Directory`, `Clipboard` и т.п.) для заданий намеренно не используются, чтобы было видно, какие
именно системные функции вызываются и в каком порядке.

---

## Содержание

1. [Как запустить](#1-как-запустить)
2. [Как вызывается Win32 API из C#](#2-как-вызывается-win32-api-из-c)
3. [Устройство проекта](#3-устройство-проекта)
4. [Часть 4.2.1 — файловые операции (п.1–12)](#4-часть-421--файловые-операции)
5. [Часть 4.2.2 — функции для работы с памятью](#5-часть-422--функции-для-работы-с-памятью)
6. [Часть 4.2.3 — буфер обмена](#6-часть-423--буфер-обмена)
7. [Дополнительные условия: заставка, меню, справка, сообщения](#7-дополнительные-условия)
8. [Ответы на контрольные вопросы](#8-ответы-на-контрольные-вопросы)

---

## 1. Как запустить

Нужны Windows и Visual Studio 2022 (рабочая нагрузка «Разработка классических приложений .NET»)
или установленный .NET 8 SDK.

* **Visual Studio:** открыть `Lab4.sln` и нажать F5.
* **Командная строка:** `dotnet run --project Lab4`

Для п.10–12 (метка тома) на системном диске нужны права администратора. Программа сама
предложит перезапуститься от имени администратора, если получит отказ в доступе. То же
можно сделать вручную: «Файл → Перезапустить от имени администратора». Удобнее всего
выполнять эти пункты на флешке: её можно выбрать в списке «Диск».

---

## 2. Как вызывается Win32 API из C#

В Delphi функции API объявлены в модуле `Windows`, и их можно сразу вызывать. В C# каждую
функцию нужно один раз **объявить** с атрибутом `[DllImport]`. Этот механизм называется
Platform Invoke (P/Invoke). Все объявления собраны в файле `Lab4/NativeMethods.cs`.

```csharp
[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
[return: MarshalAs(UnmanagedType.Bool)]
public static extern bool CreateDirectory(string lpPathName, IntPtr lpSecurityAttributes);
```

Что здесь происходит:

* `"kernel32.dll"` — библиотека, из которой берётся функция.
* `extern` — тело функции находится не в программе, а в DLL.
* `CharSet = CharSet.Unicode` — строки передаются в UTF-16, то есть вызывается
  `CreateDirectoryW`. У старых функций `OpenFile` и `_lopen` есть только ANSI-версия,
  поэтому для них указано `CharSet.Ansi`.
* `SetLastError = true` — сразу после вызова .NET сохраняет код ошибки. Его можно получить
  через `Marshal.GetLastWin32Error()`; это аналог `GetLastError` в Delphi.
* Типы сопоставляются так: `DWORD` → `uint`, `WORD` → `ushort`, `BOOL` → `bool`,
  `HANDLE`/`LPVOID` → `IntPtr`, `SIZE_T` → `UIntPtr`, `HFILE` → `int`, `LPCTSTR` → `string`.
  Если функция записывает строку в буфер (`LPTSTR`), передаётся `StringBuilder`.

Структуры (`SYSTEM_INFO`, `MEMORYSTATUS`, `MEMORY_BASIC_INFORMATION`, `WIN32_FIND_DATA`,
`OFSTRUCT`, `SYSTEMTIME`) описаны с `[StructLayout(LayoutKind.Sequential)]`: поля идут в памяти
в том же порядке и с тем же выравниванием, что и в C. Размеры структур сверены с Win32 x64:
`SYSTEM_INFO` = 48 байт, `MEMORYSTATUS` = 56, `MEMORYSTATUSEX` = 64,
`MEMORY_BASIC_INFORMATION` = 48, `WIN32_FIND_DATAW` = 592, `OFSTRUCT` = 136.

Сообщение об ошибке строится одинаково во всей программе (`MainForm.cs`):

```csharp
private static string ErrorText(int code)
{
    return $"код {code}: {new Win32Exception(code).Message}";   // текст берётся из FormatMessage
}

private void ShowApiError(string function, int code, string hint = null) { ... MessageBox ... }
```

Типичная проверка вызова в коде выглядит так:

```csharp
if (!SetFileAttributes(File1, attributes))
{
    ShowApiError("SetFileAttributes", Marshal.GetLastWin32Error());
    return;
}
```

---

## 3. Устройство проекта

| Файл | Что внутри |
|---|---|
| `Lab4/NativeMethods.cs` | Все объявления функций, структур и констант Win32 API |
| `Lab4/Program.cs` | Точка входа `Main`: регистрация кодировок, показ заставки, запуск главного окна |
| `Lab4/SplashForm.cs` | Заставка |
| `Lab4/MainForm.Designer.cs` | Разметка главного окна: меню, вкладки, кнопки, строка состояния |
| `Lab4/MainForm.cs` | Общие вещи: вывод ошибок, форматирование, «О программе», перезапуск от администратора |
| `Lab4/MainForm.Files.cs` | **4.2.1** — пункты 1–12 |
| `Lab4/MainForm.Memory.cs` | **4.2.2** — `GetSystemInfo`, `GlobalMemoryStatus`, `VirtualQuery` |
| `Lab4/MainForm.Clipboard.cs` | **4.2.3** — форматы буфера обмена |
| `Lab4/HelpForm.cs` | Справочная система (F1) |

`MainForm` — **частичный класс** (`partial class`): одна форма, но её код разнесён по файлам
по частям лабораторной. В конструкторе вызываются три функции инициализации:

```csharp
public MainForm()
{
    InitializeComponent();   // создание элементов из MainForm.Designer.cs
    InitFileSteps();         // кнопки и пункты меню п.1–12
    InitMemory();            // список адресов для VirtualQuery
    InitClipboard();         // включение таймера буфера обмена
}
```

В окне три вкладки, по одной на каждую часть задания:

* **«4.2.1. Файловые операции»**: слева кнопки п.1–12 и параметры (дата для п.7, диск и
  метка для п.10–12), в середине дерево каталогов, справа протокол выполнения.
* **«4.2.2. Память»**: кнопки и выбор адреса сверху, текстовый вывод (аналог `Memo1`)
  и таблица регионов памяти.
* **«4.2.3. Буфер обмена»**: число форматов, владелец буфера, список форматов (аналог
  `ListBox1`), кнопки записи и чтения.

---

## 4. Часть 4.2.1 — файловые операции

Весь код этой части находится в `Lab4/MainForm.Files.cs`.

### Общая подготовка

```csharp
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

private static Encoding Ansi => Encoding.GetEncoding(1251);
```

* Каталожная структура описана массивом `SubDirs`. Если в лабораторной № 3 структура
  другая, достаточно изменить этот массив.
* Строки записываются в файл в однобайтовой кодировке **Windows-1251**: один символ занимает
  один байт, как в примерах на Delphi. Поэтому «8 байт с 5-го байта» — это ровно 8 символов.
  Чтобы кодировка 1251 была доступна в .NET 8, в `Program.Main` вызывается
  `Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)`.
* При запуске запоминается текущий каталог: `startDir = GetCurDir()`. Внутри вызывается
  `GetCurrentDirectory`, поэтому структура создаётся «на текущем диске в текущем каталоге».

Кнопки п.1–12 и пункты меню «Файловые операции» создаются в `InitFileSteps()` из одного
массива. Поэтому кнопка и пункт меню всегда вызывают один и тот же метод:

```csharp
var steps = new (string Title, Action Run)[]
{
    ("Создать каталожную структуру (CreateDirectory)", Step01_CreateStructure),
    ("Создать file1.txt в S1 и записать строку (OpenFile, _lwrite)", Step02_CreateFile),
    ...
};
```

Каждый пункт запускается через `RunStep`. Он переключает вкладку, перехватывает
непредвиденные исключения и обновляет в строке состояния текущий каталог.

Вспомогательные функции, построенные поверх API:

| Функция | Что делает | Какие API использует |
|---|---|---|
| `GetCurDir()` | текущий каталог | `GetCurrentDirectory` |
| `DirExists` / `FileExists` | есть ли каталог или файл | `GetFileAttributes` (возвращает `INVALID_FILE_ATTRIBUTES`, если объекта нет) |
| `ListEntries(dir)` | содержимое каталога | `FindFirstFile`, `FindNextFile`, `FindClose` |
| `ShowStructure()` | дерево каталогов на экране и в протоколе | рекурсивно `ListEntries` |
| `ReadWholeFile(name)` | чтение файла целиком | `_lopen`, `_llseek`, `_lread`, `_lclose` |
| `EnsureS1()` / `EnsureFile1()` | проверка, что предыдущие пункты выполнены | `GetFileAttributes`, `SetCurrentDirectory` |
| `PrepareTarget(path)` | при повторном запуске удаляет мешающий файл-приёмник | `SetFileAttributes`, `DeleteFile` |

---

### П.1. Создать каталожную структуру и вывести её на экран

**Задание:** на текущем диске в текущем каталоге создать структуру каталогов из
лабораторной № 3 и показать её.

**Код (`Step01_CreateStructure`):**

```csharp
SetCurrentDirectory(startDir);                      // начинаем от каталога запуска

foreach (string dir in dirs)                        // TESTDIR, TESTDIR\S1, TESTDIR\S1\S2 ...
{
    if (CreateDirectory(dir, IntPtr.Zero))          // nil — атрибуты безопасности по умолчанию
    {
        Log("  CreateDirectory: создан     " + dir);
        continue;
    }
    int err = Marshal.GetLastWin32Error();
    if (err == ERROR_ALREADY_EXISTS)                // 183 — каталог уже есть, это не ошибка
        Log("  CreateDirectory: существует " + dir);
    else
    {
        ShowApiError("CreateDirectory", err,
            "Невозможно создать каталоги! Возможно, диск защищён от записи или нет прав доступа.");
        return;
    }
}

SetCurrentDirectory(s1Dir);                          // текущим становится S1
ShowStructure();
```

**Как это работает:**

1. Каталоги создаются строго сверху вниз: сначала родитель, потом вложенные.
   `CreateDirectory` не умеет создавать несколько уровней сразу.
2. Если каталог уже существует, `CreateDirectory` возвращает `False`, а `GetLastError` даёт
   `ERROR_ALREADY_EXISTS`. Программа считает это нормой и продолжает работу, поэтому
   пункт можно запускать повторно.
3. Любая другая ошибка (диск защищён от записи, нет прав) выводится сообщением. Это то же
   окно «Невозможно создать каталоги!», что в примере на Delphi.
4. `SetCurrentDirectory(S1)` делает S1 текущим каталогом. Дальше в п.2–9 используются
   относительные имена: `file1.txt`, `S2\file2.txt`.
5. `ShowStructure()` выводит структуру. Функция рекурсивно обходит каталоги через
   `FindFirstFile("каталог\*")` / `FindNextFile`, пропускает записи `.` и `..` и строит
   одновременно дерево `TreeView` и текстовое дерево в протоколе:

```
C:\...\TESTDIR\
└── S1\
    ├── S2\
    │   └── S4\
    └── S3\
        └── S5\
```

Позже в дереве появляются и файлы: размер и атрибуты в виде `R H S A` (только чтение,
скрытый, системный, архивный). Скрытые файлы показываются серым.

---

### П.2. Создать в S1 файл file1.txt и записать «Текстовая строка»

**Код (`Step02_CreateFile`):**

```csharp
var ofs = new OFSTRUCT();
int hFile = OpenFile(File1, ref ofs, OF_CREATE | OF_READWRITE);
if (hFile == HFILE_ERROR) { ... ShowApiError("OpenFile", err); return; }

byte[] buffer = Ansi.GetBytes(Text1);               // 16 байт в кодировке 1251
uint written = _lwrite(hFile, buffer, (uint)buffer.Length);
_lclose(hFile);
```

**Как это работает:**

* `OpenFile` — старая функция из Win16. В зависимости от флагов `uStyle` она создаёт,
  открывает или проверяет существование файла. С флагом `OF_CREATE` файл создаётся, а если
  он уже есть, обрезается до нулевой длины.
* Функция заполняет структуру `OFSTRUCT`. В поле `szPathName` оказывается полный путь
  к файлу, программа выводит его в протокол. Ограничение `OpenFile`: путь не длиннее
  128 символов (`OFS_MAXPATHNAME`) и только ANSI. Поэтому передаётся короткое
  относительное имя, ведь текущий каталог уже S1.
* Возвращается `HFILE` — целое число, дескриптор файла. При ошибке возвращается
  `HFILE_ERROR` (−1).
* `_lwrite(hFile, buffer, count)` записывает `count` байт из буфера. Строка предварительно
  переводится в байты 1251; в Delphi то же самое делает `@Buffer[1]`.
* `_lclose` закрывает файл.
* Если файл остался от прошлого запуска с атрибутами «только чтение» и «скрытый» из п.6,
  `OpenFile` не сможет его перезаписать. Поэтому перед созданием атрибуты сбрасываются
  через `SetFileAttributes(..., FILE_ATTRIBUTE_NORMAL)`.

---

### П.3. Прочитать file1.txt в память и вывести на экран

**Код (`ReadWholeFile`, вызывается из `Step03_ReadFile`):**

```csharp
int h = _lopen(name, OF_READ | OF_SHARE_DENY_NONE);
int size = _llseek(h, 0, FILE_END);       // перейти в конец → вернётся позиция = размер файла
_llseek(h, 0, FILE_BEGIN);                // вернуться в начало
var buf = new byte[size];
uint read = _lread(h, buf, (uint)buf.Length);
_lclose(h);
```

**Как это работает:**

* `_lopen` открывает существующий файл. `OF_READ` — только чтение, `OF_SHARE_DENY_NONE` —
  другим программам не запрещается открывать этот же файл.
* Размер файла определяется приёмом с `_llseek`: смещение 0 от конца (`FILE_END`)
  возвращает новую позицию, то есть длину файла. Затем указатель возвращается в начало.
* `_lread` читает весь файл в буфер, функция возвращает число прочитанных байт.
* На экран выводятся текст и побайтовый дамп с номерами байтов (с 1). По дампу удобно
  проверять п.4:

```
       1: D2 E5 EA F1 F2 EE E2 E0 FF 20 F1 F2 F0 EE EA E0  Текстовая строка
```

---

### П.4. Прочитать 8 байт, начиная с 5-го байта

**Код (`Step04_ReadPart`):**

```csharp
int hFile = _lopen(File1, OF_READ | OF_SHARE_DENY_NONE);
int pos = _llseek(hFile, 4, FILE_BEGIN);    // смещение 4 от начала = 5-й байт
var buff = new byte[8];
uint read = _lread(hFile, buff, 8);
_lclose(hFile);
```

**Как это работает:**

* Смещения в файле отсчитываются от нуля: байт № 1 имеет смещение 0, поэтому байт № 5 —
  смещение **4**. `_llseek(h, 4, FILE_BEGIN)` ставит указатель на него.
* `_lread` читает 8 байт с текущей позиции.
* Результат: `"товая ст"` (символы 5–12 строки «Текс**товая ст**рока»). Он выводится
  в протокол и в окне сообщения.

---

### П.5. Добавить в конец файла «Дополнительная строка»

Здесь используются функции из третьего примера методички: `ReadFile`, `SetFilePointer`,
`LockFile`, `UnlockFile`.

**Код (`Step05_Append`, сокращённо):**

```csharp
int myFile = _lopen(File1, OF_READWRITE | OF_SHARE_DENY_NONE);
IntPtr hFile = new IntPtr(myFile);          // HFILE из _lopen — это обычный дескриптор Win32

// 1. читаем до 4096 байт
ReadFile(hFile, buff, 4096, out uint dwBytesRead, IntPtr.Zero);

// 2. ставим указатель в конец файла
uint dwPos = SetFilePointer(hFile, 0, IntPtr.Zero, FILE_END);

// 3. блокируем область, куда будем писать
byte[] buffer = Ansi.GetBytes("\r\n" + Text2);
LockFile(hFile, dwPos, 0, (uint)buffer.Length, 0);

// 4. дописываем строку
_lwrite(myFile, buffer, (uint)buffer.Length);

// 5. (демонстрация) пробуем прочитать заблокированную область через другой дескриптор
DemonstrateLock(dwPos, count);

// 6. снимаем блокировку
UnlockFile(hFile, dwPos, 0, (uint)buffer.Length, 0);
_lclose(myFile);
```

**Как это работает:**

* `OF_READWRITE` открывает файл и для чтения, и для записи.
* `ReadFile(hFile, буфер, сколько, out сколько_прочитано, nil)` — функция Win32. В отличие
  от `_lread`, число прочитанных байт она возвращает через отдельный параметр, а сама
  возвращает `True`/`False`. Последний параметр `lpOverlapped = nil` означает синхронный
  ввод-вывод.
* Дескриптор из `_lopen` можно передавать в функции Win32 (`ReadFile`, `SetFilePointer`,
  `LockFile`). Пример на Delphi делает то же самое.
* `SetFilePointer(h, 0, nil, FILE_END)` переводит указатель в конец и возвращает новую
  позицию, то есть текущую длину файла. `nil` в третьем параметре значит, что старшие
  32 бита смещения не используются.
* `LockFile(h, смещение_мл, смещение_ст, длина_мл, длина_ст)` блокирует область
  `[dwPos; dwPos + длина)`. Блокировать можно и область за концом файла: именно туда
  и будет запись.
* Пока область заблокирована, `DemonstrateLock` открывает файл второй раз через
  `CreateFile` и пытается прочитать эту область через `ReadFile`. Система отказывает
  с ошибкой **`ERROR_LOCK_VIOLATION` (33)**. Это видно в протоколе и подтверждает, что
  блокировка действует для всех дескрипторов, кроме заблокировавшего.
* Сам заблокировавший дескриптор писать может, поэтому `_lwrite` проходит успешно.
* Перед добавляемой строкой пишется перевод строки `\r\n`, чтобы в Блокноте она была
  на новой строке.
* После `UnlockFile` и `_lclose` файл читается заново (`ReadWholeFile`), и на экран
  выводится:

```
  Текстовая строка
  Дополнительная строка
```

---

### П.6. Атрибуты «только чтение» и «скрытый», попытка удаления

**Код (`Step06_Attributes`):**

```csharp
uint attributes = FILE_ATTRIBUTE_HIDDEN | FILE_ATTRIBUTE_READONLY;   // 0x02 | 0x01
SetFileAttributes(File1, attributes);
Log("Атрибуты после: " + AttrLong(GetFileAttributes(File1)));

if (DeleteFile(File1))
    ...                                  // не должно произойти
else
{
    int err = Marshal.GetLastWin32Error();          // 5 = ERROR_ACCESS_DENIED
    ShowWarning("Удалить файл file1.txt не удалось!\n\n" + ErrorText(err) + ...);
}
```

**Как это работает:**

* Атрибуты — битовые флаги, поэтому они объединяются через `or` (`|` в C#).
* `SetFileAttributes` **заменяет** набор атрибутов целиком. После вызова у файла ровно
  `READONLY + HIDDEN`, а атрибут «архивный» исчезает.
* `GetFileAttributes` читает атрибуты обратно, `AttrLong` их расшифровывает:
  `$0003 (только чтение, скрытый)`.
* `DeleteFile` для файла «только для чтения» возвращает `False`, ошибка
  **`ERROR_ACCESS_DENIED` (5) — «Отказано в доступе»**. Это и есть ожидаемая «особая
  ситуация»: программа показывает предупреждение. Атрибут «скрытый» на удаление не
  влияет, мешает именно «только чтение».

---

### П.7. Снять атрибут «скрытый» и изменить дату и время создания

**Код (`Step07_FileTime`, сокращённо):**

```csharp
uint attrs = GetFileAttributes(File1);
SetFileAttributes(File1, attrs & ~FILE_ATTRIBUTE_HIDDEN);   // убираем только бит HIDDEN

IntPtr hFile = CreateFile(File1, FILE_WRITE_ATTRIBUTES | GENERIC_READ,
    FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);

// дата из поля «Дата создания (п.7)» → SYSTEMTIME
var st = new SYSTEMTIME { wYear = ..., wMonth = ..., wDay = ..., wHour = ..., ... };
SystemTimeToFileTime(ref st, out long localFt);      // SYSTEMTIME → FILETIME (локальное)
LocalFileTimeToFileTime(ref localFt, out long utcFt); // локальное → UTC
SetFileTime(hFile, ref utcFt, IntPtr.Zero, IntPtr.Zero); // меняем только время создания

GetFileTime(hFile, out created, out accessed, out modified); // проверяем результат
CloseHandle(hFile);
```

**Как это работает:**

* Чтобы снять один атрибут и не трогать остальные, текущие атрибуты читаются и из них
  убирается нужный бит: `attrs and not FILE_ATTRIBUTE_HIDDEN`. Атрибут «только чтение»
  остаётся.
* Для `SetFileTime` нужен дескриптор `HANDLE`, поэтому файл открывается через `CreateFile`.
  Нужно право **`FILE_WRITE_ATTRIBUTES`**: оно позволяет менять время даже у файла
  «только для чтения». Запросить `GENERIC_WRITE` для такого файла не получилось бы.
* Время в Windows хранится в формате `FILETIME`: 64-битное число интервалов по 100 нс
  с 01.01.1601, **в UTC**. Поэтому дата из поля проходит цепочку
  `SYSTEMTIME → SystemTimeToFileTime → LocalFileTimeToFileTime`.
* В `SetFileTime(h, создание, доступ, изменение)` вместо двух последних параметров
  передаётся `nil` (`IntPtr.Zero`), чтобы они не изменились.
* Для вывода время переводится обратно: `FileTimeToLocalFileTime → FileTimeToSystemTime`
  (функция `FormatFileTime`). В протоколе видна дата создания «до» и «после».

---

### П.8. Переименовать file1.txt в file2.txt

**Код (`Step08_Rename`):**

```csharp
if (!PrepareTarget(File2)) return;   // если file2.txt уже есть (повторный запуск) — удалить с подтверждения
if (!MoveFile(File1, File2)) { ShowApiError("MoveFile", ...); return; }
```

**Как это работает:** переименование — частный случай перемещения в тот же каталог,
поэтому используется `MoveFile(старое_имя, новое_имя)`. Если файл с новым именем уже
существует, `MoveFile` возвращает ошибку `ERROR_ALREADY_EXISTS`. Чтобы избежать её,
`PrepareTarget` заранее предлагает удалить мешающий файл. Атрибут «только чтение»
переименованию не мешает.

---

### П.9. Переслать file2.txt в S2, скопировать в S3, показать содержимое S1, S2, S3

**Код (`Step09_MoveCopy`):**

```csharp
string inS2 = Path.Combine("S2", File2);    // "S2\file2.txt" — относительно S1
string inS3 = Path.Combine("S3", File2);

MoveFile(File2, inS2);                       // пересылка: файл исчезает из S1
CopyFile(inS2, inS3, true);                  // копирование: bFailIfExists = True

LogDirectory("S1", s1Dir);
LogDirectory("S2", ...);
LogDirectory("S3", ...);
```

**Как это работает:**

* `MoveFile` в другой каталог того же тома просто переносит запись каталога: данные
  не копируются, файл пропадает из S1.
* `CopyFile(откуда, куда, bFailIfExists)` создаёт копию. При `True` функция не
  перезаписывает существующий файл, поэтому перед вызовом снова срабатывает `PrepareTarget`.
  Копия наследует атрибуты оригинала, в том числе «только чтение».
* `LogDirectory` выводит таблицу по данным `FindFirstFile`/`FindNextFile`: имя, размер
  (`nFileSizeHigh:nFileSizeLow`), атрибуты, даты создания и изменения. У перемещённого
  файла сохраняется дата создания, установленная в п.7. У копии в S3 дата создания —
  момент копирования, а дата изменения такая же, как у оригинала.

```
Содержимое S1 (...\TESTDIR\S1):
    Имя                Размер  Атр.  Создан               Изменён
    S2                  <DIR>  ----  ...
    S3                  <DIR>  ----  ...
Содержимое S2 (...\TESTDIR\S1\S2):
    S4                  <DIR>  ----  ...
    file2.txt              39  R---  01.01.2001 10:00:00  ...
Содержимое S3 (...\TESTDIR\S1\S3):
    S5                  <DIR>  ----  ...
    file2.txt              39  R---  <сейчас>             ...
```

---

### П.10. Установить метку тома 'LABEL'

**Код (`Step10_SetLabel`):**

```csharp
string root = SelectedRoot();               // например "C:\" или "E:\" — из списка «Диск»
if (SetVolumeLabel(root, label)) { ... "Метка тома установлена" ... }
else
{
    int err = Marshal.GetLastWin32Error();
    if (err == ERROR_ACCESS_DENIED && !IsAdministrator())
        ... предложить RestartAsAdmin() ...
    else
        ShowApiError("SetVolumeLabel", err, "Диск может быть защищён от записи ...");
}
```

**Как это работает:**

* `SetVolumeLabel(корень_тома, метка)`. Корень обязательно указывается с обратной косой
  чертой: `"C:\"`; в Delphi-примере это `'A:\'+#0`. Если вместо метки передать `nil`,
  метка удаляется.
* Список дисков заполняется в `FillDrives()` через `GetLogicalDriveStrings`. Функция
  возвращает строку вида `C:\<0>D:\<0><0>`, тип диска определяется через `GetDriveType`.
  По умолчанию выбран текущий диск.
* Изменение метки — привилегированная операция. На системном диске без прав
  администратора будет `ERROR_ACCESS_DENIED`. Тогда программа предлагает перезапуститься
  через `ShellExecute` с глаголом `runas`, это вызывает окно UAC. Повышенный процесс
  стартует в `System32`, поэтому текущий каталог передаётся аргументом `--dir`, и
  `Program.Main` восстанавливает его.

---

### П.11. Найти метку тома и сравнить с заданной в п.10

**Код (`CheckVolumeLabel`):**

```csharp
var volName = new StringBuilder(261);
var fsName  = new StringBuilder(261);
GetVolumeInformation(root, volName, (uint)volName.Capacity,
                     out uint serial, out uint maxComp, out uint flags,
                     fsName, (uint)fsName.Capacity);

string label = volName.ToString();
if (label.Length == 0)                         → «метка тома отсутствует»
else if (label == expected (без учёта регистра)) → «метка совпадает с заданной в п.10»
else                                             → «метка НЕ совпадает»
```

**Как это работает:**

* `GetVolumeInformation` записывает в буферы метку тома и имя файловой системы, а через
  `out`-параметры возвращает серийный номер, максимальную длину имени файла и флаги ФС.
  Это аналог примера с `vol := '12345678901'` в методичке, но вместо строки-заглушки
  передаётся `StringBuilder` нужного размера.
* В FAT/FAT32 метка физически хранится как особая запись **корневого каталога** с
  атрибутом «метка тома» ($08). Отсюда формулировка «найти в корневом каталоге запись
  с меткой тома». В NTFS метка хранится в служебном файле `$Volume`. `GetVolumeInformation`
  возвращает метку независимо от файловой системы.
* Серийный номер выводится как `XXXX-XXXX`, так же как его показывает команда `vol`.
* Сравнение идёт без учёта регистра: FAT хранит метку заглавными буквами.

---

### П.12. Изменить метку средствами ОС, проверить; стереть метку, проверить снова

**Код (`Step12Core`):**

```csharp
// «Да» — запускаем системную утилиту LABEL от имени администратора
Process p = Process.Start(new ProcessStartInfo("cmd.exe",
        $"/c vol {drive} & label {drive} & vol {drive} & pause")
        { UseShellExecute = true, Verb = "runas" });

await Task.Run(() => p.WaitForExit());       // ждём, не замораживая окно программы

string label = CheckVolumeLabel("П.12. Проверка метки");   // повторяем п.11
```

**Как выполнять:**

1. Нажать кнопку п.12 и выбрать **«Да»**. Откроется командная строка: `vol` покажет
   текущую метку, затем `label` попросит ввести новую. Введите, например, `NEWLABEL` и
   нажмите Enter, затем любую клавишу, чтобы закрыть окно.
   Изменить метку можно и в Проводнике: правая кнопка на диске → «Свойства» → поле
   метки. После этого в п.12 выберите **«Нет»** (сразу проверить).
2. Программа дождётся закрытия окна и проверит метку: **«Метка НЕ совпадает с заданной
   в п.10»**.
3. Снова п.12 → «Да». В утилите `LABEL` нажмите Enter на пустой строке и подтвердите
   удаление (`Y`). Программа сообщит: **«метка тома отсутствует»**.

`await` здесь нужен, чтобы окно программы не зависло, пока пользователь работает
с утилитой. Метод `Step12_OsLabel` объявлен `async void`, поэтому в нём есть свой
`try/catch`: исключения после `await` иначе не перехватились бы.

---

### Дополнительные команды части 4.2.1

* **«Выполнить п.1–9 подряд»** (`RunAllSteps`) последовательно вызывает пункты 1–9 в режиме
  `batchMode`: промежуточные информационные окна не показываются, ошибки показываются.
* **«Удалить созданную структуру»** (`DeleteTree`) рекурсивно обходит `TESTDIR`, снимает
  атрибуты (`SetFileAttributes(NORMAL)`), удаляет файлы (`DeleteFile`) и каталоги снизу
  вверх (`RemoveDirectory`: удалить можно только пустой каталог).
* **«Очистить протокол»**.

---

## 5. Часть 4.2.2 — функции для работы с памятью

Код находится в `Lab4/MainForm.Memory.cs`.

### Строки форматирования

Строки из раздела `const` методички перенесены почти дословно. В C# вместо `Format`
с `%x` используется `string.Format` с `{0:X8}`:

| Delphi | C# |
|---|---|
| `'Page Size $%.8x bytes'` | `"Page Size ${0:X8} bytes"` |
| `'Minimum Application Address $%p'` | `"Minimum Application Address ${0}"` (адрес форматируется функцией `FormatPtr`: 16 hex-цифр в 64-битном процессе, 8 — в 32-битном) |
| `'Number of Processor %d'` | `"Number of Processors {0}"` |
| `'Memory in use %d%%'` | `"Memory in use {0}%"` |
| `'Processor Revision %.4x'` | `"Processor Revision {0:X4}"` |

Рядом с каждой строкой выводится русский комментарий и размер в удобном виде:

```
Page Size $00001000 bytes                            // Размер страницы = 4 КБ
```

### 1) GetSystemInfo — основная системная информация

**Код (`btnSysInfo_Click`), аналог процедуры `TForm1.F1Click` из методички:**

```csharp
GetSystemInfo(out si);

Memo(string.Format(SProc, ArchitectureName(si.wProcessorArchitecture)), "Архитектура процессора");
Memo(string.Format(SPageSize, si.dwPageSize), ...);
Memo(string.Format(SMinAddr, FormatPtr(si.lpMinimumApplicationAddress)), ...);
Memo(string.Format(SMaxAddr, FormatPtr(si.lpMaximumApplicationAddress)), ...);
Memo(string.Format(SNumProcs, si.dwNumberOfProcessors), ...);
Memo(string.Format(SAllocGra, si.dwAllocationGranularity), ...);

switch (si.wProcessorLevel)               // тот же case, что в методичке
{
    case 3: Memo(string.Format(SProcLevel, SIntel3)); break;   // 80386
    case 4: ... SIntel4 ...                                    // 80486
    case 5: ... SIntel5 ...                                    // Pentium
    case 6: ... SIntel6 ...                                    // Pentium Pro
    default: Memo(string.Format(SProcLevel, si.wProcessorLevel.ToString())); break;
}
Memo(string.Format(SProcRev, si.wProcessorRevision), ...);
```

**Что означают поля (`SYSTEM_INFO`):**

| Поле | Значение | Типичный результат на x64 |
|---|---|---|
| `wProcessorArchitecture` | архитектура: 0 — Intel x86, 9 — AMD64, 12 — ARM64 и т.д. В методичке всегда выводится `SPIntel`; здесь архитектура определяется по значению поля | `AMD64 (x64, Intel 64)` |
| `dwPageSize` | размер страницы, единица защиты и фиксации памяти | `$00001000` = 4 КБ |
| `lpMinimumApplicationAddress` | младший адрес, доступный приложению | `$0000000000010000` (64 КБ) |
| `lpMaximumApplicationAddress` | старший адрес, доступный приложению | `$00007FFFFFFEFFFF` (≈128 ТБ) |
| `dwActiveProcessorMask` | битовая маска процессоров: бит 0 — первый процессор | `$FF` для 8 потоков |
| `dwNumberOfProcessors` | число логических процессоров | 8, 12, 16… |
| `dwProcessorType` | устаревшее поле, оставлено для совместимости | 8664 |
| `dwAllocationGranularity` | гранулярность резервирования: регионы начинаются с адресов, кратных этому числу | `$00010000` = 64 КБ |
| `wProcessorLevel` | семейство процессора. У всех современных Intel/AMD x86 — 6 и выше, поэтому выводится «Pentium Pro» (семейство P6) или номер | 6 |
| `wProcessorRevision` | `xxyy`: `xx` — модель, `yy` — степпинг | например `9E0A` |

### 2) GlobalMemoryStatus — текущее состояние памяти

**Код (`btnMemStatus_Click`), аналог `TForm1.F2Click`:**

```csharp
var ms = new MEMORYSTATUS();
ms.dwLength = (uint)Marshal.SizeOf(ms);     // ОБЯЗАТЕЛЬНО до вызова — как ms.dwLength := SizeOf(ms)
GlobalMemoryStatus(ref ms);

Memo(string.Format(SMemUse, ms.dwMemoryLoad), ...);
MemoSize(STotMem,  ms.dwTotalPhys,     "Общая физическая память");
MemoSize(SFreeMem, ms.dwAvailPhys,     "Свободная физическая память");
MemoSize(STotPage, ms.dwTotalPageFile, ...);
MemoSize(SFreePage,ms.dwAvailPageFile, ...);
MemoSize(STotVirt, ms.dwTotalVirtual,  ...);
MemoSize(SFreeVirt,ms.dwAvailVirtual,  ...);
```

**Как это работает:**

* Поле `dwLength` заполняется размером структуры перед вызовом. По нему Windows
  определяет, какую версию структуры передала программа.
* `dwMemoryLoad` — загрузка памяти от 0 до 100 %.
* `dwTotalPhys` / `dwAvailPhys` — объём установленного ОЗУ и свободная его часть.
* `dwTotalPageFile` / `dwAvailPageFile` — сколько всего может быть выделено (ОЗУ + файл
  подкачки) и сколько ещё доступно. Это не размер файла `pagefile.sys`.
* `dwTotalVirtual` / `dwAvailVirtual` — размер пользовательской части виртуального
  адресного пространства **процесса** и свободная её часть. В 64-битном процессе это
  около 128 ТБ.
* Поля в 32-битном Windows имеют тип `DWORD`, поэтому `GlobalMemoryStatus` не может
  сообщить больше 4 ГБ. В структуре они объявлены как `SIZE_T` (`UIntPtr`): в 64-битной
  программе это 8 байт. Дополнительно вызывается **`GlobalMemoryStatusEx`** с
  64-битными полями: так значения верны на любой машине.

### 3) VirtualQuery — детальная информация о виртуальном адресном пространстве

В задании сказано: «Параметр lpAddress задать, используя результаты п.п. 1) и 2)».
Поэтому адрес выбирается из выпадающего списка (`InitMemory`):

| Вариант | Как вычисляется |
|---|---|
| `lpMinimumApplicationAddress (п.1)` | `si.lpMinimumApplicationAddress` |
| `Min + dwAllocationGranularity (п.1)` | минимальный адрес + 64 КБ |
| `lpMaximumApplicationAddress (п.1)` | `si.lpMaximumApplicationAddress` |
| `Min + TotalVirtual / 2 (п.1 и п.2)` | середина адресного пространства по данным `GlobalMemoryStatus` |
| `Образ программы — GetModuleHandle(nil)` | адрес загрузки `Lab4.dll`/`.exe` |
| `kernel32.dll — GetModuleHandle` | адрес загрузки системной библиотеки |
| `Локальная переменная (стек)` | адрес переменной в стеке потока |
| `Блок GlobalAlloc (куча)` | адрес блока, выделенного `GlobalAlloc` |
| `Ввести вручную` | любой шестнадцатеричный адрес |

**Код запроса по одному адресу (`btnVirtualQuery_Click`):**

```csharp
UIntPtr res = VirtualQuery(lpAddress, out MEMORY_BASIC_INFORMATION mbi,
                           (UIntPtr)Marshal.SizeOf<MEMORY_BASIC_INFORMATION>());
if (res == UIntPtr.Zero) → ошибка (адрес вне диапазона приложения)

Memo($"BaseAddress       = ${FormatPtr(mbi.BaseAddress)}");
Memo($"AllocationBase    = ...");
Memo($"AllocationProtect = {ProtectName(mbi.AllocationProtect)}");
Memo($"RegionSize        = ${mbi.RegionSize:X8}");
Memo($"State             = {StateName(mbi.State)}");      // MEM_COMMIT / MEM_RESERVE / MEM_FREE
Memo($"Protect           = {ProtectName(mbi.Protect)}");  // PAGE_READWRITE, PAGE_EXECUTE_READ ...
Memo($"Type              = {TypeName(mbi.Type)}");        // MEM_IMAGE / MEM_MAPPED / MEM_PRIVATE
```

**Параметры функции:** `lpAddress` — интересующий адрес; `lpBuffer` — структура, которую
функция заполнит; `dwLength` — размер этой структуры. Возвращается число скопированных
байт: 48 в x64, 0 при ошибке.

**Как читать результат (`MEMORY_BASIC_INFORMATION`):**

* `BaseAddress` — `lpAddress`, округлённый вниз до границы страницы (кратно 4 КБ).
* `AllocationBase` — начало всего региона, зарезервированного одним вызовом
  `VirtualAlloc`. Оно кратно 64 КБ.
* `AllocationProtect` — защита, заданная при первоначальном резервировании.
* `RegionSize` — размер группы **подряд идущих страниц с одинаковыми** состоянием,
  защитой и типом, начиная с `BaseAddress`.
* `State`: `MEM_FREE` — адреса свободны; `MEM_RESERVE` — адреса заняты, но физическая
  память не выделена; `MEM_COMMIT` — память передана (ОЗУ или файл подкачки). Для
  `MEM_FREE` остальные поля не определены, программа выводит «не определено».
* `Protect` — текущая защита страниц: `PAGE_READONLY`, `PAGE_READWRITE`,
  `PAGE_EXECUTE_READ`, `PAGE_WRITECOPY`, модификатор `PAGE_GUARD` (так защищён край стека)
  и т.д. Расшифровку делает `ProtectName`.
* `Type`: `MEM_IMAGE` — отображение EXE/DLL, `MEM_MAPPED` — проецируемый файл,
  `MEM_PRIVATE` — закрытая память процесса (куча, стек).
* Для `MEM_IMAGE`/`MEM_MAPPED` программа дополнительно выводит имя файла через
  `GetMappedFileName`.

Примеры, которые стоит показать преподавателю:

* `lpMinimumApplicationAddress` — обычно свободная или зарезервированная область.
* «Образ программы» — `MEM_IMAGE`, `PAGE_READONLY` (заголовок PE-файла), файл `Lab4.dll`.
* «kernel32.dll» — `MEM_IMAGE`, файл `kernel32.dll`.
* «Стек» — `MEM_PRIVATE`, `MEM_COMMIT`, `PAGE_READWRITE`.
* `lpMaximumApplicationAddress + 1` (ввести вручную) — ошибка `ERROR_INVALID_PARAMETER`:
  адрес за пределами пространства приложения.

**Карта адресного пространства (`btnMemMap_Click`)** — расширенный вариант п.3. Функция
обходит всё пространство от минимального до максимального адреса из п.1:

```csharp
ulong addr = (ulong)si.lpMinimumApplicationAddress;
while (addr <= max)
{
    VirtualQuery((IntPtr)addr, out mbi, size);
    items.Add(RegionItem(mbi));                      // строка таблицы
    addr = (ulong)mbi.BaseAddress + mbi.RegionSize;  // следующий регион начинается сразу за текущим
}
```

Каждый вызов возвращает регион, а следующий регион начинается с `BaseAddress + RegionSize`.
Так перебирается всё адресное пространство процесса. Затем программа подсчитывает
суммарный объём `MEM_COMMIT`/`MEM_RESERVE`/`MEM_FREE` и сравнивает его с п.2: свободное
по карте ≈ `ullAvailVirtual`, занятое ≈ `ullTotalVirtual − ullAvailVirtual`. Так данные
`VirtualQuery` и `GlobalMemoryStatus` проверяют друг друга. Цвета строк в таблице:
серый — свободно, синий — зарезервировано, зелёный — образы EXE/DLL.

---

## 6. Часть 4.2.3 — буфер обмена

Код находится в `Lab4/MainForm.Clipboard.cs`.

### Определение текущих форматов (пример из п.4.1)

Процедура `TForm1.Timer1Timer` из методички перенесена в метод `RefreshClipboardFormats`.
Комментарии в коде совпадают с комментариями в методичке:

```csharp
// подсчитываем и выводим количество форматов
lblFormatCount.Text = "Всего форматов доступно: " + CountClipboardFormats();

// открываем буфер обмена
if (!OpenClipboard(Handle)) { ...буфер занят другой программой — повторим позже...; return; }
try
{
    // пытаемся считать первый из доступных форматов
    uint formatId = EnumClipboardFormats(0);
    // пытаемся считать остальные форматы
    while (formatId != 0)
    {
        // получаем имя формата; для предопределённых функция вернёт 0
        int len = GetClipboardFormatName(formatId, formatName, 255);
        if (len == 0)
            items.Add($"{PredefinedName(formatId)} (Предопределенный) [{formatId}]");
        else
            items.Add($"{formatName} [{formatId}]");      // зарегистрированный формат
        // получаем ID следующего формата
        formatId = EnumClipboardFormats(formatId);
    }
}
finally
{
    // обязательно закрываем буфер
    CloseClipboard();
}
```

**Как это работает:**

1. `CountClipboardFormats` возвращает количество форматов, в которых сейчас лежат данные.
   Для вызова этой функции буфер открывать не нужно.
2. `OpenClipboard(Handle)` открывает буфер; передаётся дескриптор окна нашей формы.
   Если буфер уже открыт другой программой, функция вернёт `False`. Тогда программа
   показывает, каким окном он занят (`GetOpenClipboardWindow`), и повторит попытку позже.
3. `EnumClipboardFormats(0)` возвращает первый формат, `EnumClipboardFormats(предыдущий)` —
   следующий. Значение 0 означает конец списка.
4. `GetClipboardFormatName` возвращает имя только для **зарегистрированных** форматов
   (номера `$C000–$FFFF`, например `HTML Format`, `Rich Text Format`, `FileNameW`). Для
   **предопределённых** форматов (`CF_TEXT` = 1 … `CF_DIBV5` = 17) функция возвращает 0,
   и имя берётся из таблицы `PredefinedClipboardNames`. Эта таблица — аналог массива
   `PredefinedClipboardNames` в методичке. Значение 17 в методичке подписано как
   `'CF_MAX'`, но в современных версиях Windows 17 — это `CF_DIBV5`. В таблице также есть
   форматы `CF_DSP*` ($80–$8E), а для частных и GDI-диапазонов формируется имя
   `CF_PRIVATEFIRST+n`.
5. `CloseClipboard` закрывает буфер. Это обязательно, иначе ни одна другая программа не
   сможет работать с буфером обмена.

**Таймер.** Как в методичке, список обновляется по таймеру раз в секунду (`timerClipboard`,
флажок «Таймер (1 с)» или пункт меню «Автообновление»). Чтобы не открывать буфер каждую
секунду и не мешать другим программам, сначала проверяется номер изменения буфера
`GetClipboardSequenceNumber`. Буфер открывается, только если номер изменился. Кнопка или
клавиша F5 обновляет список принудительно.

Дополнительно выводится **владелец буфера** (`GetClipboardOwner`): окно, класс окна
(`GetClassName`) и процесс (`GetWindowThreadProcessId`).

**Демонстрация (контрольный вопрос 7):**

| Что скопировать | Что появится в списке |
|---|---|
| Текст в Блокноте | `CF_UNICODETEXT`, `CF_LOCALE`, `CF_TEXT`, `CF_OEMTEXT` |
| Текст в Word | плюс `Rich Text Format`, `HTML Format`, `CF_ENHMETAFILE`, `CF_METAFILEPICT`, собственные форматы Word |
| Фрагмент в Paint | `CF_BITMAP`, `CF_DIB`, `CF_DIBV5` |
| Файл в Проводнике | `CF_HDROP`, `FileNameW`, `Shell IDList Array` и др. |

### Запись в буфер обмена (последовательность вызовов)

**Код (`PutTextToClipboard`):**

```csharp
OpenClipboard(Handle);                    // 1. открыть буфер
EmptyClipboard();                         // 2. очистить; владельцем становится наше окно

// 3. данные передаются в блоке глобальной памяти
IntPtr hText = GlobalAlloc(GMEM_MOVEABLE, размер);
IntPtr p = GlobalLock(hText);             //    получить указатель на блок
Marshal.Copy(байты_строки, 0, p, длина);  //    скопировать строку (UTF-16 + завершающий 0)
GlobalUnlock(hText);
SetClipboardData(CF_UNICODETEXT, hText);  //    отдать блок системе

// (по кнопке «+ собственный формат»)
uint fmt = RegisterClipboardFormat("Lab4.WinApi.CustomFormat");   // номер >= $C000
SetClipboardData(fmt, hCustom);

CloseClipboard();                         // 4. закрыть буфер
```

После успешного `SetClipboardData` блок памяти принадлежит системе, освобождать его
нельзя. Если вызов не удался, блок освобождается через `GlobalFree`.

### Чтение из буфера обмена и автоматическое преобразование форматов

**Код (`btnClipRead_Click`):**

```csharp
OpenClipboard(Handle);
foreach (формат in { CF_UNICODETEXT, CF_TEXT, CF_OEMTEXT, собственный })
{
    if (!IsClipboardFormatAvailable(формат)) continue;
    IntPtr h = GetClipboardData(формат);   // дескриптор данных (принадлежит системе)
    IntPtr p = GlobalLock(h);              // доступ к данным
    ... копируем и декодируем ...          // UTF-16 / кодовая страница ANSI (1251) / OEM (866)
    GlobalUnlock(h);
}
CloseClipboard();
```

Программа помещает в буфер **только** `CF_UNICODETEXT`, но после закрытия буфера в списке
появляются ещё `CF_TEXT`, `CF_OEMTEXT` и `CF_LOCALE`. Чтение в `CF_TEXT` и `CF_OEMTEXT`
возвращает тот же текст в однобайтовых кодировках. Это **автоматическое преобразование
форматов** из таблицы 2 методички: Windows синтезирует недостающие форматы при запросе.
`CF_LOCALE` хранит язык (LCID), по которому выбирается кодовая страница при
преобразовании.

Кнопка **EmptyClipboard** выполняет `OpenClipboard → EmptyClipboard → CloseClipboard`
и очищает буфер.

---

## 7. Дополнительные условия

### Заставка (`SplashForm.cs`)

Окно без рамки (`FormBorderStyle.None`) по центру экрана с названием работы и полосой
загрузки. Таймер закрывает заставку через 2,5 с; щелчок мыши или любая клавиша закрывает
её сразу. В `Program.Main` заставка показывается модально, до главного окна:

```csharp
using (var splash = new SplashForm())
    splash.ShowDialog();
Application.Run(new MainForm());
```

### Меню с командами

| Меню | Команды |
|---|---|
| Файл | Перезапустить от имени администратора, Выход (Alt+F4) |
| Файловые операции | п.1–12, Выполнить п.1–9 подряд, Показать структуру, Удалить структуру, Очистить протокол |
| Память | 1) GetSystemInfo, 2) GlobalMemoryStatus, 3) VirtualQuery, 3) Карта адресного пространства, Очистить |
| Буфер обмена | Определить форматы (F5), Автообновление по таймеру, Поместить текст, + собственный формат, Прочитать текст, Очистить |
| Справка | Содержание (F1), О программе |

### Справочная система (`HelpForm.cs`)

Открывается клавишей **F1** или через «Справка → Содержание». Сразу показывается раздел,
соответствующий открытой вкладке. Слева дерево разделов, справа текст:

* Лабораторная работа 4 (общее описание окна и меню)
* 4.2.1. Файловые операции
* Метка тома (п.10–12)
* 4.2.2. Работа с памятью
* 4.2.3. Буфер обмена
* Сообщения и особые ситуации
* Контрольные вопросы

### Информационные сообщения и особые ситуации

Результат каждого вызова API проверяется. При ошибке выводится окно с именем функции,
кодом `GetLastError` и системным текстом ошибки. Какие ситуации обрабатываются:

| Ситуация | Где | Что делает программа |
|---|---|---|
| Каталог уже существует (183) | п.1 | запись в протоколе, работа продолжается |
| Диск защищён от записи, нет прав | п.1 | окно «Невозможно создать каталоги!» |
| Не выполнен предыдущий пункт (нет S1 или file1.txt) | п.2–9 | подсказка, какой пункт выполнить |
| Отказано в доступе (5) при удалении | п.6 | предупреждение: файл защищён атрибутом «только чтение» |
| Нарушение блокировки (33) | п.5 | запись в протоколе: блокировка работает |
| Файл-приёмник уже существует | п.8, п.9 | вопрос: удалить и продолжить? |
| Отказано в доступе при смене метки | п.10 | предложение перезапуститься от имени администратора |
| Нет носителя в дисководе | п.11 | ошибка `GetVolumeInformation` |
| Метки нет или она не совпадает с заданной | п.11, п.12 | предупреждение |
| Недопустимый адрес | VirtualQuery | ошибка с подсказкой о допустимом диапазоне адресов |
| Буфер обмена занят другой программой | 4.2.3 | сообщение и повторная попытка при следующем обновлении |

---

## 8. Ответы на контрольные вопросы

**1. Основные функции Windows для файловых операций.**
Каталоги: `CreateDirectory`, `RemoveDirectory`, `GetCurrentDirectory`, `SetCurrentDirectory`.
Файлы: `CreateFile`, `OpenFile`, `_lopen`, `_lcreat`, `ReadFile`/`_lread`, `WriteFile`/`_lwrite`,
`SetFilePointer`/`_llseek`, `CloseHandle`/`_lclose`, `LockFile`/`UnlockFile`,
`GetFileAttributes`/`SetFileAttributes`, `GetFileTime`/`SetFileTime`, `CopyFile`, `MoveFile`,
`DeleteFile`. Поиск: `FindFirstFile`, `FindNextFile`, `FindClose`.
Диски: `GetLogicalDriveStrings`, `GetDriveType`, `GetVolumeInformation`, `SetVolumeLabel`.

**2. Формат функций чтения из файла и записи в файл.**
`_lread(hFile, lpBuffer, uBytes)` и `_lwrite(hFile, lpBuffer, uBytes)` принимают дескриптор
файла, указатель на буфер и количество байт. Возвращают число фактически
прочитанных/записанных байт или `HFILE_ERROR`.
`ReadFile(hFile, lpBuffer, nNumberOfBytesToRead, lpNumberOfBytesRead, lpOverlapped)` и
`WriteFile(...)` возвращают `BOOL`, число байт передают через указатель
`lpNumberOfBytesRead`. Параметр `lpOverlapped` нужен для асинхронного ввода-вывода
(при синхронном — `nil`). Чтение и запись идут с текущей позиции файлового указателя,
которую меняют `_llseek`/`SetFilePointer` (откуда отсчитывать: `FILE_BEGIN`,
`FILE_CURRENT`, `FILE_END`).

**3. Для чего используются функции блокирования и разблокирования файлов.**
`LockFile` даёт владельцу дескриптора монопольный доступ к участку файла (смещение +
длина). Другие процессы и дескрипторы при чтении или записи этого участка получают
`ERROR_LOCK_VIOLATION`; в п.5 это показано на практике. Так организуют согласованный
совместный доступ к одному файлу: базы данных, журналы, общие файлы в сети. Каждую
блокировку снимают `UnlockFile` с теми же смещением и длиной.

**4. Параметры функций GetSystemInfo и VirtualQuery.**
У `GetSystemInfo(var lpSystemInfo: TSystemInfo)` один параметр — структура, которую
функция заполняет. Поля разобраны в разделе 5, подраздел «1) GetSystemInfo».
`VirtualQuery(lpAddress, lpBuffer, dwLength)`: `lpAddress` — адрес, о котором нужна
информация; `lpBuffer` — указатель на `MEMORY_BASIC_INFORMATION`; `dwLength` — размер
этой структуры. Возвращает число записанных в буфер байт, 0 — ошибка. Поля разобраны
в разделе 5, подраздел «3) VirtualQuery».

**5. Последовательность вызовов при обмене данными через буфер обмена.**
Запись: `OpenClipboard` → `EmptyClipboard` → `GlobalAlloc` + `GlobalLock` + копирование +
`GlobalUnlock` → `SetClipboardData(формат, hMem)`; желательно в нескольких форматах →
`CloseClipboard`.
Чтение: `IsClipboardFormatAvailable` (или `EnumClipboardFormats`) → `OpenClipboard` →
`GetClipboardData(формат)` → `GlobalLock` + копирование + `GlobalUnlock` → `CloseClipboard`.

**6. Для чего и как используется преобразование форматов.**
Если программа поместила данные в одном формате, Windows может выдать их в родственном:
`CF_TEXT` ↔ `CF_OEMTEXT` ↔ `CF_UNICODETEXT`, `CF_BITMAP` ↔ `CF_DIB` ↔ `CF_DIBV5`,
`CF_METAFILEPICT` ↔ `CF_ENHMETAFILE`. Получатель просто запрашивает нужный ему формат
через `GetClipboardData`, а система выполняет преобразование. Благодаря этому данные
доступны большему числу программ. В программе это видно так: записывается только
`CF_UNICODETEXT`, а читаются ещё `CF_TEXT` и `CF_OEMTEXT`. Свои форматы приложение может
зарегистрировать функцией `RegisterClipboardFormat`.

**7. Демонстрация чтения поддерживаемых форматов.**
Откройте вкладку «4.2.3. Буфер обмена» с включённым таймером и скопируйте данные в разных
программах (таблица в разделе 6). Список форматов и их число обновляются автоматически.
