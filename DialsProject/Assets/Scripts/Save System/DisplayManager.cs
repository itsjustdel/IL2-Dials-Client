using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;

public class DisplayManager : MonoBehaviour
{
    public SlaveManager slaveManager;
    public GameObject menuPanel;
    //public GameObject displayManagerPanel;

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
    public static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

    [DllImport("user32.dll")]
    static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    

    public bool savePos;
    public bool setPos;
    RECT prevRECT;
    RECT rect;


    private void Start()
    {

        Set(slaveManager.id.ToString());
        SetFullscreen();
        //
    }
    private void SetFullscreen()
    {
        string isFullscreen = PlayerPrefs.GetString("fullscreen" + " " + slaveManager.id.ToString());
        UnityEngine.Debug.Log("Is full screen = " + isFullscreen);

        if (isFullscreen == "True")
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else
            Screen.fullScreenMode = FullScreenMode.Windowed;
    }

    public void Set(string id)
    {
        string jsonFoo = PlayerPrefs.GetString("windowInfo " + id);
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
    }

    // Use this for initialization
    void Update()
    {
        //needed every frame?
        IntPtr mainPtr =  ProcessHelper.GetProcessHandle(Process.GetCurrentProcess().Id);
        //IntPtr mainPtr = slaveManager.handle;

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
            string key = "windowInfo " + slaveManager.id.ToString();

            PlayerPrefs.SetString(key, jsonFoo);
            //save if fullscreen too
            PlayerPrefs.SetString("fullscreen" + " " + id.ToString(),  Screen.fullScreen.ToString());
            PlayerPrefs.Save();

        }

        if (savePos)
        {
            GetPosition(mainPtr);
            savePos = false;
        }
        if (setPos)
        {
            setPos = false;
        }

    }
    public static RECT GetPosition(IntPtr hwnd)
    {
        RECT _rect = new RECT();
        GetWindowRect(hwnd, out _rect);

        return _rect;
    }


#endif

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