# Лабораторная работа № 4 — разбор

**Тема:** создание приложений Windows с использованием средств прикладного программирования.

**Реализация:** C#, Windows Forms, .NET 8. Всё сделано **стандартными средствами языка и
платформы .NET**: классы `File`, `Directory`, `FileStream`, `DriveInfo`, `Environment`,
`Process`, `Clipboard`. Эти классы внутри сами вызывают функции Win32 API из методички.
В разборе для каждого пункта указано, какая функция API стоит за каждым методом .NET.

Вызов Win32 API напрямую остался **только один**: `VirtualQuery` из п.3 части 4.2.2. В .NET
у неё нет аналога.

---

## Содержание

1. [Как запустить](#1-как-запустить)
2. [Устройство программы](#2-устройство-программы)
3. [Часть 4.2.1 — файловые операции](#3-часть-421--файловые-операции)
4. [Часть 4.2.2 — работа с памятью](#4-часть-422--работа-с-памятью)
5. [Часть 4.2.3 — буфер обмена](#5-часть-423--буфер-обмена)
6. [Дополнительные условия](#6-дополнительные-условия)
7. [Ответы на контрольные вопросы](#7-ответы-на-контрольные-вопросы)

---

## 1. Как запустить

Нужны Windows и Visual Studio 2022 (рабочая нагрузка «Разработка классических приложений
.NET») или .NET 8 SDK.

* Visual Studio: открыть `Lab4.sln` и нажать **F5**.
* Командная строка: `dotnet run --project Lab4`

Чтобы менять метку системного диска `C:` (п.10–12), программу нужно запустить **от имени
администратора**. Проще выбрать флешку в списке «Диск для п.10–12».

---

## 2. Устройство программы

| Файл | Что внутри |
|---|---|
| `Program.cs` | Точка входа: подключает кодировку 1251, показывает заставку, затем главное окно |
| `SplashForm.cs` | Заставка |
| `HelpForm.cs` | Справка (F1) |
| `MainForm.Designer.cs` | Разметка окна: меню, три вкладки, поля вывода |
| `MainForm.cs` | Весь код заданий, разделён на три блока: 4.2.1, 4.2.2, 4.2.3 |

В окне три вкладки, по одной на часть задания. На каждой вкладке есть кнопки команд и поле
вывода результатов.

### Как устроены команды

Кнопки и пункты меню не рисуются вручную. Они создаются в конструкторе формы из списка
пар «текст — метод»:

```csharp
AddCommands(tabFiles, flpFiles, mnuFiles, new (string, Action)[]
{
    ("1. Создать каталоги", Step1),
    ("2. Создать file1.txt", Step2),
    ...
});
```

Метод `AddCommands` для каждой пары добавляет кнопку на панель вкладки и пункт в меню.
Оба вызывают одну и ту же функцию `Execute`:

```csharp
void Execute()
{
    tabControl.SelectedTab = tab;
    try
    {
        run();                         // выполняем пункт задания
    }
    catch (Exception ex)               // любая ошибка → окно с её описанием
    {
        MessageBox.Show(this, ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
```

Отсюда простой способ обработки ошибок во всей программе. Методы .NET при ошибке
**выбрасывают исключение**, а не возвращают `False`, как функции API. Исключение
перехватывается в одном месте, и пользователь видит понятное сообщение: «Нет доступа к
пути…», «Файл не найден…» и т.п. Если пункт выполняется раньше нужного, программа сама
выбрасывает исключение с подсказкой, например «Сначала выполните п.1».

---

## 3. Часть 4.2.1 — файловые операции

### Исходные данные

```csharp
private static readonly string[] Dirs = { @"S1\S2\S4", @"S1\S3\S5" };

private const string File1 = "file1.txt";
private const string File2 = "file2.txt";
private const string VolumeLabel = "LABEL";
private static readonly DateTime NewCreationTime = new DateTime(2001, 1, 1, 10, 0, 0);

private static Encoding Ansi => Encoding.GetEncoding(1251);
```

* **Каталожная структура** — массив `Dirs`. Получается такое дерево:
  ```
  TESTDIR\
      S1\
          S2\
              S4\
          S3\
              S5\
  ```
  Если в лабораторной № 3 структура была другой, достаточно поменять этот массив.
* **Кодировка Windows-1251.** В ней каждый русский символ занимает ровно 1 байт, как
  в Delphi. Поэтому в п.4 «8 байт» — это ровно 8 букв. В .NET 8 эту кодировку нужно один раз
  подключить; это делается в `Program.cs`:
  `Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)`.
* При запуске запоминается путь `rootDir = <текущий каталог>\TESTDIR`, так выполняется
  условие «на текущем диске в текущем каталоге».

Три маленьких помощника:

```csharp
private void GoToS1()          // перейти в S1, а если его нет — ошибка «Сначала выполните п.1»
private void RequireFile1()    // перейти в S1 и проверить, что file1.txt есть
private static void DeleteIfExists(string path)   // удалить старый файл перед повторным запуском
```

`DeleteIfExists` нужен, чтобы пункты можно было запускать повторно. Файл, оставшийся с
прошлого раза, может иметь атрибут «только чтение», поэтому сначала сбрасываются атрибуты,
а потом файл удаляется.

---

### П.1. Создать каталожную структуру и вывести её на экран

```csharp
foreach (string dir in Dirs)
{
    string path = Path.Combine(rootDir, dir);
    Log((Directory.Exists(path) ? "Уже существует: " : "Создан: ") + path);
    Directory.CreateDirectory(path);   // создаёт и все промежуточные каталоги
}

Directory.SetCurrentDirectory(s1Dir);
Log("Текущий каталог: " + Directory.GetCurrentDirectory());
PrintTree(rootDir, "    ");
```

**Что происходит:**

* `Directory.CreateDirectory(@"...\TESTDIR\S1\S2\S4")` создаёт **сразу всю цепочку**:
  TESTDIR, S1, S2, S4. API-функция `CreateDirectory` так не умеет: .NET вызывает её для
  каждого уровня по очереди. Если каталог уже есть, ошибки не будет, поэтому пункт можно
  повторять.
* `Directory.SetCurrentDirectory` — это `SetCurrentDirectory` из API. После этого S1 становится
  текущим каталогом, и дальше можно писать короткие имена: `file1.txt`, `S2\file2.txt`.
* `PrintTree` — рекурсивный вывод дерева: сначала подкаталоги (`Directory.GetDirectories`),
  потом файлы (`Directory.GetFiles`) с размером и атрибутами. Внутри работают
  `FindFirstFile`/`FindNextFile`.

```
C:\...\TESTDIR\
    S1\
        S2\
            S4\
        S3\
            S5\
```

---

### П.2. Создать в S1 файл file1.txt и записать «Текстовая строка»

```csharp
GoToS1();
DeleteIfExists(File1);
File.WriteAllText(File1, "Текстовая строка", Ansi);
```

`File.WriteAllText` за один вызов делает то же, что `OpenFile(OF_CREATE)` + `_lwrite` +
`_lclose` в примере на Delphi: создаёт файл (или обрезает существующий), записывает строку в
кодировке 1251 и закрывает файл. В файле получается 16 байт.

---

### П.3. Прочитать содержимое файла и вывести на экран

```csharp
string text = File.ReadAllText(File1, Ansi);
Log($"Содержимое: \"{text}\"");
```

`File.ReadAllText` открывает файл, читает его целиком, переводит байты в строку и закрывает
файл. Это аналог `_lopen` + `_lread` + `_lclose`.

---

### П.4. Прочитать 8 байт, начиная с байта 5

```csharp
using (FileStream fs = File.OpenRead(File1))
{
    fs.Seek(4, SeekOrigin.Begin);        // 5-й байт — это смещение 4
    var buffer = new byte[8];
    int count = fs.Read(buffer, 0, buffer.Length);
    Log($"Прочитано {count} байт: \"{Ansi.GetString(buffer, 0, count)}\"");
}
```

**Что происходит:**

* `FileStream` — поток для побайтовой работы с файлом. `File.OpenRead` открывает файл
  на чтение (`_lopen(OF_READ)`).
* `fs.Seek(4, SeekOrigin.Begin)` — это `_llseek(h, 4, FILE_BEGIN)`. Байты считаются с нуля,
  поэтому байт № 5 имеет смещение **4**.
* `fs.Read(buffer, 0, 8)` — это `_lread`: читает 8 байт и возвращает, сколько прочитано.
* `using` гарантирует закрытие файла (`_lclose`), даже если случится ошибка.
* Результат: **«товая ст»** (Текс*товая ст*рока).

---

### П.5. Добавить в конец файла «Дополнительная строка»

```csharp
using (var fs = new FileStream(File1, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
{
    long end = fs.Seek(0, SeekOrigin.End);                 // в конец файла
    byte[] data = Ansi.GetBytes("\r\nДополнительная строка");

    fs.Lock(end, data.Length);                              // блокируем участок
    fs.Write(data, 0, data.Length);
    fs.Flush();

    // проверка: другой поток не может прочитать заблокированный участок
    try { ... new FileStream(...).Read(...) ... }
    catch (IOException ex) { Log("Проверка блокировки — ...: " + ex.Message); }

    fs.Unlock(end, data.Length);                            // снимаем блокировку
}
Log(File.ReadAllText(File1, Ansi));
```

**Что происходит:**

* Файл открывается на чтение и запись (`OF_READWRITE`). `FileShare.ReadWrite` разрешает
  другим открывать этот файл одновременно; это нужно для проверки блокировки.
* `fs.Seek(0, SeekOrigin.End)` — это `SetFilePointer(h, 0, nil, FILE_END)`. Метод возвращает
  новую позицию, то есть длину файла (16).
* `fs.Lock(позиция, длина)` и `fs.Unlock(...)` — это `LockFile` и `UnlockFile` из методички.
* `fs.Write` записывает байты (`_lwrite`). `fs.Flush()` сразу сбрасывает их из буфера
  `FileStream` в файл.
* **Проверка блокировки:** пока участок заблокирован, программа открывает файл второй раз
  и пытается прочитать этот участок. Windows отказывает, возникает `IOException`
  с сообщением «Процесс не может получить доступ к файлу, так как часть этого файла
  заблокирована другим процессом». Сообщение выводится в окно.
* Перед строкой записывается перевод строки `\r\n`, поэтому в итоге файл выглядит так:
  ```
  Текстовая строка
  Дополнительная строка
  ```

---

### П.6. Установить атрибуты «только чтение» и «скрытый», попытаться удалить файл

```csharp
File.SetAttributes(File1, FileAttributes.ReadOnly | FileAttributes.Hidden);
Log("Атрибуты: " + File.GetAttributes(File1));      // ReadOnly, Hidden
try
{
    File.Delete(File1);
}
catch (UnauthorizedAccessException ex)
{
    Warning("Файл не удаляется, потому что у него атрибут «только чтение».\n\n" + ex.Message);
}
```

**Что происходит:**

* `FileAttributes` — перечисление-флаги. Атрибуты объединяются оператором `|` (в Delphi —
  `or`). `File.SetAttributes` — это `SetFileAttributes`.
* `File.Delete` для файла «только для чтения» не срабатывает. .NET выбрасывает
  `UnauthorizedAccessException` («Отказано в доступе»). Программа его перехватывает и
  показывает предупреждение. Это и есть «особая ситуация» из задания.

---

### П.7. Снять атрибут «скрытый» и изменить дату и время создания

```csharp
File.SetAttributes(File1, File.GetAttributes(File1) & ~FileAttributes.Hidden);

Log("Дата создания была:  " + File.GetCreationTime(File1));
File.SetCreationTime(File1, NewCreationTime);        // 01.01.2001 10:00:00
Log("Дата создания стала: " + File.GetCreationTime(File1));
```

**Что происходит:**

* `атрибуты & ~Hidden` убирает **только** бит «скрытый», «только чтение» остаётся.
  В Delphi это `attrs and not FILE_ATTRIBUTE_HIDDEN`.
* `File.SetCreationTime` — это `SetFileTime`. .NET сам переводит местное время в формат
  Windows (`FILETIME`, UTC), поэтому не нужны `SystemTimeToFileTime` и
  `LocalFileTimeToFileTime`. Атрибут «только чтение» не мешает менять дату.

---

### П.8. Переименовать file1.txt в file2.txt

```csharp
DeleteIfExists(File2);
File.Move(File1, File2);
```

Переименование — это перемещение в тот же каталог под новым именем: `File.Move` — это
`MoveFile`. `DeleteIfExists` убирает `file2.txt`, если он остался с прошлого запуска, иначе
`File.Move` выдал бы ошибку «файл уже существует».

---

### П.9. Переслать file2.txt в S2, скопировать в S3, показать S1, S2, S3

```csharp
string inS2 = Path.Combine("S2", File2);      // S2\file2.txt (относительно S1)
string inS3 = Path.Combine("S3", File2);
File.Move(File2, inS2);                        // пересылка — файл исчезает из S1
File.Copy(inS2, inS3);                         // копия в S3

foreach (string name in new[] { "S1", @"S1\S2", @"S1\S3" })
    foreach (FileSystemInfo item in new DirectoryInfo(...).GetFileSystemInfos())
        Log($"    {item.Name,-12} {size,-10} {item.Attributes,-25} создан {item.CreationTime}");
```

**Что происходит:**

* `File.Move` в другой каталог — это `MoveFile`: файл пропадает из S1 и появляется в S2.
* `File.Copy` — это `CopyFile`. Копия получает те же атрибуты, в том числе «только чтение».
* `DirectoryInfo.GetFileSystemInfos()` возвращает и подкаталоги, и файлы. Для каждого
  объекта выводятся имя, размер (у каталога `<DIR>`), атрибуты и дата создания. В S2 видна
  дата 01.01.2001 из п.7, у копии в S3 дата создания — момент копирования.

```
Содержимое S1\S2:
    S4           <DIR>      Directory                 создан ...
    file2.txt    39 байт    ReadOnly, Archive         создан 01.01.2001 10:00:00
```

---

### П.10. Установить метку тома 'LABEL'

```csharp
DriveInfo drive = SelectedDrive();          // диск из списка слева
try
{
    drive.VolumeLabel = VolumeLabel;        // = SetVolumeLabel
}
catch (UnauthorizedAccessException)
{
    Warning("Нет прав на изменение метки... Запустите программу от имени администратора или выберите флешку.");
}
```

* `DriveInfo` — класс для работы с диском. Список дисков заполняется при запуске через
  `DriveInfo.GetDrives()` (только готовые к работе, `IsReady`). По умолчанию выбран
  текущий диск.
* Присвоение свойства `VolumeLabel` вызывает `SetVolumeLabel`. Менять метку системного
  диска может только администратор, иначе будет `UnauthorizedAccessException`, и программа
  объяснит, что делать.

---

### П.11. Найти метку тома и сравнить с заданной

```csharp
string label = drive.VolumeLabel;            // = GetVolumeInformation
if (label == "")                → «У диска нет метки»
else if (label == "LABEL")      → «совпадает с заданной в п.10»
else                            → «НЕ совпадает»
```

Чтение `VolumeLabel` вызывает `GetVolumeInformation`. Дополнительно выводятся файловая
система (`DriveFormat`) и тип диска (`DriveType`). Сравнение идёт без учёта регистра:
FAT хранит метку заглавными буквами.

В файловой системе FAT метка действительно хранится в корневом каталоге как особая
запись с атрибутом «метка тома». Отсюда формулировка задания «найти в корневом каталоге
запись с меткой тома».

---

### П.12. Изменить метку средствами ОС и проверить; стереть и проверить снова

```csharp
MessageBox.Show("Измените метку диска средствами Windows: Проводник → правая кнопка на диске → Свойства ...");
CheckLabel();   // та же проверка, что в п.11
```

**Как выполнять:**

1. Нажать п.12. Не закрывая окно с подсказкой, открыть Проводник → правая кнопка на диске →
   «Свойства» → вписать другую метку → ОК.
2. Нажать ОК в окне программы. Ответ: **метка НЕ совпадает с LABEL**.
3. Снова п.12, в свойствах диска **очистить** поле метки → ОК → ОК в программе.
   Ответ: **у диска нет метки**.

---

### Кнопки «Выполнить п.1–9» и «Удалить TESTDIR»

* `RunSteps1To9` вызывает `Step1()…Step9()` подряд. Флаг `runningAll` отключает
  предупреждение в п.6, чтобы окна не мешали.
* `DeleteTestDir` сбрасывает атрибуты всех файлов (файл «только для чтения» не удалится)
  и удаляет весь каталог вызовом `Directory.Delete(rootDir, true)`. `true` означает «вместе
  с содержимым».

---

## 4. Часть 4.2.2 — работа с памятью

### 1) Основная системная информация (вместо GetSystemInfo)

```csharp
Mem($"Операционная система:   {RuntimeInformation.OSDescription}");
Mem($"Архитектура процессора: {RuntimeInformation.ProcessArchitecture}");
Mem($"Процессор:              {Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER")}");
Mem($"Уровень процессора:     {Environment.GetEnvironmentVariable("PROCESSOR_LEVEL")}");
Mem($"Модификация процессора: {Environment.GetEnvironmentVariable("PROCESSOR_REVISION")}");
Mem($"Число процессоров:      {Environment.ProcessorCount}");
Mem($"Размер страницы:        {Environment.SystemPageSize} байт");
Mem($"Разрядность процесса:   {(Environment.Is64BitProcess ? 64 : 32)} бит");
```

Какие поля `SYSTEM_INFO` это заменяет:

| Поле SYSTEM_INFO | Средство .NET | Пример значения |
|---|---|---|
| `wProcessorArchitecture` | `RuntimeInformation.ProcessArchitecture` | `X64` |
| `dwNumberOfProcessors` | `Environment.ProcessorCount` | `8` |
| `dwPageSize` | `Environment.SystemPageSize` | `4096` (4 КБ) |
| `wProcessorLevel` | переменная окружения `PROCESSOR_LEVEL` | `6` (семейство P6 — Pentium Pro и все современные) |
| `wProcessorRevision` | переменная окружения `PROCESSOR_REVISION` | `9e0a` (модель 9E, степпинг 0A) |

Windows сама записывает данные процессора в переменные окружения `PROCESSOR_*`. Их можно
увидеть командой `set PROCESSOR` в консоли.

### 2) Текущее состояние памяти (вместо GlobalMemoryStatus)

```csharp
var info = new ComputerInfo();               // Microsoft.VisualBasic.Devices
ulong total = info.TotalPhysicalMemory;
ulong free  = info.AvailablePhysicalMemory;

Mem($"Используется памяти:          {100 - free * 100 / total}%");
Mem($"Всего физической памяти:      {Mb(total)}");
Mem($"Свободно физической памяти:   {Mb(free)}");
Mem($"Всего виртуальной памяти:     {Mb(info.TotalVirtualMemory)}");
Mem($"Свободно виртуальной памяти:  {Mb(info.AvailableVirtualMemory)}");

using (Process p = Process.GetCurrentProcess())
{
    ... p.VirtualMemorySize64, p.WorkingSet64, p.PagedMemorySize64 ...
}
```

* `ComputerInfo` — готовый класс .NET, который внутри вызывает `GlobalMemoryStatusEx`
  и выдаёт те же поля:

| Поле MEMORYSTATUS | ComputerInfo |
|---|---|
| `dwMemoryLoad` | вычисляется: `100 − свободно × 100 / всего` |
| `dwTotalPhys` / `dwAvailPhys` | `TotalPhysicalMemory` / `AvailablePhysicalMemory` |
| `dwTotalVirtual` / `dwAvailVirtual` | `TotalVirtualMemory` / `AvailableVirtualMemory` |

* Виртуальная память процесса в 64-битной Windows — около **128 ТБ**: столько адресов
  доступно программе. Реально занята лишь малая часть.
* Через `Process.GetCurrentProcess()` дополнительно видно, сколько памяти использует сама
  программа: виртуальная память, рабочий набор (страницы в ОЗУ) и память в файле подкачки.

### 3) Виртуальное адресное пространство (VirtualQuery)

Сначала средствами .NET выводится список **модулей** процесса (`Process.Modules`). Это
EXE и все DLL, загруженные в адресное пространство: адрес начала, размер и имя.

```csharp
foreach (ProcessModule m in p.Modules.Cast<ProcessModule>().OrderBy(m => m.BaseAddress.ToInt64()))
    Mem($"    {Hex(m.BaseAddress)}  {m.ModuleMemorySize / 1024,7} КБ  {m.ModuleName}");
```

Подробные сведения о конкретном участке памяти (состояние, защита, тип) в .NET получить
нельзя, поэтому `VirtualQuery` объявлена напрямую. Это единственное место в программе, где
так сделано:

```csharp
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
```

* `[DllImport("kernel32.dll")]` говорит, что тело функции находится в системной DLL.
* `[StructLayout(LayoutKind.Sequential)]` — поля структуры лежат в памяти в том же порядке,
  что и в C.
* Параметры: адрес; структура, которую функция заполнит (`out`); размер структуры.

Запрос выполняется для двух адресов: **начала программы** (`p.MainModule.BaseAddress`) и
**блока памяти**, выделенного `Marshal.AllocHGlobal`. Числовые коды переводятся в названия
выражением `switch`:

```csharp
string state = mbi.State switch
{
    0x1000  => "MEM_COMMIT (память выделена)",
    0x2000  => "MEM_RESERVE (зарезервирована)",
    0x10000 => "MEM_FREE (свободна)",
    _       => mbi.State.ToString()
};
```

Что означают поля результата:

| Поле | Смысл | Для начала программы | Для AllocHGlobal |
|---|---|---|---|
| `BaseAddress` | адрес, округлённый до начала страницы | = адрес модуля | адрес страницы блока |
| `AllocationBase` | начало всего выделенного региона | = адрес модуля | начало кучи |
| `RegionSize` | размер подряд идущих страниц с одинаковыми свойствами | 4096 (заголовок EXE) | зависит от кучи |
| `State` | `MEM_COMMIT` / `MEM_RESERVE` / `MEM_FREE` | `MEM_COMMIT` | `MEM_COMMIT` |
| `Protect` | защита страниц (`PAGE_READONLY`, `PAGE_READWRITE`, …) | `PAGE_READONLY` | `PAGE_READWRITE` |
| `Type` | `MEM_IMAGE` — EXE/DLL, `MEM_MAPPED` — файл, `MEM_PRIVATE` — своя память | `MEM_IMAGE` | `MEM_PRIVATE` |

---

## 5. Часть 4.2.3 — буфер обмена

Пример из методички: таймер раз в секунду опрашивает буфер обмена и выводит список
форматов. На C# он выглядит так:

```csharp
private void timerClipboard_Tick(object sender, EventArgs e) => RefreshFormats(false);

private void RefreshFormats(bool force)
{
    IDataObject data;
    try
    {
        data = Clipboard.GetDataObject();          // OpenClipboard + чтение + CloseClipboard
    }
    catch (ExternalException)
    {
        return;                                    // буфер занят другой программой
    }

    string[] stored = data.GetFormats(false);      // форматы, в которых данные реально лежат
    string[] all    = data.GetFormats(true);       // плюс те, в которые их можно преобразовать

    lblCount.Text = $"Всего форматов доступно: {stored.Length}   (с учётом автопреобразования: {all.Length})";
    foreach (string format in all)
        lstFormats.Items.Add(stored.Contains(format) ? format : format + "   (автопреобразование)");
}
```

**Сравнение с примером на Delphi:**

| Delphi (Win32 API) | C# |
|---|---|
| `Timer1Timer` | `timerClipboard_Tick` (таймер 1000 мс, включается флажком «Таймер») |
| `OpenClipboard` … `CloseClipboard` | `Clipboard.GetDataObject()` — открывает и закрывает буфер сам |
| `CountClipboardFormats` | `GetFormats(false).Length` |
| цикл `EnumClipboardFormats` + `GetClipboardFormatName` | `GetFormats(...)` сразу возвращает массив **имён** форматов |
| массив `PredefinedClipboardNames` | не нужен: .NET сам называет форматы (`Text`, `UnicodeText`, `Bitmap`, `HTML Format`, `FileDrop`…) |

* `GetFormats(false)` возвращает форматы, в которых программа-источник **действительно
  положила** данные.
* `GetFormats(true)` добавляет форматы, в которые данные можно **автоматически
  преобразовать**. Они помечены в списке «(автопреобразование)». Это наглядно показывает
  преобразование форматов (контрольный вопрос 6).
* Если буфер в этот момент открыт другой программой, возникает `ExternalException`. Тогда
  обновление пропускается до следующего такта таймера.
* Список перерисовывается только при изменении набора форматов, чтобы не мигал.

**Кнопки:**

* «Обновить список форматов» обновляет список сразу.
* «Поместить текст в буфер» вызывает `Clipboard.SetText(...)`. Внутри это
  `OpenClipboard` → `EmptyClipboard` → `SetClipboardData` → `CloseClipboard`.
* «Очистить буфер» вызывает `Clipboard.Clear()` (`EmptyClipboard`).

**Демонстрация (контрольный вопрос 7):** включите таймер и копируйте данные в разных
программах.

| Что скопировать | Что появится в списке (примерно) |
|---|---|
| Текст в Блокноте | `UnicodeText`, `Locale`, `Text`, `OEMText` |
| Текст в Word | плюс `Rich Text Format`, `HTML Format`, `EnhancedMetafile` и форматы Word |
| Рисунок в Paint | `Bitmap`, `DeviceIndependentBitmap`, `Format17` (DIBV5) |
| Файл в Проводнике | `FileDrop`, `FileNameW`, `Shell IDList Array` и др. |

---

## 6. Дополнительные условия

* **Заставка** (`SplashForm.cs`) — окно без рамки по центру экрана. Таймер закрывает его
  через 2 секунды, щелчок мышью — сразу. В `Program.Main` заставка показывается до
  главного окна:
  ```csharp
  using (var splash = new SplashForm())
      splash.ShowDialog();
  Application.Run(new MainForm());
  ```
* **Меню с командами** — «Файл» (Выход), «Файловые операции» (п.1–12), «Память»,
  «Буфер обмена», «Справка». Пункты меню создаются из тех же списков, что и кнопки.
* **Справочная система** (`HelpForm.cs`) — открывается клавишей **F1** или через
  «Справка → Содержание». Описывает все пункты и порядок работы.
* **Информационные сообщения и особые ситуации:**

| Ситуация | Что видит пользователь |
|---|---|
| Пункт выполнен раньше нужного (нет S1, нет файла) | «Сначала выполните п.…» |
| Файл «только для чтения» не удаляется (п.6) | предупреждение «Отказано в доступе» |
| Участок файла заблокирован (п.5) | сообщение о блокировке в окне вывода |
| Нет прав на смену метки (п.10) | совет запустить от администратора или выбрать флешку |
| Метки нет или она не совпадает (п.11, п.12) | предупреждение |
| Диск защищён от записи, нет доступа, любая другая ошибка | окно «Ошибка» с текстом исключения |
| Буфер обмена занят другой программой | обновление пропускается до следующей секунды |

---

## 7. Ответы на контрольные вопросы

**1. Основные функции Windows для файловых операций.**
Win32 API: `CreateDirectory`, `RemoveDirectory`, `SetCurrentDirectory`, `CreateFile`/`OpenFile`,
`ReadFile`/`_lread`, `WriteFile`/`_lwrite`, `SetFilePointer`/`_llseek`, `CloseHandle`/`_lclose`,
`LockFile`/`UnlockFile`, `SetFileAttributes`, `SetFileTime`, `MoveFile`, `CopyFile`,
`DeleteFile`, `FindFirstFile`/`FindNextFile`, `GetVolumeInformation`, `SetVolumeLabel`.
В .NET им соответствуют классы `Directory`, `File`, `FileStream`, `DirectoryInfo`, `DriveInfo`.

**2. Формат функций чтения из файла и записи в файл.**
`_lread(hFile, lpBuffer, uBytes)` / `_lwrite(hFile, lpBuffer, uBytes)`: дескриптор файла,
буфер, число байт; возвращают, сколько байт прочитано/записано.
`ReadFile(hFile, lpBuffer, nNumberOfBytesToRead, lpNumberOfBytesRead, lpOverlapped)` —
то же, но число байт возвращается через параметр, а `lpOverlapped` нужен для асинхронной
работы (`nil` — обычная).
В .NET: `fs.Read(buffer, offset, count)` / `fs.Write(buffer, offset, count)`. Чтение и
запись идут с текущей позиции, которую задаёт `Seek` (`_llseek`/`SetFilePointer`).

**3. Для чего нужны блокирование и разблокирование файлов.**
`LockFile` (`FileStream.Lock`) закрывает участок файла для остальных: другой процесс при
обращении к нему получает ошибку (п.5). Так несколько программ согласованно работают
с одним файлом: базы данных, журналы, общие файлы в сети. После работы участок
освобождают `UnlockFile` (`FileStream.Unlock`) с теми же параметрами.

**4. Параметры GetSystemInfo и VirtualQuery.**
`GetSystemInfo(var SystemInfo)` — единственный параметр-структура, которую функция
заполняет: архитектура, размер страницы, минимальный и максимальный адреса приложения,
число процессоров, гранулярность выделения, уровень и модификация процессора.
`VirtualQuery(lpAddress, lpBuffer, dwLength)` — адрес; структура
`MEMORY_BASIC_INFORMATION` для результата; её размер. Возвращает число записанных байт.

**5. Последовательность вызовов при работе с буфером обмена.**
Запись: `OpenClipboard` → `EmptyClipboard` → `SetClipboardData` (лучше в нескольких
форматах) → `CloseClipboard`.
Чтение: `OpenClipboard` → `GetClipboardData` → `CloseClipboard`.
В .NET всю цепочку выполняют `Clipboard.SetText` / `Clipboard.GetDataObject`.

**6. Для чего и как используется преобразование форматов.**
Программа кладёт данные в одном формате, а Windows может выдать их в родственном:
`Text` ↔ `OEMText` ↔ `UnicodeText`, `Bitmap` ↔ `DIB`. Так данные понимает больше программ.
В нашей программе это видно по пометке «(автопреобразование)»: `GetFormats(true)`
показывает форматы, в которые данные будут преобразованы по запросу.

**7. Демонстрация.** Вкладка «Буфер обмена», флажок «Таймер» включён. Копируйте данные
в разных программах (таблица в разделе 5), список и число форматов меняются сами.
