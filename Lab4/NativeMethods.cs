using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Lab4
{
    /// <summary>
    /// Объявления функций Win32 API (kernel32.dll, user32.dll), используемых в работе.
    /// </summary>
    internal static class NativeMethods
    {
        // ------------------------------------------------------------------
        //  Общие константы
        // ------------------------------------------------------------------
        public const int HFILE_ERROR = -1;
        public const uint INVALID_FILE_ATTRIBUTES = 0xFFFFFFFF;
        public const uint INVALID_SET_FILE_POINTER = 0xFFFFFFFF;
        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        // Коды ошибок
        public const int ERROR_FILE_NOT_FOUND = 2;
        public const int ERROR_PATH_NOT_FOUND = 3;
        public const int ERROR_ACCESS_DENIED = 5;
        public const int ERROR_LOCK_VIOLATION = 33;
        public const int ERROR_FILE_EXISTS = 80;
        public const int ERROR_ALREADY_EXISTS = 183;

        // Режимы OpenFile / _lopen
        public const uint OF_READ = 0x0000;
        public const uint OF_WRITE = 0x0001;
        public const uint OF_READWRITE = 0x0002;
        public const uint OF_SHARE_DENY_NONE = 0x0040;
        public const uint OF_CREATE = 0x1000;
        public const uint OF_EXIST = 0x4000;

        // Точка отсчёта для _llseek / SetFilePointer
        public const int FILE_BEGIN = 0;
        public const int FILE_CURRENT = 1;
        public const int FILE_END = 2;

        // Атрибуты файлов
        public const uint FILE_ATTRIBUTE_READONLY = 0x0001;
        public const uint FILE_ATTRIBUTE_HIDDEN = 0x0002;
        public const uint FILE_ATTRIBUTE_SYSTEM = 0x0004;
        public const uint FILE_ATTRIBUTE_DIRECTORY = 0x0010;
        public const uint FILE_ATTRIBUTE_ARCHIVE = 0x0020;
        public const uint FILE_ATTRIBUTE_NORMAL = 0x0080;

        // CreateFile
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint FILE_WRITE_ATTRIBUTES = 0x0100;
        public const uint FILE_SHARE_READ = 0x0001;
        public const uint FILE_SHARE_WRITE = 0x0002;
        public const uint OPEN_EXISTING = 3;

        // Типы дисков
        public const uint DRIVE_REMOVABLE = 2;
        public const uint DRIVE_FIXED = 3;
        public const uint DRIVE_REMOTE = 4;
        public const uint DRIVE_CDROM = 5;
        public const uint DRIVE_RAMDISK = 6;

        // ------------------------------------------------------------------
        //  Структуры
        // ------------------------------------------------------------------
        public const int OFS_MAXPATHNAME = 128;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct OFSTRUCT
        {
            public byte cBytes;
            public byte fFixedDisk;
            public ushort nErrCode;
            public ushort Reserved1;
            public ushort Reserved2;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = OFS_MAXPATHNAME)]
            public string szPathName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FILETIME
        {
            public uint dwLowDateTime;
            public uint dwHighDateTime;

            public long ToLong() => ((long)dwHighDateTime << 32) | dwLowDateTime;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WIN32_FIND_DATA
        {
            public uint dwFileAttributes;
            public FILETIME ftCreationTime;
            public FILETIME ftLastAccessTime;
            public FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        // ------------------------------------------------------------------
        //  Каталоги
        // ------------------------------------------------------------------
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateDirectory(string lpPathName, IntPtr lpSecurityAttributes);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RemoveDirectory(string lpPathName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern uint GetCurrentDirectory(uint nBufferLength, StringBuilder lpBuffer);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCurrentDirectory(string lpPathName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FindClose(IntPtr hFindFile);

        // ------------------------------------------------------------------
        //  Файлы: "старые" функции OpenFile/_lopen/_lread/_lwrite/_llseek/_lclose
        //  (работают только с ANSI-именами, длина пути до 128 символов)
        // ------------------------------------------------------------------
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, BestFitMapping = false)]
        public static extern int OpenFile(string lpFileName, ref OFSTRUCT lpReOpenBuff, uint uStyle);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, BestFitMapping = false)]
        public static extern int _lopen(string lpPathName, uint iReadWrite);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint _lwrite(int hFile, byte[] lpBuffer, uint uBytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint _lread(int hFile, [Out] byte[] lpBuffer, uint uBytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int _llseek(int hFile, int lOffset, int iOrigin);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int _lclose(int hFile);

        // ------------------------------------------------------------------
        //  Файлы: функции Win32
        // ------------------------------------------------------------------
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode,
            IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadFile(IntPtr hFile, [Out] byte[] lpBuffer, uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint SetFilePointer(IntPtr hFile, int lDistanceToMove, IntPtr lpDistanceToMoveHigh,
            uint dwMoveMethod);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LockFile(IntPtr hFile, uint dwFileOffsetLow, uint dwFileOffsetHigh,
            uint nNumberOfBytesToLockLow, uint nNumberOfBytesToLockHigh);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnlockFile(IntPtr hFile, uint dwFileOffsetLow, uint dwFileOffsetHigh,
            uint nNumberOfBytesToUnlockLow, uint nNumberOfBytesToUnlockHigh);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern uint GetFileAttributes(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetFileAttributes(string lpFileName, uint dwFileAttributes);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteFile(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MoveFile(string lpExistingFileName, string lpNewFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CopyFile(string lpExistingFileName, string lpNewFileName,
            [MarshalAs(UnmanagedType.Bool)] bool bFailIfExists);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetFileTime(IntPtr hFile, out long lpCreationTime, out long lpLastAccessTime,
            out long lpLastWriteTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetFileTime(IntPtr hFile, ref long lpCreationTime, IntPtr lpLastAccessTime,
            IntPtr lpLastWriteTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SystemTimeToFileTime(ref SYSTEMTIME lpSystemTime, out long lpFileTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FileTimeToSystemTime(ref long lpFileTime, out SYSTEMTIME lpSystemTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LocalFileTimeToFileTime(ref long lpLocalFileTime, out long lpFileTime);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FileTimeToLocalFileTime(ref long lpFileTime, out long lpLocalFileTime);

        // ------------------------------------------------------------------
        //  Диски и тома
        // ------------------------------------------------------------------
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetVolumeLabel(string lpRootPathName, string lpVolumeName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetVolumeInformation(string lpRootPathName, StringBuilder lpVolumeNameBuffer,
            uint nVolumeNameSize, out uint lpVolumeSerialNumber, out uint lpMaximumComponentLength,
            out uint lpFileSystemFlags, StringBuilder lpFileSystemNameBuffer, uint nFileSystemNameSize);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern uint GetLogicalDriveStrings(uint nBufferLength, [Out] char[] lpBuffer);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern uint GetDriveType(string lpRootPathName);

        // ------------------------------------------------------------------
        //  Память
        // ------------------------------------------------------------------
        public const ushort PROCESSOR_ARCHITECTURE_INTEL = 0;
        public const ushort PROCESSOR_ARCHITECTURE_MIPS = 1;
        public const ushort PROCESSOR_ARCHITECTURE_ALPHA = 2;
        public const ushort PROCESSOR_ARCHITECTURE_PPC = 3;
        public const ushort PROCESSOR_ARCHITECTURE_ARM = 5;
        public const ushort PROCESSOR_ARCHITECTURE_IA64 = 6;
        public const ushort PROCESSOR_ARCHITECTURE_AMD64 = 9;
        public const ushort PROCESSOR_ARCHITECTURE_ARM64 = 12;

        public const uint MEM_COMMIT = 0x1000;
        public const uint MEM_RESERVE = 0x2000;
        public const uint MEM_FREE = 0x10000;
        public const uint MEM_PRIVATE = 0x20000;
        public const uint MEM_MAPPED = 0x40000;
        public const uint MEM_IMAGE = 0x1000000;

        public const uint PAGE_NOACCESS = 0x01;
        public const uint PAGE_READONLY = 0x02;
        public const uint PAGE_READWRITE = 0x04;
        public const uint PAGE_WRITECOPY = 0x08;
        public const uint PAGE_EXECUTE = 0x10;
        public const uint PAGE_EXECUTE_READ = 0x20;
        public const uint PAGE_EXECUTE_READWRITE = 0x40;
        public const uint PAGE_EXECUTE_WRITECOPY = 0x80;
        public const uint PAGE_GUARD = 0x100;
        public const uint PAGE_NOCACHE = 0x200;
        public const uint PAGE_WRITECOMBINE = 0x400;

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            public ushort wProcessorArchitecture;
            public ushort wReserved;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public UIntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        }

        /// <summary>MEMORYSTATUS (для GlobalMemoryStatus). Поля размера SIZE_T.</summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORYSTATUS
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public UIntPtr dwTotalPhys;
            public UIntPtr dwAvailPhys;
            public UIntPtr dwTotalPageFile;
            public UIntPtr dwAvailPageFile;
            public UIntPtr dwTotalVirtual;
            public UIntPtr dwAvailVirtual;
        }

        /// <summary>MEMORYSTATUSEX (для GlobalMemoryStatusEx) — 64-битные поля.</summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
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
        public static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll")]
        public static extern void GlobalMemoryStatus(ref MEMORYSTATUS lpBuffer);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UIntPtr VirtualQuery(IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer,
            UIntPtr dwLength);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", EntryPoint = "K32GetMappedFileNameW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern uint GetMappedFileName(IntPtr hProcess, IntPtr lpv, StringBuilder lpFilename, uint nSize);

        // ------------------------------------------------------------------
        //  Глобальная память (для буфера обмена)
        // ------------------------------------------------------------------
        public const uint GMEM_MOVEABLE = 0x0002;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GlobalFree(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UIntPtr GlobalSize(IntPtr hMem);

        [DllImport("kernel32.dll")]
        public static extern uint GetACP();

        [DllImport("kernel32.dll")]
        public static extern uint GetOEMCP();

        // ------------------------------------------------------------------
        //  Буфер обмена (user32.dll)
        // ------------------------------------------------------------------
        public const uint CF_TEXT = 1;
        public const uint CF_BITMAP = 2;
        public const uint CF_OEMTEXT = 7;
        public const uint CF_DIB = 8;
        public const uint CF_UNICODETEXT = 13;
        public const uint CF_LOCALE = 16;

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EmptyClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int CountClipboardFormats();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint EnumClipboardFormats(uint format);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetClipboardFormatName(uint format, StringBuilder lpszFormatName, int cchMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetClipboardOwner();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetOpenClipboardWindow();

        [DllImport("user32.dll")]
        public static extern uint GetClipboardSequenceNumber();

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsClipboardFormatAvailable(uint format);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern uint RegisterClipboardFormat(string lpszFormat);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
    }
}
