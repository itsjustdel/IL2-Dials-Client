using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;

public class DisplayManager : MonoBehaviour
{
    public SlaveManager slaveManager;

    //https://answers.unity.com/questions/13523/is-there-a-way-to-set-the-position-of-a-standalone.html


#if UNITY_STANDALONE_WIN || UNITY_EDITOR


    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;        // x position of upper-left corner
        public int Top;         // y position of upper-left corner
        public int Right;       // x position of lower-right corner
        public int Bottom;      // y position of lower-right corner
    }

    //imported functions
    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    private static extern IntPtr FindWindow(System.String className, System.String windowName);
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern System.IntPtr GetActiveWindow();

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetCurrentProcess();

    [DllImport("user32.dll")]
    static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    

    public bool savePos;
    public bool setPos;
    RECT prevRECT;
    RECT rect;

    private void Awake()
    {
//        enabled = false;

  //      StartCoroutine(Wait());
    }

    private void Start()
    {
        Set();
        //
    }
    void Set()
    {
        string jsonFoo = PlayerPrefs.GetString("WindowInfo " + slaveManager.id.ToString());
        if (System.String.IsNullOrEmpty(jsonFoo))
        {
            UnityEngine.Debug.Log("No window pos key found");
        }
        else
        {
            UnityEngine.Debug.Log("Setting window pos on load");
            rect = JsonUtility.FromJson<RECT>(jsonFoo);
            //set window position!

            int x = rect.Left;
            int y = rect.Top;
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;
            SetWindowPos(ProcessHelper.GetProcessHandle(Process.GetCurrentProcess().Id), 0, x, y, width, height, width * height == 0 ? 1 : 0);
        }

        enabled = true;
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);

        Set();
       
    }


    // Use this for initialization
    void Update()
    {
        //needed every frame?
        IntPtr mainPtr = ProcessHelper.GetProcessHandle(Process.GetCurrentProcess().Id);// GetActiveWindow();// ProcessHelper.GetProcessHandle(Process.GetCurrentProcess().Id);

        //UnityEngine.Debug.Log("h = " + mainPtr);

        prevRECT = rect;
        rect = GetPosition(mainPtr); //think about active window

        if (prevRECT.Top != rect.Top || prevRECT.Bottom != rect.Bottom || prevRECT.Left != rect.Left || prevRECT.Right != rect.Right)
        {


            UnityEngine.Debug.Log("Detected window change");

            
            //save new window info to player prefs
            //attach to slave id
            int id = slaveManager.id;

            //pack with json utility
            string jsonFoo = JsonUtility.ToJson(rect);

            //save packed string to player preferences (unity)
            //
        //will save master as 0 and slaves as id number
            string key = "WindowInfo " + slaveManager.id.ToString();

            PlayerPrefs.SetString(key, jsonFoo);
            PlayerPrefs.Save();

        
        }


        if (savePos)
        {
            GetPosition(mainPtr);
            savePos = false;
        }
        if (setPos)
        {
           // SetWindowPosition(mainPtr, rect.Left, rect.Top);
            setPos = false;
        }

        //SetPosition(0, 0);
    }

    /*
    public static void SetWindowPosition(IntPtr windowHandle, int x, int y)
    {
        //double check active window with name string check?
        //use named string rename slaves? and close on change
        int resX = 0;
        int resY = 0;
        SetWindowPos(windowHandle, 0, x, y, resX, resY, resX * resY == 0 ? 1 : 0);

    }
    */


    public static RECT GetPosition(IntPtr hwnd)
    {
        //rect = new RECT();
        RECT _rect = new RECT();
        GetWindowRect(hwnd, out _rect);

        //Debug.ClearDeveloperConsole();
        //Debug.Log("Top = " + rect.Top);
        //Debug.Log("Left = " + rect.Left);

        return _rect;
    }

    public static class ProcessHelper
    {
        private static class Win32
        {
            internal const uint GwOwner = 4;

            internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            internal static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            internal static extern int GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);

            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            internal static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

            [DllImport("User32.dll", CharSet = CharSet.Auto)]
            internal static extern bool IsWindowVisible(IntPtr hWnd);
        }

        public static IntPtr GetProcessHandle(int processId)
        {
            IntPtr processPtr = IntPtr.Zero;

            Win32.EnumWindows((hWnd, lParam) =>
            {
                IntPtr pid;
                Win32.GetWindowThreadProcessId(hWnd, out pid);

                if (pid == lParam &&
                    Win32.IsWindowVisible(hWnd) &&
                    Win32.GetWindow(hWnd, Win32.GwOwner) == IntPtr.Zero)
                {
                    processPtr = hWnd;
                    return false;
                }

                return true;

            }, new IntPtr(processId));

            return processPtr;
        }
    }
#endif

}