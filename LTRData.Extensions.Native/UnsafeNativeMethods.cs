using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace LTRData.Extensions.Native;

#if NET5_0_OR_GREATER
[SupportedOSPlatform("windows")]
#endif
internal static class UnsafeNativeMethods
{
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
    internal static extern nint GetProcAddress(nint hModule, [In][MarshalAs(UnmanagedType.LPStr)] string lpEntryName);

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
    internal static extern nint GetProcAddress(nint hModule, nint ordinal);

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern nint GetModuleHandleW(in char ModuleName);

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern nint LoadLibraryW(in char lpFileName);

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool FreeLibrary(nint hModule);

}
