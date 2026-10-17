using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static Lab4.NativeMethods;

namespace Lab4
{
    /// <summary>
    /// 4.2.2. Системные функции для работы с памятью:
    /// GetSystemInfo, GlobalMemoryStatus (GlobalMemoryStatusEx), VirtualQuery.
    /// </summary>
    public partial class MainForm
    {
        // { Строки информации о системе }
        private const string SProc = "Processor Architecture {0}";             // Архитектура процессора
        private const string SPIntel = "Intel";
        private const string SPageSize = "Page Size ${0:X8} bytes";            // Размер страницы
        private const string SMinAddr = "Minimum Application Address ${0}";    // Минимальный адрес приложения
        private const string SMaxAddr = "Maximum Application Address ${0}";    // Максимальный адрес приложения
        private const string SActMask = "Active Processor Mask ${0:X}";        // Маска активных процессоров
        private const string SNumProcs = "Number of Processors {0}";           // Число процессоров
        private const string SProcType = "Processor Type {0}";                 // Тип процессора (устаревшее поле)
        private const string SAllocGra = "Allocation Granularity ${0:X8} bytes"; // Степень разбиения при выделении ресурсов
        private const string SProcLevel = "Processor Level {0}";               // Уровень процессора
        private const string SIntel3 = "80386";
        private const string SIntel4 = "80486";
        private const string SIntel5 = "Pentium";
        private const string SIntel6 = "Pentium Pro";
        private const string SProcRev = "Processor Revision {0:X4}";           // Модификация процессора

        // { Строки состояния памяти }
        private const string SMemUse = "Memory in use {0}%";                         // Используемая память
        private const string STotMem = "Total physical memory ${0:X8} bytes";        // Общая физическая память
        private const string SFreeMem = "Free physical memory ${0:X8} bytes";        // Свободная физическая память
        private const string STotPage = "Total page file memory ${0:X8} bytes";      // Общая память файлов подкачки
        private const string SFreePage = "Free page file memory ${0:X8} bytes";      // Свободная память файлов подкачки
        private const string STotVirt = "Total virtual memory ${0:X8} bytes";        // Общая виртуальная память
        private const string SFreeVirt = "Free virtual memory ${0:X8} bytes";        // Свободная виртуальная память

        private SYSTEM_INFO si;
        private MEMORYSTATUSEX msx;
        private IntPtr heapBlock;   // блок памяти для демонстрации VirtualQuery на куче

        private sealed class AddressPreset
        {
            public string Text;
            public Func<IntPtr> Get;
            public override string ToString() => Text;
        }

        private void InitMemory()
        {
            GetSystemInfo(out si);
            msx = new MEMORYSTATUSEX { dwLength = (uint)Marshal.SizeOf<MEMORYSTATUSEX>() };
            GlobalMemoryStatusEx(ref msx);
            heapBlock = GlobalAlloc(0 /* GMEM_FIXED */, new UIntPtr(64 * 1024));

            var presets = new List<AddressPreset>
            {
                new AddressPreset { Text = "lpMinimumApplicationAddress (п.1)", Get = () => si.lpMinimumApplicationAddress },
                new AddressPreset
                {
                    Text = "Min + dwAllocationGranularity (п.1)",
                    Get = () => new IntPtr(si.lpMinimumApplicationAddress.ToInt64() + si.dwAllocationGranularity)
                },
                new AddressPreset { Text = "lpMaximumApplicationAddress (п.1)", Get = () => si.lpMaximumApplicationAddress },
                new AddressPreset
                {
                    Text = "Min + TotalVirtual / 2 (п.1 и п.2)",
                    Get = () => new IntPtr(si.lpMinimumApplicationAddress.ToInt64() + (long)(msx.ullTotalVirtual / 2))
                },
                new AddressPreset { Text = "Образ программы — GetModuleHandle(nil)", Get = () => GetModuleHandle(null) },
                new AddressPreset { Text = "kernel32.dll — GetModuleHandle", Get = () => GetModuleHandle("kernel32.dll") },
                new AddressPreset { Text = "Локальная переменная (стек)", Get = StackAddress },
                new AddressPreset { Text = "Блок GlobalAlloc (куча)", Get = () => heapBlock },
                new AddressPreset { Text = "Ввести вручную", Get = null }
            };
            foreach (AddressPreset p in presets)
                cmbAddrPreset.Items.Add(p);
            cmbAddrPreset.SelectedIndex = 0;
        }

        private static unsafe IntPtr StackAddress()
        {
            int local = 0;
            return new IntPtr(&local);
        }

        private void cmbAddrPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAddrPreset.SelectedItem is AddressPreset p && p.Get != null)
                txtAddress.Text = FormatPtr(p.Get());
            txtAddress.ReadOnly = !(cmbAddrPreset.SelectedItem is AddressPreset q) || q.Get != null;
            if (!txtAddress.ReadOnly)
                txtAddress.Focus();
        }

        private void Memo(string line = "")
        {
            txtMemo.AppendText(line + "\r\n");
        }

        private void Memo(string english, string russian)
        {
            Memo($"{english,-52} // {russian}");
        }

        private void MemoHeader(string title)
        {
            if (txtMemo.TextLength > 0)
                Memo();
            Memo(new string('-', 100));
            Memo($"[{DateTime.Now:HH:mm:ss}] {title}");
            Memo(new string('-', 100));
            SetStatus(title);
        }

        private static string ArchitectureName(ushort arch)
        {
            switch (arch)
            {
                case PROCESSOR_ARCHITECTURE_INTEL: return SPIntel + " (x86)";
                case PROCESSOR_ARCHITECTURE_MIPS: return "MIPS";
                case PROCESSOR_ARCHITECTURE_ALPHA: return "Alpha";
                case PROCESSOR_ARCHITECTURE_PPC: return "PowerPC";
                case PROCESSOR_ARCHITECTURE_ARM: return "ARM";
                case PROCESSOR_ARCHITECTURE_IA64: return "Intel Itanium (IA-64)";
                case PROCESSOR_ARCHITECTURE_AMD64: return "AMD64 (x64, Intel 64)";
                case PROCESSOR_ARCHITECTURE_ARM64: return "ARM64";
                default: return $"неизвестна ({arch})";
            }
        }

        // ------------------------------------------------------------------
        //  1) GetSystemInfo
        // ------------------------------------------------------------------
        private void btnSysInfo_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabMemory;
            GetSystemInfo(out si);

            MemoHeader("1) GetSystemInfo — основная системная информация");
            Memo(string.Format(SProc, ArchitectureName(si.wProcessorArchitecture)), "Архитектура процессора");
            Memo(string.Format(SPageSize, si.dwPageSize), $"Размер страницы = {HumanSize(si.dwPageSize)}");
            Memo(string.Format(SMinAddr, FormatPtr(si.lpMinimumApplicationAddress)), "Минимальный адрес приложения");
            Memo(string.Format(SMaxAddr, FormatPtr(si.lpMaximumApplicationAddress)), "Максимальный адрес приложения");
            Memo(string.Format(SActMask, si.dwActiveProcessorMask.ToUInt64()), "Маска активных процессоров");
            Memo(string.Format(SNumProcs, si.dwNumberOfProcessors), "Число процессоров (логических)");
            Memo(string.Format(SProcType, si.dwProcessorType), "Тип процессора (для совместимости)");
            Memo(string.Format(SAllocGra, si.dwAllocationGranularity),
                $"Гранулярность выделения = {HumanSize(si.dwAllocationGranularity)}");

            switch (si.wProcessorLevel)
            {
                case 3: Memo(string.Format(SProcLevel, SIntel3), "Уровень процессора"); break;
                case 4: Memo(string.Format(SProcLevel, SIntel4), "Уровень процессора"); break;
                case 5: Memo(string.Format(SProcLevel, SIntel5), "Уровень процессора"); break;
                case 6: Memo(string.Format(SProcLevel, SIntel6), "Уровень процессора (семейство 6 и новее)"); break;
                default: Memo(string.Format(SProcLevel, si.wProcessorLevel.ToString()), "Уровень процессора (семейство)"); break;
            }
            Memo(string.Format(SProcRev, si.wProcessorRevision),
                $"Модификация: модель {si.wProcessorRevision >> 8}, степпинг {si.wProcessorRevision & 0xFF}");
            Memo();
            Memo($"Разрядность процесса: {IntPtr.Size * 8} бит, ОС: {(Environment.Is64BitOperatingSystem ? 64 : 32)} бит");
            Memo($"Объём адресного пространства приложения: " +
                 HumanSize((ulong)(si.lpMaximumApplicationAddress.ToInt64() - si.lpMinimumApplicationAddress.ToInt64() + 1)));
            cmbAddrPreset_SelectedIndexChanged(this, EventArgs.Empty);
        }

        // ------------------------------------------------------------------
        //  2) GlobalMemoryStatus
        // ------------------------------------------------------------------
        private void btnMemStatus_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabMemory;

            var ms = new MEMORYSTATUS();
            ms.dwLength = (uint)Marshal.SizeOf(ms);
            GlobalMemoryStatus(ref ms);

            MemoHeader("2) GlobalMemoryStatus — текущее состояние памяти");
            Memo(string.Format(SMemUse, ms.dwMemoryLoad), "Используемая память, %");
            MemoSize(STotMem, ms.dwTotalPhys.ToUInt64(), "Общая физическая память");
            MemoSize(SFreeMem, ms.dwAvailPhys.ToUInt64(), "Свободная физическая память");
            MemoSize(STotPage, ms.dwTotalPageFile.ToUInt64(), "Общая память файлов подкачки");
            MemoSize(SFreePage, ms.dwAvailPageFile.ToUInt64(), "Свободная память файлов подкачки");
            MemoSize(STotVirt, ms.dwTotalVirtual.ToUInt64(), "Общая виртуальная память процесса");
            MemoSize(SFreeVirt, ms.dwAvailVirtual.ToUInt64(), "Свободная виртуальная память процесса");

            msx = new MEMORYSTATUSEX { dwLength = (uint)Marshal.SizeOf<MEMORYSTATUSEX>() };
            if (GlobalMemoryStatusEx(ref msx))
            {
                Memo();
                Memo("GlobalMemoryStatusEx (64-битные поля; GlobalMemoryStatus в 32-битном процессе");
                Memo("при объёме памяти > 4 ГБ возвращает усечённые значения):");
                Memo(string.Format(SMemUse, msx.dwMemoryLoad), "Используемая память, %");
                MemoSize(STotMem, msx.ullTotalPhys, "Общая физическая память");
                MemoSize(SFreeMem, msx.ullAvailPhys, "Свободная физическая память");
                MemoSize(STotPage, msx.ullTotalPageFile, "Предел выделения (ОЗУ + подкачка)");
                MemoSize(SFreePage, msx.ullAvailPageFile, "Доступно для выделения");
                MemoSize(STotVirt, msx.ullTotalVirtual, "Общая виртуальная память процесса");
                MemoSize(SFreeVirt, msx.ullAvailVirtual, "Свободная виртуальная память процесса");
            }
            cmbAddrPreset_SelectedIndexChanged(this, EventArgs.Empty);
        }

        private void MemoSize(string format, ulong value, string russian)
        {
            Memo(string.Format(format, value), $"{russian} = {HumanSize(value)}");
        }

        // ------------------------------------------------------------------
        //  3) VirtualQuery
        // ------------------------------------------------------------------
        private static string StateName(uint state)
        {
            switch (state)
            {
                case MEM_COMMIT: return "MEM_COMMIT";
                case MEM_RESERVE: return "MEM_RESERVE";
                case MEM_FREE: return "MEM_FREE";
                default: return $"${state:X}";
            }
        }

        private static string StateDescription(uint state)
        {
            switch (state)
            {
                case MEM_COMMIT: return "передана (выделена физическая память / файл подкачки)";
                case MEM_RESERVE: return "зарезервирована (адреса заняты, памяти нет)";
                case MEM_FREE: return "свободна";
                default: return "?";
            }
        }

        private static string TypeName(uint type)
        {
            switch (type)
            {
                case 0: return "—";
                case MEM_IMAGE: return "MEM_IMAGE";
                case MEM_MAPPED: return "MEM_MAPPED";
                case MEM_PRIVATE: return "MEM_PRIVATE";
                default: return $"${type:X}";
            }
        }

        private static string TypeDescription(uint type)
        {
            switch (type)
            {
                case MEM_IMAGE: return "отображение исполняемого файла (EXE/DLL)";
                case MEM_MAPPED: return "проецируемый файл / раздел";
                case MEM_PRIVATE: return "закрытая память процесса";
                default: return "не определён";
            }
        }

        private static string ProtectName(uint protect)
        {
            if (protect == 0)
                return "—";
            string s;
            switch (protect & 0xFF)
            {
                case PAGE_NOACCESS: s = "PAGE_NOACCESS"; break;
                case PAGE_READONLY: s = "PAGE_READONLY"; break;
                case PAGE_READWRITE: s = "PAGE_READWRITE"; break;
                case PAGE_WRITECOPY: s = "PAGE_WRITECOPY"; break;
                case PAGE_EXECUTE: s = "PAGE_EXECUTE"; break;
                case PAGE_EXECUTE_READ: s = "PAGE_EXECUTE_READ"; break;
                case PAGE_EXECUTE_READWRITE: s = "PAGE_EXECUTE_READWRITE"; break;
                case PAGE_EXECUTE_WRITECOPY: s = "PAGE_EXECUTE_WRITECOPY"; break;
                default: s = $"${protect & 0xFF:X2}"; break;
            }
            if ((protect & PAGE_GUARD) != 0) s += "|GUARD";
            if ((protect & PAGE_NOCACHE) != 0) s += "|NOCACHE";
            if ((protect & PAGE_WRITECOMBINE) != 0) s += "|WRITECOMBINE";
            return s;
        }

        private static string MappedFileName(MEMORY_BASIC_INFORMATION mbi)
        {
            if (mbi.State == MEM_FREE || (mbi.Type != MEM_IMAGE && mbi.Type != MEM_MAPPED))
                return "";
            var sb = new StringBuilder(1024);
            uint len = GetMappedFileName(GetCurrentProcess(), mbi.BaseAddress, sb, (uint)sb.Capacity);
            return len == 0 ? "" : Path.GetFileName(sb.ToString());
        }

        private static bool TryParseAddress(string text, out ulong value)
        {
            text = (text ?? "").Trim().Replace("$", "").Replace("`", "").Replace(" ", "");
            if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                text = text.Substring(2);
            return ulong.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
        }

        private ListViewItem RegionItem(MEMORY_BASIC_INFORMATION mbi)
        {
            bool free = mbi.State == MEM_FREE;
            var item = new ListViewItem(FormatPtr(mbi.BaseAddress));
            item.SubItems.Add(free ? "—" : FormatPtr(mbi.AllocationBase));
            item.SubItems.Add(free ? "—" : ProtectName(mbi.AllocationProtect));
            item.SubItems.Add(mbi.RegionSize.ToUInt64().ToString("N0"));
            item.SubItems.Add(StateName(mbi.State));
            item.SubItems.Add(free ? "—" : ProtectName(mbi.Protect));
            item.SubItems.Add(free ? "—" : TypeName(mbi.Type));
            item.SubItems.Add(MappedFileName(mbi));
            if (free)
                item.ForeColor = Color.Gray;
            else if (mbi.State == MEM_RESERVE)
                item.ForeColor = Color.SteelBlue;
            else if (mbi.Type == MEM_IMAGE)
                item.ForeColor = Color.DarkGreen;
            return item;
        }

        private void btnVirtualQuery_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabMemory;
            if (!TryParseAddress(txtAddress.Text, out ulong address))
            {
                ShowWarning("Введите адрес в шестнадцатеричном виде, например 7FF6A0000000.", "VirtualQuery");
                txtAddress.Focus();
                return;
            }

            var lpAddress = new IntPtr(unchecked((long)address));
            UIntPtr res = VirtualQuery(lpAddress, out MEMORY_BASIC_INFORMATION mbi,
                new UIntPtr((uint)Marshal.SizeOf<MEMORY_BASIC_INFORMATION>()));

            MemoHeader($"3) VirtualQuery(lpAddress = ${FormatPtr(lpAddress)})  [{cmbAddrPreset.SelectedItem}]");
            if (res == UIntPtr.Zero)
            {
                int err = Marshal.GetLastWin32Error();
                Memo("VirtualQuery вернула 0 — " + ErrorText(err));
                ShowApiError("VirtualQuery", err,
                    $"Адрес должен лежать в диапазоне ${FormatPtr(si.lpMinimumApplicationAddress)} – " +
                    $"${FormatPtr(si.lpMaximumApplicationAddress)} (см. п.1).");
                return;
            }

            bool free = mbi.State == MEM_FREE;
            Memo($"Скопировано в буфер: {res.ToUInt64()} байт (SizeOf(MEMORY_BASIC_INFORMATION))");
            Memo($"BaseAddress       = ${FormatPtr(mbi.BaseAddress)}", "адрес, округлённый до границы страницы");
            Memo($"AllocationBase    = {(free ? "не определено" : "$" + FormatPtr(mbi.AllocationBase))}", "базовый адрес региона");
            Memo($"AllocationProtect = {(free ? "не определено" : ProtectName(mbi.AllocationProtect))}", "защита при резервировании");
            Memo($"RegionSize        = ${mbi.RegionSize.ToUInt64():X8}", $"{mbi.RegionSize.ToUInt64():N0} байт = {HumanSize(mbi.RegionSize.ToUInt64())}");
            Memo($"State             = {StateName(mbi.State)}", StateDescription(mbi.State));
            Memo($"Protect           = {(free ? "не определено" : ProtectName(mbi.Protect))}", "текущая защита страниц");
            Memo($"Type              = {(free ? "не определено" : TypeName(mbi.Type))}", free ? "—" : TypeDescription(mbi.Type));
            string file = MappedFileName(mbi);
            if (file.Length > 0)
                Memo($"Файл              = {file}", "GetMappedFileName");
            Memo($"Страниц в группе  = {mbi.RegionSize.ToUInt64() / si.dwPageSize}", $"по {si.dwPageSize} байт (п.1)");

            lvRegions.BeginUpdate();
            lvRegions.Items.Clear();
            lvRegions.Items.Add(RegionItem(mbi));
            lvRegions.EndUpdate();
        }

        /// <summary>
        /// Обход всего адресного пространства процесса от lpMinimumApplicationAddress
        /// до lpMaximumApplicationAddress (результаты п.1) последовательными вызовами VirtualQuery.
        /// </summary>
        private void btnMemMap_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabMemory;
            GetSystemInfo(out si);
            msx = new MEMORYSTATUSEX { dwLength = (uint)Marshal.SizeOf<MEMORYSTATUSEX>() };
            GlobalMemoryStatusEx(ref msx);

            Cursor = Cursors.WaitCursor;
            var items = new List<ListViewItem>();
            ulong commit = 0, reserve = 0, freeSize = 0, image = 0, mapped = 0, priv = 0;
            int nCommit = 0, nReserve = 0, nFree = 0;
            var mbiSize = new UIntPtr((uint)Marshal.SizeOf<MEMORY_BASIC_INFORMATION>());
            try
            {
                ulong addr = (ulong)si.lpMinimumApplicationAddress.ToInt64();
                ulong max = (ulong)si.lpMaximumApplicationAddress.ToInt64();
                while (addr <= max && items.Count < 200000)
                {
                    if (VirtualQuery(new IntPtr(unchecked((long)addr)), out MEMORY_BASIC_INFORMATION mbi, mbiSize) == UIntPtr.Zero)
                        break;
                    items.Add(RegionItem(mbi));

                    ulong size = mbi.RegionSize.ToUInt64();
                    switch (mbi.State)
                    {
                        case MEM_COMMIT: commit += size; nCommit++; break;
                        case MEM_RESERVE: reserve += size; nReserve++; break;
                        case MEM_FREE: freeSize += size; nFree++; break;
                    }
                    if (mbi.State != MEM_FREE)
                    {
                        if (mbi.Type == MEM_IMAGE) image += size;
                        else if (mbi.Type == MEM_MAPPED) mapped += size;
                        else if (mbi.Type == MEM_PRIVATE) priv += size;
                    }

                    ulong next = (ulong)mbi.BaseAddress.ToInt64() + size;
                    if (next <= addr)
                        break;
                    addr = next;
                }

                lvRegions.BeginUpdate();
                lvRegions.Items.Clear();
                lvRegions.Items.AddRange(items.ToArray());
                lvRegions.EndUpdate();
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            MemoHeader("3) Карта виртуального адресного пространства процесса (VirtualQuery)");
            Memo($"Диапазон обхода (п.1): ${FormatPtr(si.lpMinimumApplicationAddress)} – ${FormatPtr(si.lpMaximumApplicationAddress)}");
            Memo($"Всего регионов: {items.Count}");
            Memo($"  MEM_COMMIT  : {nCommit,6} регионов, {HumanSize(commit),12}");
            Memo($"  MEM_RESERVE : {nReserve,6} регионов, {HumanSize(reserve),12}");
            Memo($"  MEM_FREE    : {nFree,6} регионов, {HumanSize(freeSize),12}");
            Memo($"  из занятых: MEM_IMAGE {HumanSize(image)}, MEM_MAPPED {HumanSize(mapped)}, MEM_PRIVATE {HumanSize(priv)}");
            Memo();
            Memo("Сопоставление с GlobalMemoryStatus (п.2):");
            Memo($"  Свободно по карте (MEM_FREE)          = {HumanSize(freeSize)}");
            Memo($"  ullAvailVirtual (п.2)                 = {HumanSize(msx.ullAvailVirtual)}");
            Memo($"  Занято по карте (COMMIT + RESERVE)    = {HumanSize(commit + reserve)}");
            Memo($"  ullTotalVirtual − ullAvailVirtual     = {HumanSize(msx.ullTotalVirtual - msx.ullAvailVirtual)}");
            Memo("Цвета в таблице: серый — свободно, синий — зарезервировано, зелёный — образы EXE/DLL.");
        }

        private void btnMemClear_Click(object sender, EventArgs e)
        {
            txtMemo.Clear();
            lvRegions.Items.Clear();
        }
    }
}
