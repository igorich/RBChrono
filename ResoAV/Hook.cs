using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace ResoAV
{
    public class ResoavEventArgs : EventArgs
    {
        public ResoavEventArgs(Keys k)
        {
            key = k;
        }
        private Keys key;

        public Keys Key
        {
            get { return key; }
            set { key = value; }
        }
    }

    public class InterceptKeys
    {
        private static bool _CtrlPressed;

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        public static event EventHandler<ResoavEventArgs> ResoavHookKey;

        public static void Main1()
        {
            _hookID = SetHook(_proc);
            //Application.Run();
            //UnhookWindowsHookEx(_hookID);
        }

        static public void UnhookWindowsHookEx1()
        {
            UnhookWindowsHookEx(_hookID);
        }

        public static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);


		public static Dictionary<Keys, bool> KeysUnderLook = new Dictionary<Keys,bool>();

        private static IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            int vkCode = Marshal.ReadInt32(lParam);

            if ((Keys)vkCode == Keys.ControlKey || (Keys)vkCode == Keys.LControlKey || (Keys)vkCode == Keys.RControlKey)
                _CtrlPressed = wParam == (IntPtr)WM_KEYDOWN;
			bool oldValue = false;
			if(KeysUnderLook.ContainsKey((Keys)vkCode))
			{
				oldValue = KeysUnderLook[(Keys)vkCode];
				KeysUnderLook[(Keys)vkCode] = wParam == (IntPtr)WM_KEYDOWN;
			}
            EventHandler<ResoavEventArgs> handler = ResoavHookKey;
            if (handler != null && _CtrlPressed &&
                !((Keys)vkCode == Keys.ControlKey || (Keys)vkCode == Keys.LControlKey || (Keys)vkCode == Keys.RControlKey)
				&& KeysUnderLook.ContainsKey((Keys)vkCode)
				&& oldValue == false &&
				KeysUnderLook[(Keys)vkCode] == true)
            {
				Console.WriteLine(String.Format("{0} - {1} - {2}", (Keys)vkCode, wParam == (IntPtr)WM_KEYDOWN ? "Down" : "Up", lParam));
				var arg = new ResoavEventArgs((Keys)vkCode);
                handler(null, arg);
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}