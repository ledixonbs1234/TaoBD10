using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;
using TaoBD10.Model;
using static TaoBD10.Manager.EnumAll;

namespace TaoBD10.Manager
{
    public static class APIManager
    {

        [DllImport("User32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);
        [DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);



        public const int SW_RESTORE = 9;
        public static int currentNumberBD = 0;
        public static void FocusHandle(IntPtr handle)
        {
            SendMessage(handle, 0x0007, 0, 0);
            SendMessage(handle, 0x0007, 0, 0);
        }
        public static void ClearClipboard()
        {
            Thread thread = new Thread(() => System.Windows.Clipboard.Clear());
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool InvalidateRect(IntPtr hWnd, IntPtr rect, bool bErase);
        public static List<ChildListModel> GetDataTable(AutomationElement table, int numberLeftHeader = 0)
        {
            try
            {
                List<ChildListModel> list = new List<ChildListModel>();
                var childs = table.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Custom));
                //Rows
                if (childs.Count == 0)
                    return null;
                for (int i = 1; i < childs.Count; i++)
                {
                    AutomationElement item = childs[i];
                    var listItem = item.FindAll(TreeScope.Children, Condition.TrueCondition);
                    ChildListModel tableItem = new ChildListModel();
                    tableItem.ChildList = new List<string>();
                    if (listItem.Count == 0)
                        return null;
                    for (int j = numberLeftHeader; j < listItem.Count; j++)
                    {
                        AutomationElement item1 = listItem[j];
                        ValuePattern pattern = item1.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                        tableItem.ChildList.Add(pattern.Current.Value);
                    }
                    list.Add(tableItem);
                }
                return list;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                OpenNotePad(ex.Message + '\n' + "APIManger " + line + " Number Line " + APIManager.GetLineNumber(ex), "loi ");
                throw;
            }

        }

        public static void ClickButton(IntPtr handle)
        {
            Thread.Sleep(100);
            PostMessage(handle, 0x00F5, 0, 0);
        }
        private const uint WM_SETTEXT = 0x000C;
        public static void setTextControl(IntPtr handle, string content)
        {
            SendMessage(handle, WM_SETTEXT, IntPtr.Zero, new StringBuilder(content));
        }

        //private static int WM_LBUTTONDOWN = 0x0201;
        //private static int WM_LBUTTONUP = 0x0202;
        public static void ClickButton(IntPtr window, string name, bool isExactly = true)
        {
            try
            {
                if (window == IntPtr.Zero)
                    return;

                List<TestAPIModel> controls = GetListControlText(window);
                if (controls.Count == 0)
                    return;
                foreach (TestAPIModel control in controls)
                {
                    if (isExactly)
                    {
                        if (control.Text == name)
                        {
                            ClickButton(control.Handle);
                            break;
                        }
                    }
                    else
                    {
                        if (BoDauAndToLower(control.Text).IndexOf(name) != -1)
                        {
                            ClickButton(control.Handle);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                APIManager.OpenNotePad(ex.Message + '\n' + "APIManager " + line + " Number Line " + APIManager.GetLineNumber(ex), "loi ");
                throw;
            }

        }

        public static void ClickButton(IntPtr mainHandle, IntPtr buttonHandle)
        {
            int wparam = (0 << 16) | (0x79 & 0xffff);
            APIManager.SendMessage(mainHandle, WM_COMMAND, wparam, (int)buttonHandle);
        }

        public static string ConvertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D').ToLower();
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

        public static DownLoadRoad downLoadRoad = DownLoadRoad.None;

        public static WindowInfo GetActiveWindowTitle(bool isExactly = false)
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                string text = Buff.ToString();
                if (isExactly != true)
                {
                    text = ConvertToUnSign3(text).ToLower();
                }
                return new WindowInfo(text, handle);
            }
            return new WindowInfo("", handle);
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

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetClassNameW(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        public static string GetControlText(IntPtr hWnd)
        {
            try
            {
                // Get the size of the string required to hold the window title (including trailing null.)
                int titleSize = (int)SendMessage(hWnd, WM_GETTEXTLENGTH, 0, 0);

                // If titleSize is 0, there is no title so return an empty string (or null)
                if (titleSize == 0)
                    return String.Empty;

                StringBuilder title = new StringBuilder(titleSize + 1);

                SendMessage(hWnd, WM_GETTEXT, new IntPtr(title.Capacity), title);

                return title.ToString();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                APIManager.OpenNotePad(ex.Message + '\n' + "APIManager " + line + " Number Line " + APIManager.GetLineNumber(ex), "loi ");
                throw;
            }

        }

        public static string GetCopyData()
        {
            string clipboard = "";
            Thread thread;
            thread = new Thread(() => clipboard = System.Windows.Clipboard.GetText());
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            for (int i = 0; i < 10; i++)
            {
                SendKeys.SendWait("^(c)");
                Thread.Sleep(50);

                thread.Start();
                thread.Join(); //Wait for the thread to end
                if (!string.IsNullOrEmpty(clipboard))
                {
                    break;
                }
            }
            return clipboard;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetLastActivePopup(IntPtr hWnd);

        public static int GetLineNumber(Exception ex)
        {
            var lineNumber = 0;
            const string lineSearch = ":line ";
            var index = ex.StackTrace.LastIndexOf(lineSearch);
            if (index != -1)
            {
                var lineNumberText = ex.StackTrace.Substring(index + lineSearch.Length);
                if (int.TryParse(lineNumberText, out lineNumber))
                {
                }
            }
            return lineNumber;
        }

        public static List<TestAPIModel> GetListControlText(IntPtr handleActiveWindow)
        {
            try
            {
                var allChild = GetAllChildHandles(handleActiveWindow);


                List<TestAPIModel> list = new List<TestAPIModel>();
                if (allChild.Count == 0)
                    return list;
                int count = 0;
                foreach (var item in allChild)
                {
                    TestAPIModel test = new TestAPIModel
                    {
                        Index = count,
                        Text = GetControlText(item),
                        Handle = item,
                        ClassName = GetWindowClass(item)
                    };
                    list.Add(test);
                    count++;
                }
                return list;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                APIManager.OpenNotePad(ex.Message + '\n' + "API MANager " + line + " Number Line " + APIManager.GetLineNumber(ex), "loi ");

                throw;
            }

        }

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
                    //throw new Win32Exception(Marshal.GetLastWin32Error());
                    return "null";
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

        public static void OpenNotePad(string message = null, string title = null)
        {
            Process notepad = Process.Start(new ProcessStartInfo("notepad.exe"));
            if (notepad != null)
            {
                notepad.WaitForInputIdle();

                if (!string.IsNullOrEmpty(title))
                    SetWindowText(notepad.MainWindowHandle, title);

                if (!string.IsNullOrEmpty(message))
                {
                    IntPtr child = FindWindowEx(notepad.MainWindowHandle, new IntPtr(0), "Edit", null);
                    SendMessage(child, 0x000C, 0, message);
                }
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam);

        [DllImportAttribute("User32.dll")]
        public static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        public static void SetPrintBD10()
        {
            if (!string.IsNullOrEmpty(namePrinterBD10))
            {
                SetDefaultPrinter(namePrinterBD10);
            }
        }

        public static void SetPrintBD8()
        {
            if (!string.IsNullOrEmpty(namePrinterBD8))
            {
                SetDefaultPrinter(namePrinterBD8);
            }
        }

        public static void ShowSnackbar(string content)
        {
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Snackbar", Content = content });
        }

        public static void ShowTest(string content)
        {
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Test", Content = content });
        }

        public static string BoDauAndToLower(string text)
        {
            return APIManager.ConvertToUnSign3(text).ToLower();
        }

        //Tu Dong vao ung dung 230 280
        public static IntPtr SetToLastWindow(string maBuuCuc)
        {
            Process[] processes = Process.GetProcesses();
            IntPtr windowHandle = IntPtr.Zero;
            bool isHaveProgram = false;

            foreach (Process p in processes)
            {
                if (p.ProcessName.IndexOf("Ctin") != -1)
                {
                    windowHandle = p.MainWindowHandle;
                    List<IntPtr> handles = GetAllChildHandles(windowHandle);
                    foreach (IntPtr item1 in handles)
                    {
                        string title = GetControlText(item1);
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
                IntPtr last = GetLastActivePopup(windowHandle);
                if (last != windowHandle)
                {
                    SetForegroundWindow(last);
                    return last;
                }
            }
            return IntPtr.Zero;
        }

        private static bool isWaitingThoat = false;
        public static void GoToWindow(string maBuuCuc, string nameHandle, string firstNumber, string twoNumber)
        {
            if (!ThoatToDefault(maBuuCuc, nameHandle))
            {
                SendKeys.SendWait(firstNumber);
                Thread.Sleep(200);
                SendKeys.SendWait(twoNumber);
            }
        }

        /// <summary>
        /// Thoat toi view
        /// </summary>
        /// <param name="maBuuCuc"> Mã Bưu Cục</param>
        /// <param name="nameHandleChildToHangOn">Tên có dấu</param>
        /// <returns></returns>
        public static bool ThoatToDefault(string maBuuCuc, string nameHandleChildToHangOn)
        {
            if (isWaitingThoat)
                return false;
            try
            {
                isWaitingThoat = true;
                Process[] processes = Process.GetProcesses();
                IntPtr windowHandle = IntPtr.Zero;
                bool isHaveProgram = false;

                foreach (Process p in processes)
                {
                    if (p.ProcessName.IndexOf("Ctin") != -1)
                    {
                        windowHandle = p.MainWindowHandle;
                        List<IntPtr> handles = GetAllChildHandles(windowHandle);
                        foreach (IntPtr item1 in handles)
                        {
                            string title = GetControlText(item1);
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
                    IntPtr last = GetLastActivePopup(windowHandle);
                    if (last != windowHandle)
                    {
                        SetForegroundWindow(last);

                        //Kiem tra handle name trong nay thu
                        WindowInfo currentWindow = GetActiveWindowTitle();

                        //thuc hien xoa tuan tu cho toi window main handle
                        while (currentWindow.hwnd != windowHandle)
                        {
                            currentWindow = GetActiveWindowTitle();
                            string textKoDau = ConvertToUnSign3(currentWindow.text).ToLower();
                            if (textKoDau.IndexOf(nameHandleChildToHangOn) != -1)
                            {
                                isWaitingThoat = false;
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
                    isWaitingThoat = false;
                    return false;
                }
                else
                {
                    isWaitingThoat = false;
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                APIManager.OpenNotePad(ex.Message + '\n' + "loi Line APIManager " + line + " Number Line " + APIManager.GetLineNumber(ex), "loi ");
                throw;
            }
        }

        /// <summary>
        /// Cho toi khi nao co active window
        /// </summary>
        /// <param name="title"> tieu de</param>
        /// <param name="time">thoi gian s* 0.2</param>
        /// <param name="isExactly">co chinh xac title khong</param>
        /// <returns>null neu khong tim thay</returns>
        public static WindowInfo WaitingFindedWindow(string title, string title2 = "null", int time = 8, bool isExactly = false)
        {
            try
            {
                WindowInfo currentWindow = null;
                string titleWindow = "";
                time *= 5;
                if (isExactly)
                {
                    while (titleWindow != title)
                    {
                        time--;
                        if (time <= 0)
                            return null;

                        Thread.Sleep(200);
                        currentWindow = GetActiveWindowTitle(true);

                        titleWindow = currentWindow.text;
                    }
                }
                else
                {
                    while (titleWindow.IndexOf(title) == -1 && titleWindow.IndexOf(title2) == -1)
                    {
                        time--;
                        if (time <= 0)
                            return null;

                        Thread.Sleep(200);
                        currentWindow = GetActiveWindowTitle();
                        if (!string.IsNullOrEmpty(currentWindow.text))
                        {
                            titleWindow = ConvertToUnSign3(currentWindow.text).ToLower();
                        }
                    }
                }
                Thread.Sleep(100);
                return currentWindow;
            }
            catch (Exception ex)
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                APIManager.OpenNotePad(ex.Message + '\n' + "loi Line API Manager " + line + " Number Line " + APIManager.GetLineNumber(ex), "loi ");
                throw;
            }
        }

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDefaultPrinter(string Name);

        [DllImport("user32.dll", EntryPoint = "SetWindowText")]
        private static extern int SetWindowText(IntPtr hWnd, string text);



        public delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

        public static string namePrinterBD10 = "";
        public static string namePrinterBD8 = "";
        private const int WM_CLOSE = 0x0010;
        private const int WM_COMMAND = 0x0111;
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

        public WindowInfo()
        {
            this.text = "";
            this.hwnd = IntPtr.Zero;
        }

        public IntPtr hwnd;
        public string text;
    }
}