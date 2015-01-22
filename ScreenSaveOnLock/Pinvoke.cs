using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ScreenSaveOnLock {
    class Pinvoke {
        // http://www.codeproject.com/Articles/17067/Controlling-The-Screen-Saver-With-C
        private const int SPI_GETSCREENSAVERRUNNING = 0x0072;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool SystemParametersInfo(int uAction, int uParam, ref bool lpvParam, int flags);

        public static bool GetScreenSaverRunning() {
            bool isRunning = false;

            SystemParametersInfo(SPI_GETSCREENSAVERRUNNING, 0, ref isRunning, 0);

            return isRunning;
        }

        // http://www.pinvoke.net/default.aspx/user32.GetDesktopWindow
        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr GetDesktopWindow();

        private const Int32 WM_SYSCOMMAND = 0x0112;
        private const Int32 SC_SCREENSAVE = 0xF140;
        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xFFFF);

        // http://www.pinvoke.net/default.aspx/user32.SendMessage
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);

        public static void StartScreenSaver() {
            //SendMessage(Program.formMain.Handle, WM_SYSCOMMAND, SC_SCREENSAVE, 0);
            IntPtr desktop = GetDesktopWindow();
            SendMessage(desktop, WM_SYSCOMMAND, SC_SCREENSAVE, 0);
        }

        // http://www.pinvoke.net/default.aspx/user32.LockWorkStation
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool LockWorkStation();

        public static void LockWorkStationSafe() {
            bool result = LockWorkStation();

            if (result == false) {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        // http://www.pinvoke.net/default.aspx/Structures/LASTINPUTINFO.html
        [StructLayout(LayoutKind.Sequential)]
        private struct LASTINPUTINFO {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }

        // http://www.pinvoke.net/default.aspx/user32.GetLastInputInfo
        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        public static uint GetLastInputTime() {
            uint idleTime = 0;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            uint envTicks = (uint)Environment.TickCount;

            if (GetLastInputInfo(ref lastInputInfo)) {
                uint lastInputTick = lastInputInfo.dwTime;

                idleTime = envTicks - lastInputTick;
            }

            return idleTime;
        }
    }
}
