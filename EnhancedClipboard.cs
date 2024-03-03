// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.InteropServices;



namespace Community.PowerToys.Run.Plugin._1Password;
public static class EnhancedClipboard
{
    const uint CF_UNICODETEXT = 13;

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GlobalLock(IntPtr hMem);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GlobalUnlock(IntPtr hMem);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool OpenClipboard(IntPtr hWndNewOwner);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool CloseClipboard();

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr SetClipboardData(uint uFormat, IntPtr data);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool EmptyClipboard();

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern uint RegisterClipboardFormat(string format);

    private static void ThrowWin32()
    {
        throw new Win32Exception(Marshal.GetLastWin32Error());
    }

    private static bool TryOpenClipboard(int retries = 10, int delay = 100)
    {
        while (retries-- > 0)
        {
            if (OpenClipboard(IntPtr.Zero))
            {
                return true;
            }
            Thread.Sleep(delay);
        }
        return false;
    }

    public static bool CopyHelper(string? value, bool WindowsEnableHistory = true, bool WindowsEnableRoaming = true)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        IntPtr hGlobal = IntPtr.Zero;
        Dictionary<string, IntPtr> optionsAllocations = new();

        try
        {
            if (!TryOpenClipboard())
            {
                ThrowWin32();
            }

            EmptyClipboard();

            // Allocate and set Unicode text to clipboard
            hGlobal = Marshal.StringToHGlobalUni(value);
            if (SetClipboardData(CF_UNICODETEXT, hGlobal) == IntPtr.Zero)
            {
                ThrowWin32();
            }
            hGlobal = IntPtr.Zero; // Clipboard now owns the memory

            // Custom clipboard formats handling
            if (!WindowsEnableHistory && !WindowsEnableRoaming)
            {
                SetCustomClipboardData("ExcludeClipboardContentFromMonitorProcessing", optionsAllocations);
            }

            if (!WindowsEnableHistory)
            {
                SetCustomClipboardData("CanIncludeInClipboardHistory", optionsAllocations, 0);
            }

            if (!WindowsEnableRoaming)
            {
                SetCustomClipboardData("CanUploadToCloudClipboard", optionsAllocations, 0);
            }
        }
        finally
        {
            CloseClipboard();

            if (hGlobal != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(hGlobal);
            }

            // Free allocated memory for options
            foreach (var allocation in optionsAllocations.Values)
            {
                Marshal.FreeHGlobal(allocation);
            }
        }

        return true;
    }

    private static void SetCustomClipboardData(string formatName, Dictionary<string, IntPtr> allocations, uint value = 1)
    {
        uint format = RegisterClipboardFormat(formatName);
        IntPtr allocation = Marshal.AllocHGlobal(sizeof(uint));
        Marshal.WriteInt32(allocation, unchecked((int)value));
        if (SetClipboardData(format, allocation) == IntPtr.Zero)
        {
            Marshal.FreeHGlobal(allocation); // Free immediately if setting data fails
        }
        else
        {
            allocations.Add(formatName, allocation); // Keep track for later freeing
        }
    }
}