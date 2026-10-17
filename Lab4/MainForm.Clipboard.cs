using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static Lab4.NativeMethods;

namespace Lab4
{
    /// <summary>
    /// 4.2.3. Системные функции для работы с буфером обмена.
    /// Определение форматов (по примеру из п.4.1), запись и чтение данных.
    /// </summary>
    public partial class MainForm
    {
        /// <summary>Предопределённые имена форматов буфера обмена.</summary>
        private static readonly Dictionary<uint, string> PredefinedClipboardNames = new Dictionary<uint, string>
        {
            { 1, "CF_TEXT" }, { 2, "CF_BITMAP" }, { 3, "CF_METAFILEPICT" }, { 4, "CF_SYLK" },
            { 5, "CF_DIF" }, { 6, "CF_TIFF" }, { 7, "CF_OEMTEXT" }, { 8, "CF_DIB" },
            { 9, "CF_PALETTE" }, { 10, "CF_PENDATA" }, { 11, "CF_RIFF" }, { 12, "CF_WAVE" },
            { 13, "CF_UNICODETEXT" }, { 14, "CF_ENHMETAFILE" }, { 15, "CF_HDROP" }, { 16, "CF_LOCALE" },
            { 17, "CF_DIBV5" },
            { 0x0080, "CF_OWNERDISPLAY" }, { 0x0081, "CF_DSPTEXT" }, { 0x0082, "CF_DSPBITMAP" },
            { 0x0083, "CF_DSPMETAFILEPICT" }, { 0x008E, "CF_DSPENHMETAFILE" }
        };

        private const string CustomFormatName = "Lab4.WinApi.CustomFormat";

        private uint lastClipboardSequence;

        private void InitClipboard()
        {
            chkClipTimer.Checked = true;   // как в примере — обновление по таймеру
        }

        private static string PredefinedName(uint id)
        {
            if (PredefinedClipboardNames.TryGetValue(id, out string name))
                return name;
            if (id >= 0x0200 && id <= 0x02FF)
                return $"CF_PRIVATEFIRST+{id - 0x0200}";
            if (id >= 0x0300 && id <= 0x03FF)
                return $"CF_GDIOBJFIRST+{id - 0x0300}";
            return "Формат " + id;
        }

        private static string FormatName(uint id)
        {
            var sb = new StringBuilder(256);
            int len = GetClipboardFormatName(id, sb, sb.Capacity);
            return len == 0 ? PredefinedName(id) : sb.ToString();
        }

        private void ClipLog(string text = "")
        {
            txtClipOut.AppendText(text + "\r\n");
        }

        private void ClipHeader(string title)
        {
            if (txtClipOut.TextLength > 0)
                ClipLog();
            ClipLog($"[{DateTime.Now:HH:mm:ss}] {title}");
            ClipLog(new string('-', 70));
            SetStatus(title);
        }

        // ------------------------------------------------------------------
        //  Определение текущих форматов буфера обмена (пример из п.4.1)
        // ------------------------------------------------------------------
        private void RefreshClipboardFormats(bool force)
        {
            uint seq = GetClipboardSequenceNumber();
            if (!force && seq == lastClipboardSequence)
                return;

            // подсчитываем и выводим количество форматов, поддерживаемых текущим владельцем буфера обмена
            lblFormatCount.Text = "Всего форматов доступно: " + CountClipboardFormats();

            // открываем буфер обмена
            if (!OpenClipboard(Handle))
            {
                // буфер обмена сейчас открыт другим окном — попробуем на следующем такте таймера
                IntPtr wnd = GetOpenClipboardWindow();
                lblOwner.Text = $"Буфер обмена занят (открыт окном ${FormatPtr(wnd)}), повтор при следующем обновлении...";
                return;
            }

            var items = new List<string>();
            try
            {
                // пытаемся считать первый из доступных форматов
                uint formatId = EnumClipboardFormats(0);
                // пытаемся считать остальные форматы
                while (formatId != 0)
                {
                    // получаем имя формата; для предопределённых форматов функция вернёт 0
                    var formatName = new StringBuilder(256);
                    int len = GetClipboardFormatName(formatId, formatName, formatName.Capacity);
                    if (len == 0)
                        items.Add($"{PredefinedName(formatId)} (Предопределенный) [{formatId}]");
                    else
                        items.Add($"{formatName} [{formatId}]");   // зарегистрированный формат

                    // получаем ID следующего формата буфера обмена
                    formatId = EnumClipboardFormats(formatId);
                }
            }
            finally
            {
                // обязательно закрываем буфер
                CloseClipboard();
            }

            lstFormats.BeginUpdate();
            lstFormats.Items.Clear();
            foreach (string s in items)
                lstFormats.Items.Add(s);
            if (items.Count == 0)
                lstFormats.Items.Add("(буфер обмена пуст)");
            lstFormats.EndUpdate();

            lblOwner.Text = $"Владелец буфера обмена: {DescribeClipboardOwner()}    " +
                            $"Номер последовательности: {seq}    Обновлено: {DateTime.Now:HH:mm:ss}";
            lastClipboardSequence = seq;
        }

        private string DescribeClipboardOwner()
        {
            IntPtr owner = GetClipboardOwner();
            if (owner == IntPtr.Zero)
                return "нет (NULL)";
            if (owner == Handle)
                return $"это окно (${FormatPtr(owner)})";

            string result = "окно $" + FormatPtr(owner);
            var cls = new StringBuilder(256);
            if (GetClassName(owner, cls, cls.Capacity) > 0)
                result += $", класс «{cls}»";
            GetWindowThreadProcessId(owner, out uint pid);
            try
            {
                using (Process p = Process.GetProcessById((int)pid))
                    result += $", процесс {p.ProcessName}.exe (PID {pid})";
            }
            catch (ArgumentException)
            {
                result += $", PID {pid}";
            }
            return result;
        }

        private void timerClipboard_Tick(object sender, EventArgs e)
        {
            RefreshClipboardFormats(false);
        }

        private void btnClipRefresh_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabClipboard;
            RefreshClipboardFormats(true);
            SetStatus($"Форматы буфера обмена обновлены: {CountClipboardFormats()}");
        }

        private void chkClipTimer_CheckedChanged(object sender, EventArgs e)
        {
            timerClipboard.Enabled = chkClipTimer.Checked;
            if (mnuClipTimer.Checked != chkClipTimer.Checked)
                mnuClipTimer.Checked = chkClipTimer.Checked;
        }

        private void mnuClipTimer_CheckedChanged(object sender, EventArgs e)
        {
            if (chkClipTimer.Checked != mnuClipTimer.Checked)
                chkClipTimer.Checked = mnuClipTimer.Checked;
        }

        // ------------------------------------------------------------------
        //  Запись данных в буфер обмена
        // ------------------------------------------------------------------

        /// <summary>Копирует байты в новый блок глобальной памяти (GlobalAlloc + GlobalLock).</summary>
        private static IntPtr GlobalFromBytes(byte[] data)
        {
            IntPtr hMem = GlobalAlloc(GMEM_MOVEABLE, new UIntPtr((uint)data.Length));
            if (hMem == IntPtr.Zero)
                return IntPtr.Zero;
            IntPtr p = GlobalLock(hMem);
            Marshal.Copy(data, 0, p, data.Length);
            GlobalUnlock(hMem);
            return hMem;
        }

        private void PutTextToClipboard(bool withCustomFormat)
        {
            tabControl.SelectedTab = tabClipboard;
            string text = txtClipText.Text;
            ClipHeader(withCustomFormat ? "Запись текста и собственного формата в буфер обмена" : "Запись текста в буфер обмена");

            if (!OpenClipboard(Handle))
            {
                ShowApiError("OpenClipboard", Marshal.GetLastWin32Error(), "Буфер обмена занят другим приложением.");
                return;
            }
            try
            {
                ClipLog("1. OpenClipboard(Handle) — буфер открыт");
                if (!EmptyClipboard())
                {
                    ShowApiError("EmptyClipboard", Marshal.GetLastWin32Error());
                    return;
                }
                ClipLog("2. EmptyClipboard — содержимое удалено, владелец буфера — это окно");

                // CF_UNICODETEXT: строка UTF-16 с завершающим нулём
                IntPtr hText = GlobalFromBytes(Encoding.Unicode.GetBytes(text + "\0"));
                if (SetClipboardData(CF_UNICODETEXT, hText) == IntPtr.Zero)
                {
                    int err = Marshal.GetLastWin32Error();
                    GlobalFree(hText);
                    ShowApiError("SetClipboardData", err);
                    return;
                }
                ClipLog($"3. GlobalAlloc/GlobalLock/GlobalUnlock + SetClipboardData(CF_UNICODETEXT): \"{text}\"");

                if (withCustomFormat)
                {
                    uint fmt = RegisterClipboardFormat(CustomFormatName);
                    if (fmt == 0)
                    {
                        ShowApiError("RegisterClipboardFormat", Marshal.GetLastWin32Error());
                        return;
                    }
                    string payload = $"{text} | {Environment.UserName} | {DateTime.Now:dd.MM.yyyy HH:mm:ss}";
                    IntPtr hCustom = GlobalFromBytes(Encoding.UTF8.GetBytes(payload + "\0"));
                    if (SetClipboardData(fmt, hCustom) == IntPtr.Zero)
                    {
                        int err = Marshal.GetLastWin32Error();
                        GlobalFree(hCustom);
                        ShowApiError("SetClipboardData", err);
                        return;
                    }
                    ClipLog($"   RegisterClipboardFormat(\"{CustomFormatName}\") → {fmt} (${fmt:X4})");
                    ClipLog($"   SetClipboardData({fmt}): \"{payload}\"");
                }
            }
            finally
            {
                CloseClipboard();
                ClipLog("4. CloseClipboard — буфер закрыт");
            }

            RefreshClipboardFormats(true);
            ClipLog();
            ClipLog("В буфер помещён только CF_UNICODETEXT" + (withCustomFormat ? " (и собственный формат)" : "") + ",");
            ClipLog("но в списке есть также CF_TEXT, CF_OEMTEXT и CF_LOCALE — Windows");
            ClipLog("синтезирует их автоматически при запросе (см. табл. 2 методички).");
        }

        private void btnClipWrite_Click(object sender, EventArgs e)
        {
            PutTextToClipboard(false);
        }

        private void btnClipWriteCustom_Click(object sender, EventArgs e)
        {
            PutTextToClipboard(true);
        }

        // ------------------------------------------------------------------
        //  Чтение данных из буфера обмена
        // ------------------------------------------------------------------
        private static byte[] ReadGlobalBytes(IntPtr hMem)
        {
            IntPtr p = GlobalLock(hMem);
            if (p == IntPtr.Zero)
                return null;
            try
            {
                int size = (int)GlobalSize(hMem).ToUInt64();
                var data = new byte[size];
                Marshal.Copy(p, data, 0, size);
                return data;
            }
            finally
            {
                GlobalUnlock(hMem);
            }
        }

        private static string DecodeZeroTerminated(byte[] data, Encoding enc, int charSize)
        {
            int len = 0;
            while (len + charSize <= data.Length)
            {
                bool zero = true;
                for (int i = 0; i < charSize; i++)
                    zero &= data[len + i] == 0;
                if (zero)
                    break;
                len += charSize;
            }
            return enc.GetString(data, 0, len);
        }

        private void btnClipRead_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabClipboard;
            ClipHeader("Чтение данных из буфера обмена (GetClipboardData)");

            if (!OpenClipboard(Handle))
            {
                ShowApiError("OpenClipboard", Marshal.GetLastWin32Error(), "Буфер обмена занят другим приложением.");
                return;
            }
            try
            {
                ClipLog("OpenClipboard(Handle) — буфер открыт");
                uint customFmt = RegisterClipboardFormat(CustomFormatName);
                var formats = new[]
                {
                    (Id: CF_UNICODETEXT, Enc: Encoding.Unicode, CharSize: 2),
                    (Id: CF_TEXT, Enc: Encoding.GetEncoding((int)GetACP()), CharSize: 1),
                    (Id: CF_OEMTEXT, Enc: Encoding.GetEncoding((int)GetOEMCP()), CharSize: 1),
                    (Id: customFmt, Enc: Encoding.UTF8, CharSize: 1)
                };

                bool any = false;
                foreach (var f in formats)
                {
                    string name = FormatName(f.Id);
                    if (!IsClipboardFormatAvailable(f.Id))
                    {
                        ClipLog($"IsClipboardFormatAvailable({name}) = False — формат недоступен");
                        continue;
                    }
                    IntPtr h = GetClipboardData(f.Id);
                    byte[] data = h == IntPtr.Zero ? null : ReadGlobalBytes(h);
                    if (data == null)
                    {
                        ClipLog($"GetClipboardData({name}) — ошибка {ErrorText(Marshal.GetLastWin32Error())}");
                        continue;
                    }
                    string s = DecodeZeroTerminated(data, f.Enc, f.CharSize);
                    ClipLog($"GetClipboardData({name}): {data.Length} байт, кодовая страница {f.Enc.CodePage}");
                    ClipLog($"    \"{s}\"");
                    any = true;
                }

                if (IsClipboardFormatAvailable(CF_LOCALE))
                {
                    IntPtr h = GetClipboardData(CF_LOCALE);
                    byte[] data = h == IntPtr.Zero ? null : ReadGlobalBytes(h);
                    if (data != null && data.Length >= 4)
                    {
                        int lcid = BitConverter.ToInt32(data, 0);
                        string culture;
                        try { culture = new CultureInfo(lcid).DisplayName; }
                        catch (CultureNotFoundException) { culture = "?"; }
                        ClipLog($"GetClipboardData(CF_LOCALE): LCID = ${lcid:X4} ({culture})");
                    }
                }

                if (!any)
                    ClipLog("Текстовых данных в буфере обмена нет.");
            }
            finally
            {
                CloseClipboard();
                ClipLog("CloseClipboard — буфер закрыт");
            }
        }

        private void btnClipEmpty_Click(object sender, EventArgs e)
        {
            tabControl.SelectedTab = tabClipboard;
            ClipHeader("Очистка буфера обмена (EmptyClipboard)");
            if (!OpenClipboard(Handle))
            {
                ShowApiError("OpenClipboard", Marshal.GetLastWin32Error(), "Буфер обмена занят другим приложением.");
                return;
            }
            bool ok = EmptyClipboard();
            int err = Marshal.GetLastWin32Error();
            CloseClipboard();
            if (!ok)
            {
                ShowApiError("EmptyClipboard", err);
                return;
            }
            ClipLog("OpenClipboard → EmptyClipboard → CloseClipboard: буфер обмена очищен");
            RefreshClipboardFormats(true);
        }
    }
}
