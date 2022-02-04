using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using System.Runtime.InteropServices;

public class SlaveManager : MonoBehaviour
{
    public AirplaneData airplaneData;
    public bool createNew;
    public bool slave = false;
    public int id;
    public int monintorNumber = -1;


    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
    // Start is called before the first frame update

    //when a slave is launched by the master, the master sets its window size and position
    //when a slave loads for the first time, it runs Start
    //it should check if its id matches any of the keys in player prefs

    void Start()
    {
        string[] args = Environment.GetCommandLineArgs();

        UnityEngine.Debug.Log("args = ");
        foreach(string s in args)
            UnityEngine.Debug.Log(s);


        //get args and save them to public variables for easy access
        //first arg is file location of exe
        if (args.Length == 3)
        {
            if (args[1] == "Slave")
            {
                UnityEngine.Debug.Log("This is a slave");

                slave = true;
            }
         

            id = int.Parse( args[2] );
            UnityEngine.Debug.Log("id = " + id);

        }
        else
        {
            UnityEngine.Debug.Log("This is the master");
            //the master spawns all slave windows on startup
            //the amount of slavesuser has crated is stored in player prefs
            int slaves = PlayerPrefs.GetInt("slaves");
            for (int i = 0; i < slaves; i++)
            {
                //spawn a client window for each slave and pass the id (will be plus 1 of index - master is 0)
                SpawnOldSlave(i + 1);
            }

        }
    }

    //new slave
    public static void SpawnNewSlave()
    {

        //create id and pass as arg
        //id is slave count + 1
        int id = PlayerPrefs.GetInt("slaves");
        id++;
        PlayerPrefs.SetInt("slaves", id);
        //UnityEngine.Debug.Log("Slaves = " + id);

        string args = "Slave " + id;// System.DateTime.Now.ToString("hh.mm.ss.ffffff");

        var process = Process.GetCurrentProcess();
        string fullPath = process.MainModule.FileName;

        var myProcess = new Process();

        myProcess.StartInfo.FileName = fullPath;
        myProcess.StartInfo.Arguments = args;
        myProcess.Start();
    }

    public static void SpawnOldSlave(int id)
    {
        string args = "Slave " + id;

        var process = Process.GetCurrentProcess();
        string fullPath = process.MainModule.FileName;

        var myProcess = new Process();

     
        //window size and position -ise when loading - 
        //SetWindowPos(myProcess.MainWindowHandle, layout.rect.Top, layout.rect.Left);

        myProcess.StartInfo.FileName = fullPath;
        myProcess.StartInfo.Arguments = args;
        myProcess.Start();

        
    }

    //unused 
    static void WaitAndMove(Process myProcess, string id)
    {
        //use id to get window info + id from player prefs
        string jsonFoo = PlayerPrefs.GetString("WindowInfo " + id);
        if (System.String.IsNullOrEmpty(jsonFoo))
        {
            UnityEngine.Debug.Log("No window pos key found");
        }
        else
        {
            UnityEngine.Debug.Log("Setting window pos on load");
            DisplayManager.RECT rect = JsonUtility.FromJson<DisplayManager.RECT>(jsonFoo);
            //set window position!

            int x = rect.Left;
            int y = rect.Top;
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;


            MoveWindow(myProcess.MainWindowHandle, x, y, width, height, true);
        }
;
    }

}
