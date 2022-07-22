using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AntiAfkKick {
  class Native {
    internal struct LASTINPUTINFO {
      public uint cbSize;

      public uint dwTime;
    }

    /// <summary>
    /// Helps to find the idle time, (in milliseconds) spent since the last user input
    /// </summary>
    public class IdleTimeFinder {
      [DllImport("User32.dll")]
      private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

      [DllImport("Kernel32.dll")]
      private static extern uint GetLastError();

      public static uint GetIdleTime() {
        Native.LASTINPUTINFO lastInPut = default(Native.LASTINPUTINFO);
        lastInPut.cbSize = (uint)Marshal.SizeOf<Native.LASTINPUTINFO>(lastInPut);
        Native.IdleTimeFinder.GetLastInputInfo(ref lastInPut);
        return (uint)(Environment.TickCount - (int)lastInPut.dwTime);
      }
      /// <summary>
      /// Get the Last input time in milliseconds
      /// </summary>
      /// <returns></returns>
      public static long GetLastInputTime() {
        Native.LASTINPUTINFO lastInPut = default(Native.LASTINPUTINFO);
        lastInPut.cbSize = (uint)Marshal.SizeOf<Native.LASTINPUTINFO>(lastInPut);
        if (!Native.IdleTimeFinder.GetLastInputInfo(ref lastInPut)) {
          throw new Exception(Native.IdleTimeFinder.GetLastError().ToString());
        }
        return (long)((ulong)lastInPut.dwTime);
      }
    }

    public static bool TryFindGameWindow(out IntPtr hwnd) {
      hwnd = IntPtr.Zero;
      int pid;
      do {
        hwnd = Native.FindWindowEx(IntPtr.Zero, hwnd, "FFXIVGAME", null);
        if (hwnd == IntPtr.Zero) {
          break;
        }
        Native.GetWindowThreadProcessId(hwnd, out pid);
      }
      while (pid != Process.GetCurrentProcess().Id);
      return hwnd != IntPtr.Zero;
    }

    [DllImport("user32.dll")]
    static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpszClass, string lpszWindow);

    [DllImport("user32.dll")]
    static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern IntPtr GetForegroundWindow();

    public class Keypress {
      public const int LControlKey = 162;
      public const int Space = 32;

      public const uint WM_KEYUP = 0x101;
      public const uint WM_KEYDOWN = 0x100;

      [DllImport("user32.dll")]
      public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

      public static void SendKeycode(IntPtr hwnd, int keycode) {
        SendMessage(hwnd, WM_KEYDOWN, (IntPtr)keycode, (IntPtr)0);
        SendMessage(hwnd, WM_KEYUP, (IntPtr)keycode, (IntPtr)0);
      }
    }
  }
}
