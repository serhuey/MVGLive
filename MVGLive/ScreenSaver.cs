// Copyright (c) Sergei Grigorev. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace MVGLive
{
    public static class ScreenSaver
    {
        public static void DisableSleep()
        {
            if (SafeNativeMethods.SetThreadExecutionState(SafeNativeMethods.EXECUTION_STATE.ES_CONTINUOUS |
                                            SafeNativeMethods.EXECUTION_STATE.ES_DISPLAY_REQUIRED |
                                            SafeNativeMethods.EXECUTION_STATE.ES_SYSTEM_REQUIRED |
                                            SafeNativeMethods.EXECUTION_STATE.ES_AWAYMODE_REQUIRED
                                        ) == 0)                                         //Away mode for Windows >= Vista 
                SafeNativeMethods.SetThreadExecutionState(SafeNativeMethods.EXECUTION_STATE.ES_CONTINUOUS |
                                        SafeNativeMethods.EXECUTION_STATE.ES_DISPLAY_REQUIRED |
                                        SafeNativeMethods.EXECUTION_STATE.ES_SYSTEM_REQUIRED);         //Windows < Vista, forget away mode 
        }

        public static void EnableSleep()
        {
            SafeNativeMethods.SetThreadExecutionState(SafeNativeMethods.EXECUTION_STATE.ES_CONTINUOUS);
        }

        /// <summary>
        /// Returns true if the screen saver is active  
        /// </summary>
        /// <returns></returns>
        public static bool GetScreenSaverActive()
        {
            bool isActive = false;

            SafeNativeMethods.SystemParametersInfo(SafeNativeMethods.SPI_GETSCREENSAVERACTIVE, 0,
               ref isActive, 0);
            return isActive;
        }

        /// <summary>
        /// Activate or deactivate screensaver
        /// </summary>
        /// <param name="active">true to activate or false to deactivate</param>
        public static void SetScreenSaverActive(bool active)
        {
            int nullVar = 0;

            SafeNativeMethods.SystemParametersInfo(SafeNativeMethods.SPI_SETSCREENSAVERACTIVE,
               active ? 1 : 0, ref nullVar, SafeNativeMethods.SPIF_SENDWININICHANGE);
        }

        /// <summary>
        /// Returns the screen saver timeout setting, in seconds
        /// </summary>
        /// <returns>Timeout in seconds</returns>
        public static Int32 GetScreenSaverTimeout()
        {
            Int32 value = 0;

            SafeNativeMethods.SystemParametersInfo(SafeNativeMethods.SPI_GETSCREENSAVERTIMEOUT, 0,
               ref value, 0);
            return value;
        }

        /// <summary>
        /// Set screensaver timeout value
        /// </summary>
        /// <param name="value">Timeout in seconds</param>
        public static void SetScreenSaverTimeout(Int32 value)
        {
            int nullVar = 0;

            SafeNativeMethods.SystemParametersInfo(SafeNativeMethods.SPI_SETSCREENSAVERTIMEOUT,
               value, ref nullVar, SafeNativeMethods.SPIF_SENDWININICHANGE);
        }

        /// <summary>
        /// Get actual screen saver status
        /// </summary>
        /// <returns>True if screen saver is running</returns>
        public static bool GetScreenSaverRunning()
        {
            bool isRunning = false;

            SafeNativeMethods.SystemParametersInfo(SafeNativeMethods.SPI_GETSCREENSAVERRUNNING, 0,
               ref isRunning, 0);
            return isRunning;
        }

        // From Microsoft's Knowledge Base article #140723: 
        // http://support.microsoft.com/kb/140723
        // "How to force a screen saver to close once started 
        // in Windows NT, Windows 2000, and Windows Server 2003"

        public static void KillScreenSaver()
        {
            IntPtr hDesktop = SafeNativeMethods.OpenDesktop("Screen-saver", 0,
               false, SafeNativeMethods.DESKTOP_READOBJECTS | SafeNativeMethods.DESKTOP_WRITEOBJECTS);
            if (hDesktop != IntPtr.Zero)
            {
                SafeNativeMethods.EnumDesktopWindows(hDesktop, new
                   SafeNativeMethods.EnumDesktopWindowsProc(KillScreenSaverFunc),
                   IntPtr.Zero);
                SafeNativeMethods.CloseDesktop(hDesktop);
            }
            else
            {
                SafeNativeMethods.PostMessage(SafeNativeMethods.GetForegroundWindow(), SafeNativeMethods.WM_CLOSE,
                   (IntPtr)0, (IntPtr)0);
            }
        }

        private static bool KillScreenSaverFunc(IntPtr hWnd,
           IntPtr lParam)
        {
            if (SafeNativeMethods.IsWindowVisible(hWnd))
                SafeNativeMethods.PostMessage(hWnd, SafeNativeMethods.WM_CLOSE, (IntPtr)0, (IntPtr)0);
            return true;
        }
    }
}



