// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

namespace MVGLive
{
    internal class SafeNativeMethods
    {
        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
        }

        // Signatures for unmanaged calls
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool SystemParametersInfo(
           int uAction, int uParam, ref int lpvParam,
           int flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool SystemParametersInfo(
           int uAction, int uParam, ref bool lpvParam,
           int flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int PostMessage(IntPtr hWnd,
           int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr OpenDesktop(
           string hDesktop, int Flags, bool Inherit,
           uint DesiredAccess);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool CloseDesktop(
           IntPtr hDesktop);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool EnumDesktopWindows(
           IntPtr hDesktop, EnumDesktopWindowsProc callback,
           IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool IsWindowVisible(
           IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        // Callbacks
        internal delegate bool EnumDesktopWindowsProc(
           IntPtr hDesktop, IntPtr lParam);

        // Constants
        internal const int SPI_GETSCREENSAVERACTIVE = 16;

        internal const int SPI_SETSCREENSAVERACTIVE = 17;
        internal const int SPI_GETSCREENSAVERTIMEOUT = 14;
        internal const int SPI_SETSCREENSAVERTIMEOUT = 15;
        internal const int SPI_GETSCREENSAVERRUNNING = 114;
        internal const int SPIF_SENDWININICHANGE = 2;

        internal const uint DESKTOP_WRITEOBJECTS = 0x0080;
        internal const uint DESKTOP_READOBJECTS = 0x0001;
        internal const int WM_CLOSE = 16;
    }
}