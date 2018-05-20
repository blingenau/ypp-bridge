using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using WindowsInput.Native;
using WindowsInput;

namespace quartermaster
{
    class Program
    {
        static Process yppProcess;

        [DllImport("user32.dll")]
        [return: MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]  
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);  
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]  
        static extern int GetWindowTextLength(IntPtr hWnd);

        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };

        static public void BringMainWindowToFront()
        {
            if (!yppProcess.HasExited)
            {
                // check if the window is hidden / minimized
                if (yppProcess.MainWindowHandle == IntPtr.Zero)
                {
                    // the window is hidden so try to restore it before setting focus.
                    ShowWindow(yppProcess.Handle, ShowWindowEnum.Restore);
                }

                // set user the focus to the window
                SetForegroundWindow(yppProcess.MainWindowHandle);
            }
        }

        static public void SendMessageToYPP(string message) {
            BringMainWindowToFront();

            InputSimulator sim = new InputSimulator();

            // send our message with a return
            sim.Keyboard.TextEntry(message);
            sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
        }

        static public void GetPirateInfo() {
            string title = yppProcess.MainWindowTitle;
            Regex pirateInfo = new Regex(@"(\w+) on the (\w+) ocean");
            Match match = pirateInfo.Match(title);
            if (match.Success) {
                // This means we've got 3 matches
                // The entire string (i.e. "Yyuuii on the Obsidian Ocean") at position 0
                // The Pirate name (i.e. "Yyuuii") at position 1
                // The Ocean name (i.e. "Obsidian") at position 1
                if (match.Groups.Count == 3) {
                    string pirateName = match.Groups[1].Value;
                    string oceanName = match.Groups[2].Value;
                }
            }
        }

        static public Process GetYppProcess() {
            // get the process
            Process bProcess = Process.GetProcessesByName("javaw").FirstOrDefault();

            // check if the process is running
            if (bProcess != null)
            {
                return bProcess;
            } else {
                throw new System.Exception("Process not running");
            }
        }

        static int Main(string[] args)
        {
            // Cache the running ypp process
            
            try
            {
                yppProcess = GetYppProcess();
            }
            catch (System.Exception)
            {
                return 1;
            }

            return 0;
        }
    }
}
