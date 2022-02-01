using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class PlaceWindow : MonoBehaviour
{

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


    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    public static extern IntPtr FindWindow(System.String className, System.String windowName);
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern System.IntPtr GetActiveWindow();
    [DllImport("user32.dll")]
    static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    public bool savePos;
    public bool setPos;
    RECT rect;

    

    public void GetPosition(IntPtr hwnd)
    {
        rect = new RECT();
        GetWindowRect(hwnd, out rect);

        Debug.ClearDeveloperConsole();
        Debug.Log("Top = " + rect.Top);
        Debug.Log("Left = " + rect.Left);
    }

    public static void SetPosition(int x, int y, int resX = 0, int resY = 0)
    {
        //double check active window with name string check?
        //use named string rename slaves? and close on change
        SetWindowPos(GetActiveWindow(), 0, x, y, resX, resY, resX * resY == 0 ? 1 : 0);
        
    }


    // Use this for initialization
    void Update()
    {

        if(savePos)
        {
            GetPosition(GetActiveWindow());
            savePos = false;
        }
        if(setPos)
        {
            SetPosition(rect.Left, rect.Top);
            setPos = false;
        }
        
        //SetPosition(0, 0);
    }


#endif

}