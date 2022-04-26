﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace TaoBD10.Manager
{
    public static class APIManager
    {
        public delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

        public static string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        public static bool EnumWindow(IntPtr hWnd, IntPtr lParam)
        {
            GCHandle gcChildhandlesList = GCHandle.FromIntPtr(lParam);

            if (gcChildhandlesList == null || gcChildhandlesList.Target == null)
            {
                return false;
            }

            List<IntPtr> childHandles = gcChildhandlesList.Target as List<IntPtr>;
            childHandles.Add(hWnd);

            return true;
        }

        public static WindowInfo GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return new WindowInfo(Buff.ToString(), handle);
            }
            return null;
        }

        public static List<IntPtr> GetAllChildHandles(IntPtr handle)
        {
            List<IntPtr> childHandles = new List<IntPtr>();

            GCHandle gcChildhandlesList = GCHandle.Alloc(childHandles);
            IntPtr pointerChildHandlesList = GCHandle.ToIntPtr(gcChildhandlesList);

            try
            {
                EnumWindowProc childProc = new EnumWindowProc(EnumWindow);
                EnumChildWindows(handle, childProc, pointerChildHandlesList);
            }
            finally
            {
                gcChildhandlesList.Free();
            }

            return childHandles;
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetClassNameW(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        public static string GetControlText(IntPtr hWnd)
        {
            // Get the size of the string required to hold the window title (including trailing null.)
            int titleSize = (int)APIManager.SendMessage(hWnd, WM_GETTEXTLENGTH, 0, 0);

            // If titleSize is 0, there is no title so return an empty string (or null)
            if (titleSize == 0)
                return String.Empty;

            StringBuilder title = new StringBuilder(titleSize + 1);

            APIManager.SendMessage(hWnd, (int)WM_GETTEXT, new IntPtr(title.Capacity), title);

            return title.ToString();
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetLastActivePopup(IntPtr hWnd);

        public static string GetWindowClass(IntPtr hWnd)
        {
            string className = String.Empty;
            int length = 10; // deliberately small so you can
                             // see the algorithm iterate several times.
            StringBuilder sb = new StringBuilder(1024);
            while (length < 1024)
            {
                int cchClassNameLength = GetClassNameW(hWnd, sb, length);
                if (cchClassNameLength == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                else if (cchClassNameLength < length - 1) // -1 for null terminator
                {
                    className = sb.ToString();
                    break;
                }
                else length *= 2;
            }
            return className;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, StringBuilder lParam);

        [DllImportAttribute("User32.dll")]
        public static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Thoat toi view
        /// </summary>
        /// <param name="maBuuCuc"> Mã Bưu Cục</param>
        /// <param name="nameHandleChildToHangOn">Tên có dấu</param>
        /// <returns></returns>
        public static bool ThoatToDefault(string maBuuCuc, string nameHandleChildToHangOn)
        {
            Process[] processes = Process.GetProcesses();
            IntPtr windowHandle = IntPtr.Zero;
            bool isHaveProgram = false;

            foreach (Process p in processes)
            {
                if (p.ProcessName.IndexOf("Ctin") != -1)
                {
                    windowHandle = p.MainWindowHandle;
                    var handles = APIManager.GetAllChildHandles(windowHandle);
                    foreach (IntPtr item1 in handles)
                    {
                        String title = GetControlText(item1);
                        if (title.ToString().IndexOf(maBuuCuc) != -1)
                        {
                            isHaveProgram = true;
                            break;
                        }
                    }
                    if (isHaveProgram)
                        break;
                }
            }
            if (isHaveProgram)
            {
                //APIManager.SetForegroundWindow(p.MainWindowHandle);
                IntPtr last = APIManager.GetLastActivePopup(windowHandle);
                if (last != windowHandle)
                {
                    SetForegroundWindow(last);

                    //Kiem tra handle name trong nay thu
                    WindowInfo currentWindow = GetActiveWindowTitle();

                    //thuc hien xoa tuan tu cho toi window main handle
                    while (currentWindow.hwnd != windowHandle)
                    {
                        currentWindow = APIManager.GetActiveWindowTitle();
                        if (currentWindow.text.IndexOf(nameHandleChildToHangOn) != -1)
                        {
                            return true;
                        }
                        last = GetLastActivePopup(windowHandle);
                        SetForegroundWindow(last);

                        if (last != windowHandle)
                        {
                            SendMessage(last, WM_CLOSE, 0, 0);
                        }
                        Thread.Sleep(100);
                    }
                }
                else
                {
                    APIManager.SetForegroundWindow(last);
                }
                Thread.Sleep(300);
                SendKeys.SendWait("{F7}");
                Thread.Sleep(300);

                return false;
            }
            else
            {
                return false;
            }

        }

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr lParam);
        private const int WM_CLOSE = 0x0010;
        private const int WM_GETTEXT = 0x000D;
        private const int WM_GETTEXTLENGTH = 0x000E;
    }

    public class WindowInfo
    {
        public WindowInfo(string text, IntPtr hwnd)
        {
            this.text = text;
            this.hwnd = hwnd;
        }

        public IntPtr hwnd;
        public string text;
    }
}