//  
//  Copyright (c) 2012-2023, Arsenal Consulting, Inc. (d/b/a Arsenal Recon) <http://www.ArsenalRecon.com>
//  This source code and API are available under the terms of the Affero General Public
//  License v3.
// 
//  Please see LICENSE.txt for full license terms, including the availability of
//  proprietary exceptions.
//  Questions, comments, or requests for clarification: http://ArsenalRecon.com/contact/
// 

using LTRData.Extensions.Buffers;
using LTRData.Extensions.IO;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

namespace LTRData.Extensions.Native;

/// <summary>
/// </summary>
public static class NativeLib
{
    /// <summary>
    /// Encapsulates call to a Win32 API function that returns a value where failure
    /// is indicated as a NULL return and GetLastError() returns an error code. If
    /// non-zero value is passed to this method it just returns that value. If zero
    /// value is passed, it calls GetLastError() and throws a managed exception for
    /// that error code.
    /// </summary>
    /// <param name="result">Return code from a Win32 API function call.</param>
    public static T Win32Try<T>(T result)
        => result is null ? throw new Win32Exception() : result;

#if NETCOREAPP

    /// <summary>
    /// </summary>
    public static nint CrtDllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath) => !IOExtensions.IsWindows &&
        (libraryName.StartsWith("msvcr", StringComparison.OrdinalIgnoreCase) ||
        libraryName.StartsWith("msvcp", StringComparison.OrdinalIgnoreCase) ||
        libraryName.Equals("ntdll", StringComparison.OrdinalIgnoreCase) ||
        libraryName.Equals("advapi32", StringComparison.OrdinalIgnoreCase) ||
        libraryName.Equals("kernel32", StringComparison.OrdinalIgnoreCase) ||
        libraryName.Equals("crtdll", StringComparison.OrdinalIgnoreCase))
        ? NativeLibrary.Load("c", assembly, searchPath)
        : 0;

    /// <summary>
    /// </summary>
    public static TDelegate GetProcAddress<TDelegate>(nint hModule, string procedureName) where TDelegate : Delegate
        => Marshal.GetDelegateForFunctionPointer<TDelegate>(NativeLibrary.GetExport(hModule, procedureName));

    /// <summary>
    /// </summary>
    public static TDelegate? GetProcAddressNoThrow<TDelegate>(nint hModule, string procedureName) where TDelegate : Delegate
        => NativeLibrary.TryGetExport(hModule, procedureName, out var fptr)
            ? Marshal.GetDelegateForFunctionPointer<TDelegate>(fptr)
            : null;

    /// <summary>
    /// </summary>
    public static TDelegate GetProcAddress<TDelegate>(string moduleName, string procedureName)
    {
        var hModule = NativeLibrary.Load(moduleName);

        return Marshal.GetDelegateForFunctionPointer<TDelegate>(NativeLibrary.GetExport(hModule, procedureName));
    }

    /// <summary>
    /// </summary>
    public static nint GetProcAddressNoThrow(string moduleName, string procedureName)
    {
        if (!NativeLibrary.TryLoad(moduleName, out var hModule))
        {
            return default;
        }

        return !NativeLibrary.TryGetExport(hModule, procedureName, out var address) ? default : address;
    }

    /// <summary>
    /// </summary>
    public static TDelegate? GetProcAddressNoThrow<TDelegate>(string moduleName, string procedureName) where TDelegate : Delegate
    {
        var fptr = GetProcAddressNoThrow(moduleName, procedureName);

        return fptr == 0 ? null : Marshal.GetDelegateForFunctionPointer<TDelegate>(fptr);
    }

#elif NET471_OR_GREATER

    /// <summary>
    /// </summary>
    public static TDelegate GetProcAddress<TDelegate>(nint hModule, string procedureName) where TDelegate : Delegate
        => Marshal.GetDelegateForFunctionPointer<TDelegate>(Win32Try(UnsafeNativeMethods.GetProcAddress(hModule, procedureName)));

    /// <summary>
    /// </summary>
    public static TDelegate? GetProcAddressNoThrow<TDelegate>(nint hModule, string procedureName) where TDelegate : Delegate
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return null;
        }

        var fptr = UnsafeNativeMethods.GetProcAddress(hModule, procedureName);

        return fptr == 0 ? null : Marshal.GetDelegateForFunctionPointer<TDelegate>(fptr);
    }

    /// <summary>
    /// </summary>
    public static TDelegate GetProcAddress<TDelegate>(string moduleName, string procedureName) where TDelegate : Delegate
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new PlatformNotSupportedException();
        }

        var hModule = Win32Try(UnsafeNativeMethods.LoadLibraryW(moduleName.AsRef()));

        return Marshal.GetDelegateForFunctionPointer<TDelegate>(Win32Try(UnsafeNativeMethods.GetProcAddress(hModule, procedureName)));
    }

    /// <summary>
    /// </summary>
    public static nint GetProcAddressNoThrow(string moduleName, string procedureName)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return 0;
        }

        var hModule = UnsafeNativeMethods.LoadLibraryW(moduleName.AsRef());

        return hModule == 0 ? default : UnsafeNativeMethods.GetProcAddress(hModule, procedureName);
    }

    /// <summary>
    /// </summary>
    public static TDelegate? GetProcAddressNoThrow<TDelegate>(string moduleName, string procedureName) where TDelegate : Delegate
    {
        var fptr = GetProcAddressNoThrow(moduleName, procedureName);

        return fptr == 0 ? null : Marshal.GetDelegateForFunctionPointer<TDelegate>(fptr);
    }

#endif
}
