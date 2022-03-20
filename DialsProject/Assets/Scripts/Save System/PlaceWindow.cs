using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class PlaceWindow : MonoBehaviour
{

    //https://answers.unity.com/questions/13523/is-there-a-way-to-set-the-position-of-a-standalone.html


#if UNITY_STANDALONE_WIN && UNITY_EDITOR || UNITY_STANDALONE_WIN


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
    RECT prevRECT;
    RECT rect;

    

    public RECT GetPosition(IntPtr hwnd)
    {
        //rect = new RECT();
        RECT _rect = new RECT();
        GetWindowRect(hwnd, out _rect);

        //Debug.ClearDeveloperConsole();
        //Debug.Log("Top = " + rect.Top);
        //Debug.Log("Left = " + rect.Left);

        return _rect;
    }

    public static void SetPosition(int x, int y, int resX = 0, int resY = 0)
    {
        //double check active window with name string check?
        //use named string rename slaves? and close on change
        SetWindowPos(GetActiveWindow(), 0, x, y, resX, resY, resX * resY == 0 ? 1 : 0);
        
    }

    private void Start()
    {
        //check for window position key in player pref
        //unpack rect
        string jsonFoo = PlayerPrefs.GetString("WindowPos");
        if (System.String.IsNullOrEmpty(jsonFoo))
        {
            Debug.Log("No window pos key found");
        }
        else
        {
            Debug.Log("Setting window pos on load");
            rect = JsonUtility.FromJson<RECT>(jsonFoo);
            //set window position!
            SetPosition(rect.Left, rect.Top);//pass to launch on myProcess?
        }


    }


    // Use this for initialization
    void Update()
    {

        

        prevRECT = rect;

        rect = GetPosition(GetActiveWindow()); //think about active window

        if(prevRECT.Top != rect.Top || prevRECT.Bottom != rect.Bottom || prevRECT.Left != rect.Left || prevRECT.Right != rect.Right )
        {
            Debug.Log("Detected window change");
            //save new window info to player prefs
            //if master client
            //test, master client
            //master client is base player prefs
            //save window pos

            //pack with json utility
            string jsonFoo = JsonUtility.ToJson(rect);

            //save packed string to player preferences (unity)
            //
            string key = "WindowPos";

            PlayerPrefs.SetString(key, jsonFoo);
            PlayerPrefs.Save();
        }


        if (savePos)
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