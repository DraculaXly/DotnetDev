using System;
using System.Runtime.InteropServices;
using System.Windows.Automation;

namespace UIAutomationDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            SetProcessDPIAware();
            ToolbarClick();
            Console.ReadKey();
        }

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        const int MOUSEEVENTF_MOVE = 0x0001;
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        const int MOUSEEVENTF_LEFTUP = 0x0004;
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const int MOUSEEVENTF_RIGHTUP = 0x0010;
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        const int MOUSEEVENTF_WHEEL = 0x0800;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        private static void ToolbarClick()
        {
            AutomationElement notificationElement = AutomationElement.FromHandle(RefreshTrayArea());
            if (notificationElement != null)
            {
                Console.WriteLine("Found!");
                AutomationElementCollection automationElementCollection = FindByMultipleConditions(notificationElement);
                foreach (AutomationElement ele in automationElementCollection)
                {
                    string nameProp = ele.GetCurrentPropertyValue(AutomationElement.NameProperty) as string;
                    if (nameProp.Contains("access"))
                    {
                        System.Windows.Point clickablePoint = ele.GetClickablePoint();
                        SetCursorPos(Convert.ToInt32(clickablePoint.X), Convert.ToInt32(clickablePoint.Y));
                        mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        break;
                    }
                }
            }
        }

        private static AutomationElementCollection FindByMultipleConditions(AutomationElement elementWindowElement)
        {
            Condition conditions = new AndCondition(new PropertyCondition(AutomationElement.IsEnabledProperty, true), new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));

            AutomationElementCollection elementCollection = elementWindowElement.FindAll(TreeScope.Children, conditions);
            return elementCollection;
        }

        private static IntPtr RefreshTrayArea()
        {
            IntPtr systemTrayContainerHandle = FindWindow("Shell_TrayWnd", null);
            IntPtr systemTrayHandle = FindWindowEx(systemTrayContainerHandle, IntPtr.Zero, "TrayNotifyWnd", null);
            IntPtr sysPagerHandle = FindWindowEx(systemTrayHandle, IntPtr.Zero, "SysPager", null);
            IntPtr notificationAreaHandle = FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", "User Promoted Notification Area");
            Console.WriteLine(notificationAreaHandle.ToString());
            return notificationAreaHandle;
        }
    }
}
